using System;
using System.Threading.Tasks;
using EconomIA.Application.Queries.GetEstatisticas;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Dashboard;

public static class GetEstatisticasEndpoint {
	public static IEndpointRouteBuilder MapGetEstatisticas(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/dashboard/estatisticas", Handle)
			.WithName("GetEstatisticas")
			.WithTags("Dashboard");

		return app;
	}

	private static async Task<IResult> Handle([FromServices] IMediator mediator) {
		var query = new GetEstatisticas.Query();
		var result = await mediator.Send(query);

		return result.ToOk(Response.From);
	}

	private record Response(
		Int64 TotalCompras,
		Int64 TotalContratos,
		Int64 TotalAtas,
		Int64 TotalItens,
		Int64 TotalOrgaosMonitorados) {
		public static Response From(GetEstatisticas.Response r) {
			return new Response(
				r.TotalCompras,
				r.TotalContratos,
				r.TotalAtas,
				r.TotalItens,
				r.TotalOrgaosMonitorados);
		}
	}
}
