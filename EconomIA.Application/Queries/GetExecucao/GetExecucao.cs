using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Application.Extensions;
using EconomIA.Common.Results;
using EconomIA.Domain.Repositories;

namespace EconomIA.Application.Queries.GetExecucao;

public static class GetExecucao {
	public record Query(Int64 Id) : IQuery<Response>;

	public record Response(
		Int64 Id,
		String ModoExecucao,
		String TipoGatilho,
		DateTime? InicioEm,
		DateTime? FimEm,
		Int64? DuracaoTotalMs,
		String Status,
		String? MensagemErro,
		String? StackTrace,
		Int32 TotalOrgaosProcessados,
		Int32 TotalOrgaosComErro,
		Int32 TotalComprasProcessadas,
		Int32 TotalContratosProcessados,
		Int32 TotalAtasProcessadas,
		Int32 TotalItensIndexados,
		String? VersaoAplicacao,
		String? Hostname,
		DateTime CriadoEm,
		Response.OrgaoItem[] Orgaos) {
		public record OrgaoItem(
			Int64 Id,
			Int64 IdentificadorDoOrgao,
			String Cnpj,
			String RazaoSocial,
			DateTime InicioEm,
			DateTime? FimEm,
			Int64? DuracaoMs,
			String Status,
			String? MensagemErro,
			Int32 ComprasProcessadas,
			Int64 ComprasDuracaoMs,
			Int32 ContratosProcessados,
			Int64 ContratosDuracaoMs,
			Int32 AtasProcessadas,
			Int64 AtasDuracaoMs,
			Int32 ItensProcessados,
			DateTime? DataInicialProcessada,
			DateTime? DataFinalProcessada,
			DateTime CriadoEm);
	}

	public class Handler(IExecucoesCargaReader execucoes) : QueryHandler<Query, Response> {
		public override async Task<Result<Response, HandlerResultError>> Handle(Query query, CancellationToken cancellationToken = default) {
			var result = await execucoes.Retrieve(query.Id, cancellationToken);

			if (result.IsFailure) {
				return Failure(result.Error.ToExecucaoCargaError());
			}

			var execucao = result.Value;
			var response = new Response(
				execucao.Id,
				execucao.ModoExecucao,
				execucao.TipoGatilho,
				execucao.InicioEm,
				execucao.FimEm,
				execucao.DuracaoTotalMs,
				execucao.Status,
				execucao.MensagemErro,
				execucao.StackTrace,
				execucao.TotalOrgaosProcessados,
				execucao.TotalOrgaosComErro,
				execucao.TotalComprasProcessadas,
				execucao.TotalContratosProcessados,
				execucao.TotalAtasProcessadas,
				execucao.TotalItensIndexados,
				execucao.VersaoAplicacao,
				execucao.Hostname,
				execucao.CriadoEm,
				execucao.Orgaos.Select(o => new Response.OrgaoItem(
					o.Id,
					o.IdentificadorDoOrgao,
					o.Orgao?.Cnpj ?? "",
					o.Orgao?.RazaoSocial ?? "",
					o.InicioEm,
					o.FimEm,
					o.DuracaoMs,
					o.Status,
					o.MensagemErro,
					o.ComprasProcessadas,
					o.ComprasDuracaoMs,
					o.ContratosProcessados,
					o.ContratosDuracaoMs,
					o.AtasProcessadas,
					o.AtasDuracaoMs,
					o.ItensProcessados,
					o.DataInicialProcessada,
					o.DataFinalProcessada,
					o.CriadoEm
				)).ToArray()
			);

			return Success(response);
		}
	}
}
