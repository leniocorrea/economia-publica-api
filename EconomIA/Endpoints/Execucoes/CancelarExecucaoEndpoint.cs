using System;
using System.Threading.Tasks;
using EconomIA.Application.Commands.CancelarExecucao;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Execucoes;

public static class CancelarExecucaoEndpoint {
	public static IEndpointRouteBuilder MapCancelarExecucao(this IEndpointRouteBuilder app) {
		app.MapPost("/v1/execucoes/{id:long}/cancelar", Handle)
			.WithName("CancelarExecucao")
			.WithTags("Execuções");

		return app;
	}

	private static async Task<IResult> Handle([FromServices] IMediator mediator, [FromRoute] Int64 id) {
		var command = new CancelarExecucao.Command(id);
		var result = await mediator.Send(command);

		return result.ToNoContent();
	}
}
