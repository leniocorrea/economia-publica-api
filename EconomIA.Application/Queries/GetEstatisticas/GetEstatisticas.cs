using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Common.Results;
using EconomIA.Domain.Repositories;

namespace EconomIA.Application.Queries.GetEstatisticas;

public static class GetEstatisticas {
	public record Query : IQuery<Response>;

	public record Response(
		Int64 TotalCompras,
		Int64 TotalContratos,
		Int64 TotalAtas,
		Int64 TotalItens,
		Int64 TotalOrgaosMonitorados);

	public class Handler(IEstatisticas estatisticas) : QueryHandler<Query, Response> {
		public override async Task<Result<Response, HandlerResultError>> Handle(Query query, CancellationToken cancellationToken = default) {
			var stats = await estatisticas.ObterEstatisticasGeraisAsync(cancellationToken);

			var response = new Response(
				TotalCompras: stats.TotalCompras,
				TotalContratos: stats.TotalContratos,
				TotalAtas: stats.TotalAtas,
				TotalItens: stats.TotalItens,
				TotalOrgaosMonitorados: stats.TotalOrgaosMonitorados);

			return Success(response);
		}
	}
}
