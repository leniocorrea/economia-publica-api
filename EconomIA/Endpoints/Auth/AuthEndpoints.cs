using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Auth;

public static class AuthEndpoints {
	public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app) {
		app.MapLogin();
		app.MapMeEndpoint();

		return app;
	}
}
