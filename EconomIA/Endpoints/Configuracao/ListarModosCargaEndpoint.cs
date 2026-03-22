using System;
using System.Linq;
using EconomIA.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Configuracao;

public static class ListarModosCargaEndpoint {
	public static IEndpointRouteBuilder MapListarModosCarga(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/configuracao/modos-carga", Handle)
			.WithName("ListarModosCarga")
			.WithTags("Configuração");

		return app;
	}

	private static IResult Handle() {
		var modos = ModoExecucaoTipo.ModosCargaAutomatica
			.Select(m => new ModoResponse(m))
			.ToArray();

		return Microsoft.AspNetCore.Http.Results.Ok(modos);
	}

	private record ModoResponse(String Valor);
}
