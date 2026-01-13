using System.Net.Http.Json;
using System.Text.Json;
using EconomIA.CargaDeDados.Models;
using EconomIA.CargaDeDados.Observability;

namespace EconomIA.CargaDeDados.Services;

public interface IClienteDeNotificacoes {
	Task NotificarExecucaoIniciadaAsync(ExecucaoCarga execucao);
	Task NotificarExecucaoFinalizadaAsync(ExecucaoCarga execucao, MetricasExecucao metricas);
	Task NotificarExecucaoErroAsync(ExecucaoCarga execucao, MetricasExecucao metricas, String mensagemErro);
}

public class ClienteDeNotificacoes : IClienteDeNotificacoes {
	private readonly HttpClient httpClient;
	private readonly ILogger<ClienteDeNotificacoes> logger;
	private readonly Boolean habilitado;

	public ClienteDeNotificacoes(
		HttpClient httpClient,
		IConfiguration configuration,
		ILogger<ClienteDeNotificacoes> logger) {
		this.httpClient = httpClient;
		this.logger = logger;
		this.habilitado = !String.IsNullOrEmpty(configuration["Api:BaseUrl"]);
	}

	public async Task NotificarExecucaoIniciadaAsync(ExecucaoCarga execucao) {
		await EnviarNotificacaoAsync(new NotificacaoRequest(
			execucao.Identificador,
			"iniciado",
			execucao.ModoExecucao,
			0, 0, 0, 0,
			null, null
		));
	}

	public async Task NotificarExecucaoFinalizadaAsync(ExecucaoCarga execucao, MetricasExecucao metricas) {
		await EnviarNotificacaoAsync(new NotificacaoRequest(
			execucao.Identificador,
			"sucesso",
			execucao.ModoExecucao,
			metricas.TotalOrgaosProcessados,
			metricas.TotalComprasProcessadas,
			metricas.TotalContratosProcessados,
			metricas.TotalAtasProcessadas,
			metricas.DuracaoTotalMs,
			null
		));
	}

	public async Task NotificarExecucaoErroAsync(ExecucaoCarga execucao, MetricasExecucao metricas, String mensagemErro) {
		await EnviarNotificacaoAsync(new NotificacaoRequest(
			execucao.Identificador,
			"erro",
			execucao.ModoExecucao,
			metricas.TotalOrgaosProcessados,
			metricas.TotalComprasProcessadas,
			metricas.TotalContratosProcessados,
			metricas.TotalAtasProcessadas,
			metricas.DuracaoTotalMs,
			mensagemErro
		));
	}

	private async Task EnviarNotificacaoAsync(NotificacaoRequest request) {
		if (!habilitado) {
			logger.LogDebug("Notificacoes desabilitadas. Ignorando envio.");
			return;
		}

		try {
			var response = await httpClient.PostAsJsonAsync("/v1/notificacoes/execucao", request, new JsonSerializerOptions {
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			});

			if (response.IsSuccessStatusCode) {
				logger.LogDebug("Notificacao enviada com sucesso. ExecucaoId: {ExecucaoId}, Status: {Status}",
					request.ExecucaoId, request.Status);
			} else {
				logger.LogWarning("Falha ao enviar notificacao. StatusCode: {StatusCode}, ExecucaoId: {ExecucaoId}",
					response.StatusCode, request.ExecucaoId);
			}
		} catch (Exception ex) {
			logger.LogWarning(ex, "Erro ao enviar notificacao. ExecucaoId: {ExecucaoId}", request.ExecucaoId);
		}
	}

	private record NotificacaoRequest(
		Int64 ExecucaoId,
		String Status,
		String ModoExecucao,
		Int32 TotalOrgaosProcessados,
		Int32 TotalComprasProcessadas,
		Int32 TotalContratosProcessados,
		Int32 TotalAtasProcessadas,
		Int64? DuracaoMs,
		String? MensagemErro
	);
}

public class ClienteDeNotificacoesDesabilitado : IClienteDeNotificacoes {
	public Task NotificarExecucaoIniciadaAsync(ExecucaoCarga execucao) => Task.CompletedTask;
	public Task NotificarExecucaoFinalizadaAsync(ExecucaoCarga execucao, MetricasExecucao metricas) => Task.CompletedTask;
	public Task NotificarExecucaoErroAsync(ExecucaoCarga execucao, MetricasExecucao metricas, String mensagemErro) => Task.CompletedTask;
}
