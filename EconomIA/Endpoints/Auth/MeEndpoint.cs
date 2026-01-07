using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EconomIA.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Auth;

public static class MeEndpoint {
	public static IEndpointRouteBuilder MapMeEndpoint(this IEndpointRouteBuilder app) {
		app.MapGet("/v1/auth/me", Handle)
			.WithName("Me")
			.WithTags("Autenticação")
			.RequireAuthorization();

		return app;
	}

	private static async Task<IResult> Handle(
		HttpContext httpContext,
		[FromServices] IUsuariosReader usuariosReader) {
		var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (String.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) {
			return TypedResults.Unauthorized();
		}

		var usuarioResult = await usuariosReader.Find(UsuariosSpecifications.WithIdentificadorExterno(userId));

		if (usuarioResult.IsFailure) {
			return TypedResults.Unauthorized();
		}

		var usuario = usuarioResult.Value;

		return TypedResults.Ok(new Response(
			usuario.IdentificadorExterno,
			usuario.Nome,
			usuario.Email,
			usuario.Perfil,
			usuario.UltimoAcesso));
	}

	private record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		DateTime? UltimoAcesso);
}
