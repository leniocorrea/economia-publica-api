using System;
using System.Threading.Tasks;
using EconomIA.Application.Commands.AtualizarUsuario;
using EconomIA.Domain.Repositories;
using EconomIA.Results;
using EconomIA.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Usuarios;

public static class AtualizarUsuarioEndpoint {
	public static IEndpointRouteBuilder MapAtualizarUsuario(this IEndpointRouteBuilder app) {
		app.MapPut("/v1/usuarios/{id:guid}", Handle)
			.WithName("AtualizarUsuario")
			.WithTags("Usuários")
			.RequireAuthorization();

		return app;
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid id,
		[FromServices] IUsuariosReader usuariosReader,
		[FromServices] IUsuarios usuarios,
		[FromServices] IServicoDeAutenticacao servicoDeAutenticacao,
		[FromBody] Request request) {
		var handler = new AtualizarUsuario.Handler(usuariosReader, usuarios);
		handler.SetCriptografarSenha(servicoDeAutenticacao.CriptografarSenha);

		var command = new AtualizarUsuario.Command(
			id,
			request.Nome,
			request.Email,
			request.Senha,
			request.Perfil,
			request.Ativo);

		var result = await handler.Handle(command);

		return result.ToOk(Response.From);
	}

	private record Request(
		String? Nome,
		String? Email,
		String? Senha,
		String? Perfil,
		Boolean? Ativo);

	private record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		Boolean Ativo,
		DateTime AtualizadoEm) {
		public static Response From(AtualizarUsuario.Response r) =>
			new(r.Id, r.Nome, r.Email, r.Perfil, r.Ativo, r.AtualizadoEm);
	}
}
