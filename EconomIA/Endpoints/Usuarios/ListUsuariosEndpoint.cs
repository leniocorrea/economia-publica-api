using System;
using System.Threading.Tasks;
using EconomIA.Application.Queries.ListUsuarios;
using EconomIA.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Usuarios;

public static class ListUsuariosEndpoint {
	public static IEndpointRouteBuilder MapListUsuarios(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/usuarios", Handle)
			.WithName("ListUsuarios")
			.WithTags("Usuários")
			.RequireAuthorization();

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IMediator mediator,
		[FromQuery] String? search,
		[FromQuery] String? order,
		[FromQuery] String? cursor,
		[FromQuery] Int32? limit) {
		var query = new ListUsuarios.Query(search, order, cursor, limit);
		var result = await mediator.Send(query);

		return result.ToOk(Response.From);
	}

	private record Response(
		Response.Item[] Items,
		Boolean HasMoreItems,
		String? NextCursor) {
		public static Response From(ListUsuarios.Response r) =>
			new(
				Array.ConvertAll(r.Items, Item.From),
				r.HasMoreItems,
				r.NextCursor);

		public record Item(
			Guid Id,
			String Nome,
			String Email,
			String Perfil,
			Boolean Ativo,
			DateTime? UltimoAcesso,
			DateTime CriadoEm) {
			public static Item From(ListUsuarios.Response.Item i) =>
				new(i.Id, i.Nome, i.Email, i.Perfil, i.Ativo, i.UltimoAcesso, i.CriadoEm);
		}
	}
}
