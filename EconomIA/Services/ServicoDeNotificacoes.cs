using System;
using System.Threading.Tasks;
using EconomIA.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EconomIA.Services;

public interface IServicoDeNotificacoes {
	Task NotificarExecucaoIniciadaAsync(NotificacaoExecucao notificacao);
	Task NotificarExecucaoFinalizadaAsync(NotificacaoExecucao notificacao);
	Task NotificarExecucaoErroAsync(NotificacaoExecucao notificacao);
	Task NotificarExecucaoProgressoAsync(NotificacaoProgresso notificacao);
}

public class ServicoDeNotificacoes : IServicoDeNotificacoes {
	private readonly IHubContext<NotificacoesHub> hubContext;
	private readonly ILogger<ServicoDeNotificacoes> logger;

	public ServicoDeNotificacoes(
		IHubContext<NotificacoesHub> hubContext,
		ILogger<ServicoDeNotificacoes> logger) {
		this.hubContext = hubContext;
		this.logger = logger;
	}

	public async Task NotificarExecucaoIniciadaAsync(NotificacaoExecucao notificacao) {
		logger.LogInformation("Enviando notificacao de execucao iniciada. ExecucaoId: {ExecucaoId}", notificacao.ExecucaoId);
		await hubContext.Clients.All.SendAsync(NotificacoesHubMethods.ExecucaoIniciada, notificacao);
	}

	public async Task NotificarExecucaoFinalizadaAsync(NotificacaoExecucao notificacao) {
		logger.LogInformation("Enviando notificacao de execucao finalizada. ExecucaoId: {ExecucaoId}", notificacao.ExecucaoId);
		await hubContext.Clients.All.SendAsync(NotificacoesHubMethods.ExecucaoFinalizada, notificacao);
	}

	public async Task NotificarExecucaoErroAsync(NotificacaoExecucao notificacao) {
		logger.LogInformation("Enviando notificacao de execucao com erro. ExecucaoId: {ExecucaoId}", notificacao.ExecucaoId);
		await hubContext.Clients.All.SendAsync(NotificacoesHubMethods.ExecucaoErro, notificacao);
	}

	public async Task NotificarExecucaoProgressoAsync(NotificacaoProgresso notificacao) {
		await hubContext.Clients.All.SendAsync(NotificacoesHubMethods.ExecucaoProgresso, notificacao);
	}
}

public record NotificacaoExecucao(
	Int64 ExecucaoId,
	String Status,
	String ModoExecucao,
	Int32 TotalOrgaosProcessados,
	Int32 TotalComprasProcessadas,
	Int32 TotalContratosProcessados,
	Int32 TotalAtasProcessadas,
	Int64? DuracaoMs,
	String? MensagemErro,
	DateTime Timestamp
);

public record NotificacaoProgresso(
	Int64 ExecucaoId,
	String OrgaoAtual,
	Int32 OrgaosProcessados,
	Int32 TotalOrgaos,
	Int32 PercentualConcluido,
	DateTime Timestamp
);
