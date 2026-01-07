using System;
using System.Threading.Tasks;
using EconomIA.Application.Commands.Login;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using EconomIA.Results;
using EconomIA.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Auth;

public static class LoginEndpoint {
	public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder app) {
		app.MapPost("/v1/auth/login", Handle)
			.WithName("Login")
			.WithTags("Autenticação")
			.AllowAnonymous();

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IMediator mediator,
		[FromServices] IServicoDeAutenticacao servicoDeAutenticacao,
		[FromServices] IUsuariosReader usuariosReader,
		[FromBody] Request request) {
		var handler = (Login.Handler)mediator.GetType()
			.GetMethod("CreateHandler")
			?.MakeGenericMethod(typeof(Login.Command), typeof(Login.Response))
			.Invoke(mediator, null)!;

		var command = new Login.Command(request.Email, request.Senha);
		var loginHandler = new Login.Handler(usuariosReader);
		loginHandler.SetValidarSenha(servicoDeAutenticacao.ValidarSenha);

		var result = await loginHandler.Handle(command);

		if (result.IsFailure) {
			return result.ToErrorResponse();
		}

		var usuarioResult = await usuariosReader.Find(UsuariosSpecifications.WithIdentificadorExterno(result.Value.UsuarioId));
		var token = servicoDeAutenticacao.GerarToken(usuarioResult.Value);

		return TypedResults.Ok(Response.From(result.Value, token));
	}

	private record Request(String Email, String Senha);

	private record Response(
		Guid UsuarioId,
		String Nome,
		String Email,
		String Perfil,
		String Token) {
		public static Response From(Login.Response r, String token) =>
			new(r.UsuarioId, r.Nome, r.Email, r.Perfil, token);
	}
}
