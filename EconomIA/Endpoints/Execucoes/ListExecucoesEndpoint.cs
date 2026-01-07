using System;
using System.Linq;
using System.Threading.Tasks;
using EconomIA.Application.Queries.ListExecucoes;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Execucoes;

public static class ListExecucoesEndpoint {
	public static IEndpointRouteBuilder MapListExecucoes(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/execucoes", Handle)
			.WithName("ListExecucoes")
			.WithTags("Execuções");

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IMediator mediator,
		[FromQuery] String? status,
		[FromQuery] String? order,
		[FromQuery] String? cursor,
		[FromQuery] Int32? limit) {
		var query = new ListExecucoes.Query(status, order, cursor, limit);
		var result = await mediator.Send(query);

		return result.ToOk(Response.From);
	}

	private record Response(
		Response.Item[] Items,
		Boolean HasMoreItems,
		String? NextCursor) {
		public static Response From(ListExecucoes.Response r) {
			return new Response(
				r.Items.Select(x => new Item(
					x.Id,
					x.ModoExecucao,
					x.TipoGatilho,
					x.InicioEm,
					x.FimEm,
					x.DuracaoTotalMs,
					x.Status,
					x.MensagemErro,
					x.TotalOrgaosProcessados,
					x.TotalOrgaosComErro,
					x.TotalComprasProcessadas,
					x.TotalContratosProcessados,
					x.TotalAtasProcessadas,
					x.TotalItensIndexados,
					x.VersaoAplicacao,
					x.Hostname,
					x.CriadoEm)).ToArray(),
				r.HasMoreItems,
				r.NextCursor);
		}

		public record Item(
			Int64 Id,
			String ModoExecucao,
			String TipoGatilho,
			DateTime? InicioEm,
			DateTime? FimEm,
			Int64? DuracaoTotalMs,
			String Status,
			String? MensagemErro,
			Int32 TotalOrgaosProcessados,
			Int32 TotalOrgaosComErro,
			Int32 TotalComprasProcessadas,
			Int32 TotalContratosProcessados,
			Int32 TotalAtasProcessadas,
			Int32 TotalItensIndexados,
			String? VersaoAplicacao,
			String? Hostname,
			DateTime CriadoEm);
	}
}
