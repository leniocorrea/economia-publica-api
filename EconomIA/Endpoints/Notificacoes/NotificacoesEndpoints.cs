using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Notificacoes;

public static class NotificacoesEndpoints {
	public static IEndpointRouteBuilder MapNotificacoesEndpoints(this IEndpointRouteBuilder app) {
		app.MapEnviarNotificacao();
		return app;
	}
}
