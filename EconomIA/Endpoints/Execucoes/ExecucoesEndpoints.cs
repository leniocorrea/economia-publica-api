using System;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Execucoes;

public static class ExecucoesEndpoints {
	public static IEndpointRouteBuilder MapExecucoesEndpoints(this IEndpointRouteBuilder app) {
		app.MapListExecucoes();
		app.MapGetExecucao();
		app.MapIniciarExecucao();

		return app;
	}
}
