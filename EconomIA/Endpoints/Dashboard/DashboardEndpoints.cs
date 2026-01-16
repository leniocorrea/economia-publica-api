using System;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Dashboard;

public static class DashboardEndpoints {
	public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app) {
		app.MapGetEstatisticas();

		return app;
	}
}
