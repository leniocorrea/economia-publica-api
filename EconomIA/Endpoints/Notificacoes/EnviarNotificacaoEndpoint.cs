using System;
using System.Threading.Tasks;
using EconomIA.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace EconomIA.Endpoints.Notificacoes;

public static class EnviarNotificacaoEndpoint {
	public static IEndpointRouteBuilder MapEnviarNotificacao(this IEndpointRouteBuilder app) {
		app.MapPost("/v1/notificacoes/execucao", Handle)
			.WithName("EnviarNotificacaoExecucao")
			.WithTags("Notificações")
			.AllowAnonymous();

		return app;
	}

	private static async Task<IResult> Handle(
		[FromServices] IServicoDeNotificacoes servicoDeNotificacoes,
		[FromBody] Request request) {
		var notificacao = new NotificacaoExecucao(
			request.ExecucaoId,
			request.Status,
			request.ModoExecucao,
			request.TotalOrgaosProcessados,
			request.TotalComprasProcessadas,
			request.TotalContratosProcessados,
			request.TotalAtasProcessadas,
			request.DuracaoMs,
			request.MensagemErro,
			DateTime.UtcNow
		);

		switch (request.Status.ToLowerInvariant()) {
			case "iniciado":
			case "processando":
				await servicoDeNotificacoes.NotificarExecucaoIniciadaAsync(notificacao);
				break;
			case "sucesso":
			case "finalizado":
				await servicoDeNotificacoes.NotificarExecucaoFinalizadaAsync(notificacao);
				break;
			case "erro":
			case "falha":
				await servicoDeNotificacoes.NotificarExecucaoErroAsync(notificacao);
				break;
			default:
				await servicoDeNotificacoes.NotificarExecucaoFinalizadaAsync(notificacao);
				break;
		}

		return TypedResults.Ok(new { sucesso = true });
	}

	private record Request(
		Int64 ExecucaoId,
		String Status,
		String ModoExecucao,
		Int32 TotalOrgaosProcessados,
		Int32 TotalComprasProcessadas,
		Int32 TotalContratosProcessados,
		Int32 TotalAtasProcessadas,
		Int64? DuracaoMs,
		String? MensagemErro
	);
}
