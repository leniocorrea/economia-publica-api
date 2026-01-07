using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Application.Extensions;
using EconomIA.Common.Persistence.Pagination;
using EconomIA.Common.Results;
using EconomIA.Domain.Repositories;
using static EconomIA.Domain.Results.EconomIAErrorCodes;

namespace EconomIA.Application.Queries.ListUsuarios;

public static class ListUsuarios {
	public record Query(String? Search, String? Order, String? Cursor, Int32? Limit) : IQuery<Response>;

	public record Response(Response.Item[] Items, Boolean HasMoreItems, String? NextCursor) {
		public record Item(
			Guid Id,
			String Nome,
			String Email,
			String Perfil,
			Boolean Ativo,
			DateTime? UltimoAcesso,
			DateTime CriadoEm);
	}

	public class Handler(IUsuariosReader usuariosReader) : QueryHandler<Query, Response> {
		public override async Task<Result<Response, HandlerResultError>> Handle(Query query, CancellationToken cancellationToken = default) {
			var paginationResult = PaginationParameters.Create(query.Order, query.Cursor, query.Limit);

			if (paginationResult.IsFailure) {
				return Failure(InvalidArgument, paginationResult.Error);
			}

			var pagination = paginationResult.Value;
			var filter = UsuariosSpecifications.All();

			var result = await usuariosReader.Paginate(pagination, filter, cancellationToken);

			if (result.IsFailure) {
				return Failure(result.Error.ToUsuarioError());
			}

			var page = result.Value;
			var items = page.Items.Select(x => new Response.Item(
				x.IdentificadorExterno,
				x.Nome,
				x.Email,
				x.Perfil,
				x.Ativo,
				x.UltimoAcesso,
				x.CriadoEm
			)).ToArray();

			return Success(new Response(items, page.HasMoreItems, page.NextCursor));
		}
	}
}
