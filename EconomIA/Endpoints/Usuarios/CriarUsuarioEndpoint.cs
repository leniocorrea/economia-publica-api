using System;
using System.Threading.Tasks;
using EconomIA.Application.Commands.CriarUsuario;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using EconomIA.Results;
using EconomIA.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Usuarios;

public static class CriarUsuarioEndpoint {
	public static IEndpointRouteBuilder MapCriarUsuario(this IEndpointRouteBuilder app) {
		app.MapPost("/v1/usuarios", Handle)
			.WithName("CriarUsuario")
			.WithTags("Usuários")
			.RequireAuthorization();

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IUsuariosReader usuariosReader,
		[FromServices] IUsuarios usuarios,
		[FromServices] IServicoDeAutenticacao servicoDeAutenticacao,
		[FromBody] Request request) {
		var handler = new CriarUsuario.Handler(usuariosReader, usuarios);
		handler.SetCriptografarSenha(servicoDeAutenticacao.CriptografarSenha);

		var command = new CriarUsuario.Command(
			request.Nome,
			request.Email,
			request.Senha,
			request.Perfil ?? Perfis.Administrador);

		var result = await handler.Handle(command);

		return result.ToCreated(r => $"/v1/usuarios/{r.Id}", Response.From);
	}

	private record Request(
		String Nome,
		String Email,
		String Senha,
		String? Perfil);

	private record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		DateTime CriadoEm) {
		public static Response From(CriarUsuario.Response r) =>
			new(r.Id, r.Nome, r.Email, r.Perfil, r.CriadoEm);
	}
}
