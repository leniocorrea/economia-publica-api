using System;
using System.Threading.Tasks;
using EconomIA.Application.Commands.MonitorarOrgao;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.OrgaosMonitorados;

public static class MonitorarOrgaoEndpoint {
	public static IEndpointRouteBuilder MapMonitorarOrgao(this IEndpointRouteBuilder app) {
		app.MapPost("/v1/orgaos-monitorados", Handle)
			.WithName("MonitorarOrgao")
			.WithTags("Órgãos Monitorados");

		return app;
	}

	private record Request(String Cnpj);

	private static async Task<IResult> Handle([FromServices] IMediator mediator, [FromBody] Request request) {
		var command = new MonitorarOrgao.Command(request.Cnpj);
		var result = await mediator.Send(command);

		return result.ToCreated($"/v1/orgaos-monitorados/{request.Cnpj}");
	}
}
