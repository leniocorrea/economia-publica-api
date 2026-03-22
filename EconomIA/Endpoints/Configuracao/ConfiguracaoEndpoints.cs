using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Configuracao;

public static class ConfiguracaoEndpoints {
	public static IEndpointRouteBuilder MapConfiguracaoEndpoints(this IEndpointRouteBuilder app) {
		app.MapGetConfiguracao();
		app.MapAtualizarConfiguracao();
		app.MapListarModosCarga();

		return app;
	}
}
