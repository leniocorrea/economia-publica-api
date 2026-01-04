using System;
using System.Linq;
using System.Threading.Tasks;
using EconomIA.Application.Queries.GetExecucao;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Execucoes;

public static class GetExecucaoEndpoint {
	public static IEndpointRouteBuilder MapGetExecucao(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/execucoes/{id:long}", Handle)
			.WithName("GetExecucao")
			.WithTags("Execuções");

		return app;
	}

	private static async Task<IResult> Handle([FromServices] IMediator mediator, [FromRoute] Int64 id) {
		var result = await mediator.Send(new GetExecucao.Query(id));

		return result.ToOk(Response.From);
	}

	private record Response(
		Int64 Id,
		String ModoExecucao,
		String TipoGatilho,
		DateTime InicioEm,
		DateTime? FimEm,
		Int64? DuracaoTotalMs,
		String Status,
		String? MensagemErro,
		String? StackTrace,
		Int32 TotalOrgaosProcessados,
		Int32 TotalOrgaosComErro,
		Int32 TotalComprasProcessadas,
		Int32 TotalContratosProcessados,
		Int32 TotalAtasProcessadas,
		Int32 TotalItensIndexados,
		String? VersaoAplicacao,
		String? Hostname,
		DateTime CriadoEm,
		Response.OrgaoItem[] Orgaos) {
		public static Response From(GetExecucao.Response r) {
			return new Response(
				r.Id,
				r.ModoExecucao,
				r.TipoGatilho,
				r.InicioEm,
				r.FimEm,
				r.DuracaoTotalMs,
				r.Status,
				r.MensagemErro,
				r.StackTrace,
				r.TotalOrgaosProcessados,
				r.TotalOrgaosComErro,
				r.TotalComprasProcessadas,
				r.TotalContratosProcessados,
				r.TotalAtasProcessadas,
				r.TotalItensIndexados,
				r.VersaoAplicacao,
				r.Hostname,
				r.CriadoEm,
				r.Orgaos.Select(o => new OrgaoItem(
					o.Id,
					o.IdentificadorDoOrgao,
					o.Cnpj,
					o.RazaoSocial,
					o.InicioEm,
					o.FimEm,
					o.DuracaoMs,
					o.Status,
					o.MensagemErro,
					o.ComprasProcessadas,
					o.ComprasDuracaoMs,
					o.ContratosProcessados,
					o.ContratosDuracaoMs,
					o.AtasProcessadas,
					o.AtasDuracaoMs,
					o.ItensProcessados,
					o.DataInicialProcessada,
					o.DataFinalProcessada,
					o.CriadoEm)).ToArray()
			);
		}

		public record OrgaoItem(
			Int64 Id,
			Int64 IdentificadorDoOrgao,
			String Cnpj,
			String RazaoSocial,
			DateTime InicioEm,
			DateTime? FimEm,
			Int64? DuracaoMs,
			String Status,
			String? MensagemErro,
			Int32 ComprasProcessadas,
			Int64 ComprasDuracaoMs,
			Int32 ContratosProcessados,
			Int64 ContratosDuracaoMs,
			Int32 AtasProcessadas,
			Int64 AtasDuracaoMs,
			Int32 ItensProcessados,
			DateTime? DataInicialProcessada,
			DateTime? DataFinalProcessada,
			DateTime CriadoEm);
	}
}
