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

namespace EconomIA.Application.Queries.ListExecucoes;

public static class ListExecucoes {
	public record Query(String? Status, String? Order, String? Cursor, Int32? Limit) : IQuery<Response>;

	public record Response(Response.Item[] Items, Boolean HasMoreItems, String? NextCursor) {
		public record Item(
			Int64 Id,
			String ModoExecucao,
			String TipoGatilho,
			DateTime InicioEm,
			DateTime? FimEm,
			Int64? DuracaoTotalMs,
			String Status,
			String? MensagemErro,
			Int32 TotalOrgaosProcessados,
			Int32 TotalOrgaosComErro,
			Int32 TotalComprasProcessadas,
			Int32 TotalContratosProcessados,
			Int32 TotalAtasProcessadas,
			Int32 TotalItensIndexados,
			String? VersaoAplicacao,
			String? Hostname,
			DateTime CriadoEm);
	}

	public class Handler(IExecucoesCargaReader execucoes) : QueryHandler<Query, Response> {
		public override async Task<Result<Response, HandlerResultError>> Handle(Query query, CancellationToken cancellationToken = default) {
			var paginationResult = PaginationParameters.Create(query.Order ?? "-id", query.Cursor, query.Limit ?? 20);

			if (paginationResult.IsFailure) {
				return Failure(InvalidArgument, paginationResult.Error);
			}

			var pagination = paginationResult.Value;
			var filter = ExecucoesCargaSpecifications.All();

			if (!String.IsNullOrEmpty(query.Status)) {
				filter = ExecucoesCargaSpecifications.ComStatus(query.Status);
			}

			var result = await execucoes.Paginate(pagination, filter, cancellationToken);

			if (result.IsFailure) {
				return Failure(result.Error.ToExecucaoCargaError());
			}

			var page = result.Value;
			var items = page.Items.Select(x => new Response.Item(
				x.Id,
				x.ModoExecucao,
				x.TipoGatilho,
				x.InicioEm,
				x.FimEm,
				x.DuracaoTotalMs,
				x.Status,
				x.MensagemErro,
				x.TotalOrgaosProcessados,
				x.TotalOrgaosComErro,
				x.TotalComprasProcessadas,
				x.TotalContratosProcessados,
				x.TotalAtasProcessadas,
				x.TotalItensIndexados,
				x.VersaoAplicacao,
				x.Hostname,
				x.CriadoEm
			)).ToArray();

			return Success(new Response(items, page.HasMoreItems, page.NextCursor));
		}
	}
}
