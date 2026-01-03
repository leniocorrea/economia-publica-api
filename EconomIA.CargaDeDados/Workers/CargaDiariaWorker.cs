using Cronos;
using EconomIA.CargaDeDados.Configuration;
using EconomIA.CargaDeDados.HealthChecks;
using EconomIA.CargaDeDados.Models;
using EconomIA.CargaDeDados.Observability;
using EconomIA.CargaDeDados.Repositories;
using EconomIA.CargaDeDados.Services;
using Microsoft.Extensions.Options;

namespace EconomIA.CargaDeDados.Workers;

public class CargaDiariaWorker : BackgroundService {
	private readonly IServiceScopeFactory scopeFactory;
	private readonly ILogger<CargaDiariaWorker> logger;
	private readonly WorkerConfiguration configuracao;
	private readonly CronExpression cronExpression;

	public CargaDiariaWorker(
		IServiceScopeFactory scopeFactory,
		ILogger<CargaDiariaWorker> logger,
		IOptions<WorkerConfiguration> configuracao) {
		this.scopeFactory = scopeFactory;
		this.logger = logger;
		this.configuracao = configuracao.Value;
		this.cronExpression = CronExpression.Parse(this.configuracao.CronExpression);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		logger.LogInformation("Worker de carga diaria iniciado. Cron: {CronExpression}", configuracao.CronExpression);

		while (!stoppingToken.IsCancellationRequested) {
			var proximaExecucao = cronExpression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Local);

			if (proximaExecucao is null) {
				logger.LogWarning("Nao foi possivel calcular proxima execucao. Tentando novamente em 1 minuto");
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
				continue;
			}

			var delay = proximaExecucao.Value - DateTimeOffset.UtcNow;

			if (delay > TimeSpan.Zero) {
				logger.LogInformation("Proxima execucao agendada para {ProximaExecucao:dd/MM/yyyy HH:mm:ss}", proximaExecucao.Value.LocalDateTime);

				try {
					await Task.Delay(delay, stoppingToken);
				} catch (OperationCanceledException) {
					break;
				}
			}

			if (!stoppingToken.IsCancellationRequested) {
				await ExecutarCargaAsync(stoppingToken);
			}
		}

		logger.LogInformation("Worker de carga diaria finalizado");
	}

	private async Task ExecutarCargaAsync(CancellationToken stoppingToken) {
		using var scope = scopeFactory.CreateScope();
		var servicos = scope.ServiceProvider;

		var execucoesCarga = servicos.GetRequiredService<ExecucoesCarga>();
		var orquestrador = servicos.GetRequiredService<ServicoOrquestradorImportacao>();

		var metricas = new MetricasExecucao();
		var execucao = await execucoesCarga.IniciarExecucaoAsync(ModoExecucao.Incremental, TipoGatilho.Scheduler);

		WorkerHealthCheck.RegistrarInicioExecucao(execucao.Identificador);

		try {
			logger.LogInformation("Iniciando carga incremental. Execucao ID: {ExecucaoId}", execucao.Identificador);

			await orquestrador.ExecutarImportacaoIncrementalAsync(metricas, cnpjsFiltro: null, stoppingToken);

			await execucoesCarga.FinalizarComSucessoAsync(execucao.Identificador, metricas);

			logger.LogInformation(
				"Carga incremental finalizada. Execucao ID: {ExecucaoId}, Orgaos: {TotalOrgaos}, Compras: {TotalCompras}, Contratos: {TotalContratos}, Atas: {TotalAtas}, Duracao: {Duracao}ms",
				execucao.Identificador,
				metricas.TotalOrgaosProcessados,
				metricas.TotalComprasProcessadas,
				metricas.TotalContratosProcessados,
				metricas.TotalAtasProcessadas,
				metricas.DuracaoTotalMs);

			WorkerHealthCheck.RegistrarFimExecucao(StatusExecucao.Sucesso);
		} catch (OperationCanceledException) {
			logger.LogWarning("Carga incremental cancelada. Execucao ID: {ExecucaoId}", execucao.Identificador);
			await execucoesCarga.FinalizarComCancelamentoAsync(execucao.Identificador, metricas);
			WorkerHealthCheck.RegistrarFimExecucao(StatusExecucao.Cancelado);
		} catch (Exception ex) {
			logger.LogError(ex, "Erro na carga incremental. Execucao ID: {ExecucaoId}", execucao.Identificador);
			await execucoesCarga.FinalizarComErroAsync(execucao.Identificador, ex.Message, ex.StackTrace, metricas);
			WorkerHealthCheck.RegistrarFimExecucao(StatusExecucao.Erro);
		}
	}
}
