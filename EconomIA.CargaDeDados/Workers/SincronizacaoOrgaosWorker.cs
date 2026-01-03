using Cronos;
using EconomIA.CargaDeDados.Configuration;
using EconomIA.CargaDeDados.Models;
using EconomIA.CargaDeDados.Observability;
using EconomIA.CargaDeDados.Repositories;
using EconomIA.CargaDeDados.Services;
using Microsoft.Extensions.Options;

namespace EconomIA.CargaDeDados.Workers;

public class SincronizacaoOrgaosWorker : BackgroundService {
	private readonly IServiceScopeFactory scopeFactory;
	private readonly ILogger<SincronizacaoOrgaosWorker> logger;
	private readonly WorkerConfiguration configuracao;
	private readonly CronExpression cronExpression;

	public SincronizacaoOrgaosWorker(
		IServiceScopeFactory scopeFactory,
		ILogger<SincronizacaoOrgaosWorker> logger,
		IOptions<WorkerConfiguration> configuracao) {
		this.scopeFactory = scopeFactory;
		this.logger = logger;
		this.configuracao = configuracao.Value;
		this.cronExpression = CronExpression.Parse(this.configuracao.CronSincronizacaoOrgaos);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		if (!configuracao.SincronizarOrgaos) {
			logger.LogInformation("Sincronizacao de orgaos desabilitada");
			return;
		}

		logger.LogInformation("Worker de sincronizacao de orgaos iniciado. Cron: {CronExpression}", configuracao.CronSincronizacaoOrgaos);

		while (!stoppingToken.IsCancellationRequested) {
			var proximaExecucao = cronExpression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Local);

			if (proximaExecucao is null) {
				logger.LogWarning("Nao foi possivel calcular proxima sincronizacao. Tentando novamente em 1 hora");
				await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
				continue;
			}

			var delay = proximaExecucao.Value - DateTimeOffset.UtcNow;

			if (delay > TimeSpan.Zero) {
				logger.LogInformation("Proxima sincronizacao de orgaos agendada para {ProximaExecucao:dd/MM/yyyy HH:mm:ss}", proximaExecucao.Value.LocalDateTime);

				try {
					await Task.Delay(delay, stoppingToken);
				} catch (OperationCanceledException) {
					break;
				}
			}

			if (!stoppingToken.IsCancellationRequested) {
				await SincronizarOrgaosAsync(stoppingToken);
			}
		}

		logger.LogInformation("Worker de sincronizacao de orgaos finalizado");
	}

	private async Task SincronizarOrgaosAsync(CancellationToken stoppingToken) {
		using var scope = scopeFactory.CreateScope();
		var servicos = scope.ServiceProvider;

		var execucoesCarga = servicos.GetRequiredService<ExecucoesCarga>();
		var servicoCargaOrgaos = servicos.GetRequiredService<ServicoCargaOrgaos>();

		var metricas = new MetricasExecucao();
		var execucao = await execucoesCarga.IniciarExecucaoAsync(ModoExecucao.Orgaos, TipoGatilho.Scheduler);

		try {
			logger.LogInformation("Iniciando sincronizacao de orgaos e unidades. Execucao ID: {ExecucaoId}", execucao.Identificador);

			await servicoCargaOrgaos.CarregarTodosOrgaosEUnidadesAsync();

			await execucoesCarga.FinalizarComSucessoAsync(execucao.Identificador, metricas);

			logger.LogInformation("Sincronizacao de orgaos e unidades finalizada. Execucao ID: {ExecucaoId}, Duracao: {Duracao}ms",
				execucao.Identificador,
				metricas.DuracaoTotalMs);
		} catch (OperationCanceledException) {
			logger.LogWarning("Sincronizacao de orgaos cancelada. Execucao ID: {ExecucaoId}", execucao.Identificador);
			await execucoesCarga.FinalizarComCancelamentoAsync(execucao.Identificador, metricas);
		} catch (Exception ex) {
			logger.LogError(ex, "Erro na sincronizacao de orgaos. Execucao ID: {ExecucaoId}", execucao.Identificador);
			await execucoesCarga.FinalizarComErroAsync(execucao.Identificador, ex.Message, ex.StackTrace, metricas);
		}
	}
}
