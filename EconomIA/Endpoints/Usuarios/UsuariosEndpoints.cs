using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Usuarios;

public static class UsuariosEndpoints {
	public static IEndpointRouteBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder app) {
		app.MapCriarUsuario();
		app.MapListUsuarios();
		app.MapGetUsuario();
		app.MapAtualizarUsuario();

		return app;
	}
}
