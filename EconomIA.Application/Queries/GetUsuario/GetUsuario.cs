using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Application.Extensions;
using EconomIA.Common.Results;
using EconomIA.Domain.Repositories;

namespace EconomIA.Application.Queries.GetUsuario;

public static class GetUsuario {
	public record Query(Guid IdentificadorExterno) : IQuery<Response>;

	public record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		Boolean Ativo,
		DateTime? UltimoAcesso,
		DateTime CriadoEm);

	public class Handler(IUsuariosReader usuariosReader) : QueryHandler<Query, Response> {
		public override async Task<Result<Response, HandlerResultError>> Handle(Query query, CancellationToken cancellationToken = default) {
			var result = await usuariosReader.Find(UsuariosSpecifications.WithIdentificadorExterno(query.IdentificadorExterno), cancellationToken);

			if (result.IsFailure) {
				return Failure(result.Error.ToUsuarioError());
			}

			var usuario = result.Value;

			return Success(new Response(
				usuario.IdentificadorExterno,
				usuario.Nome,
				usuario.Email,
				usuario.Perfil,
				usuario.Ativo,
				usuario.UltimoAcesso,
				usuario.CriadoEm));
		}
	}
}
