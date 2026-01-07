using System;
using System.Threading.Tasks;
using EconomIA.Application.Commands.IniciarExecucao;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Execucoes;

public static class IniciarExecucaoEndpoint {
	public static IEndpointRouteBuilder MapIniciarExecucao(this IEndpointRouteBuilder app) {
		app.MapPost("/v1/execucoes", Handle)
			.WithName("IniciarExecucao")
			.WithTags("Execuções");

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IMediator mediator,
		[FromBody] Request request) {
		var command = new IniciarExecucao.Command(
			request.ModoExecucao,
			request.DiasRetroativos,
			request.Cnpjs);

		var result = await mediator.Send(command);

		return result.ToCreated(r => $"/v1/execucoes/{r.Id}", Response.From);
	}

	private record Request(
		String ModoExecucao,
		Int32? DiasRetroativos,
		String[]? Cnpjs);

	private record Response(
		Int64 Id,
		String Status,
		String ModoExecucao,
		String TipoGatilho,
		DateTime CriadoEm) {
		public static Response From(IniciarExecucao.Response r) =>
			new(r.Id, r.Status, r.ModoExecucao, r.TipoGatilho, r.CriadoEm);
	}
}
