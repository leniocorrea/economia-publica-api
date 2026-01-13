using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EconomIA.Hubs;

[Authorize]
public class NotificacoesHub : Hub {
	private readonly ILogger<NotificacoesHub> logger;

	public NotificacoesHub(ILogger<NotificacoesHub> logger) {
		this.logger = logger;
	}

	public override async Task OnConnectedAsync() {
		var userId = Context.UserIdentifier;
		logger.LogInformation("Cliente conectado ao hub de notificacoes. ConnectionId: {ConnectionId}, UserId: {UserId}",
			Context.ConnectionId, userId);
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception) {
		logger.LogInformation("Cliente desconectado do hub de notificacoes. ConnectionId: {ConnectionId}",
			Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}
}

public static class NotificacoesHubMethods {
	public const String ExecucaoIniciada = "ExecucaoIniciada";
	public const String ExecucaoFinalizada = "ExecucaoFinalizada";
	public const String ExecucaoErro = "ExecucaoErro";
	public const String ExecucaoProgresso = "ExecucaoProgresso";
}
