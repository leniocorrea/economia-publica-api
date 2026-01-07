using System;
using System.Threading.Tasks;
using EconomIA.Application.Queries.GetUsuario;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Usuarios;

public static class GetUsuarioEndpoint {
	public static IEndpointRouteBuilder MapGetUsuario(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/usuarios/{id:guid}", Handle)
			.WithName("GetUsuario")
			.WithTags("Usuários")
			.RequireAuthorization();

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IMediator mediator,
		[FromRoute] Guid id) {
		var query = new GetUsuario.Query(id);
		var result = await mediator.Send(query);

		return result.ToOk(Response.From);
	}

	private record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		Boolean Ativo,
		DateTime? UltimoAcesso,
		DateTime CriadoEm) {
		public static Response From(GetUsuario.Response r) =>
			new(r.Id, r.Nome, r.Email, r.Perfil, r.Ativo, r.UltimoAcesso, r.CriadoEm);
	}
}
