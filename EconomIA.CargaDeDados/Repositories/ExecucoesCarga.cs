using System.Data;
using System.Reflection;
using Dapper;
using EconomIA.CargaDeDados.Models;
using EconomIA.CargaDeDados.Observability;

namespace EconomIA.CargaDeDados.Repositories;

public class ExecucoesCarga {
	private readonly IDbConnection conexao;

	public ExecucoesCarga(IDbConnection conexao) {
		this.conexao = conexao;
	}

	public async Task<ExecucaoCarga> IniciarExecucaoAsync(String modoExecucao, String tipoGatilho) {
		var sql = @"
			INSERT INTO public.execucao_carga (
				modo_execucao,
				tipo_gatilho,
				inicio_em,
				status,
				versao_aplicacao,
				hostname,
				criado_em
			) VALUES (
				@ModoExecucao,
				@TipoGatilho,
				@InicioEm,
				@Status,
				@VersaoAplicacao,
				@Hostname,
				NOW()
			)
			RETURNING identificador;
		";

		var versao = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
		var hostname = Environment.MachineName;

		var identificador = await conexao.ExecuteScalarAsync<Int64>(sql, new {
			ModoExecucao = modoExecucao,
			TipoGatilho = tipoGatilho,
			InicioEm = DateTime.UtcNow,
			Status = StatusExecucao.EmAndamento,
			VersaoAplicacao = versao,
			Hostname = hostname
		});

		return new ExecucaoCarga {
			Identificador = identificador,
			ModoExecucao = modoExecucao,
			TipoGatilho = tipoGatilho,
			InicioEm = DateTime.UtcNow,
			Status = StatusExecucao.EmAndamento
		};
	}

	public async Task FinalizarComSucessoAsync(Int64 identificador, MetricasExecucao metricas) {
		metricas.Finalizar();

		var sql = @"
			UPDATE public.execucao_carga
			SET
				fim_em = @FimEm,
				duracao_total_ms = @DuracaoTotalMs,
				status = @Status,
				total_orgaos_processados = @TotalOrgaosProcessados,
				total_orgaos_com_erro = @TotalOrgaosComErro,
				total_compras_processadas = @TotalComprasProcessadas,
				total_contratos_processados = @TotalContratosProcessados,
				total_atas_processadas = @TotalAtasProcessadas,
				total_itens_indexados = @TotalItensIndexados
			WHERE identificador = @Identificador;
		";

		var status = metricas.TotalOrgaosComErro > 0 ? StatusExecucao.Parcial : StatusExecucao.Sucesso;

		await conexao.ExecuteAsync(sql, new {
			Identificador = identificador,
			FimEm = DateTime.UtcNow,
			DuracaoTotalMs = metricas.DuracaoTotalMs,
			Status = status,
			TotalOrgaosProcessados = metricas.TotalOrgaosProcessados,
			TotalOrgaosComErro = metricas.TotalOrgaosComErro,
			TotalComprasProcessadas = metricas.TotalComprasProcessadas,
			TotalContratosProcessados = metricas.TotalContratosProcessados,
			TotalAtasProcessadas = metricas.TotalAtasProcessadas,
			TotalItensIndexados = metricas.TotalItensIndexados
		});

		await SalvarMetricasPorOrgaoAsync(identificador, metricas);
	}

	public async Task FinalizarComErroAsync(Int64 identificador, String mensagemErro, String? stackTrace, MetricasExecucao? metricas = null) {
		metricas?.Finalizar();

		var sql = @"
			UPDATE public.execucao_carga
			SET
				fim_em = @FimEm,
				duracao_total_ms = @DuracaoTotalMs,
				status = @Status,
				mensagem_erro = @MensagemErro,
				stack_trace = @StackTrace,
				total_orgaos_processados = @TotalOrgaosProcessados,
				total_orgaos_com_erro = @TotalOrgaosComErro,
				total_compras_processadas = @TotalComprasProcessadas,
				total_contratos_processados = @TotalContratosProcessados,
				total_atas_processadas = @TotalAtasProcessadas,
				total_itens_indexados = @TotalItensIndexados
			WHERE identificador = @Identificador;
		";

		await conexao.ExecuteAsync(sql, new {
			Identificador = identificador,
			FimEm = DateTime.UtcNow,
			DuracaoTotalMs = metricas?.DuracaoTotalMs ?? 0,
			Status = StatusExecucao.Erro,
			MensagemErro = mensagemErro,
			StackTrace = stackTrace,
			TotalOrgaosProcessados = metricas?.TotalOrgaosProcessados ?? 0,
			TotalOrgaosComErro = metricas?.TotalOrgaosComErro ?? 0,
			TotalComprasProcessadas = metricas?.TotalComprasProcessadas ?? 0,
			TotalContratosProcessados = metricas?.TotalContratosProcessados ?? 0,
			TotalAtasProcessadas = metricas?.TotalAtasProcessadas ?? 0,
			TotalItensIndexados = metricas?.TotalItensIndexados ?? 0
		});

		if (metricas is not null) {
			await SalvarMetricasPorOrgaoAsync(identificador, metricas);
		}
	}

	public async Task FinalizarComCancelamentoAsync(Int64 identificador, MetricasExecucao? metricas = null) {
		metricas?.Finalizar();

		var sql = @"
			UPDATE public.execucao_carga
			SET
				fim_em = @FimEm,
				duracao_total_ms = @DuracaoTotalMs,
				status = @Status,
				mensagem_erro = 'Operacao cancelada',
				total_orgaos_processados = @TotalOrgaosProcessados,
				total_orgaos_com_erro = @TotalOrgaosComErro,
				total_compras_processadas = @TotalComprasProcessadas,
				total_contratos_processados = @TotalContratosProcessados,
				total_atas_processadas = @TotalAtasProcessadas,
				total_itens_indexados = @TotalItensIndexados
			WHERE identificador = @Identificador;
		";

		await conexao.ExecuteAsync(sql, new {
			Identificador = identificador,
			FimEm = DateTime.UtcNow,
			DuracaoTotalMs = metricas?.DuracaoTotalMs ?? 0,
			Status = StatusExecucao.Cancelado,
			TotalOrgaosProcessados = metricas?.TotalOrgaosProcessados ?? 0,
			TotalOrgaosComErro = metricas?.TotalOrgaosComErro ?? 0,
			TotalComprasProcessadas = metricas?.TotalComprasProcessadas ?? 0,
			TotalContratosProcessados = metricas?.TotalContratosProcessados ?? 0,
			TotalAtasProcessadas = metricas?.TotalAtasProcessadas ?? 0,
			TotalItensIndexados = metricas?.TotalItensIndexados ?? 0
		});

		if (metricas is not null) {
			await SalvarMetricasPorOrgaoAsync(identificador, metricas);
		}
	}

	private async Task SalvarMetricasPorOrgaoAsync(Int64 identificadorDaExecucao, MetricasExecucao metricas) {
		var sql = @"
			INSERT INTO public.execucao_carga_orgao (
				identificador_da_execucao,
				identificador_do_orgao,
				inicio_em,
				fim_em,
				duracao_ms,
				status,
				mensagem_erro,
				compras_processadas,
				contratos_processados,
				atas_processadas,
				itens_processados,
				data_inicial_processada,
				data_final_processada,
				criado_em
			) VALUES (
				@IdentificadorDaExecucao,
				@IdentificadorDoOrgao,
				@InicioEm,
				@FimEm,
				@DuracaoMs,
				@Status,
				@MensagemErro,
				@ComprasProcessadas,
				@ContratosProcessados,
				@AtasProcessadas,
				@ItensProcessados,
				@DataInicialProcessada,
				@DataFinalProcessada,
				NOW()
			);
		";

		foreach (var metricaOrgao in metricas.ObterTodasMetricas()) {
			await conexao.ExecuteAsync(sql, new {
				IdentificadorDaExecucao = identificadorDaExecucao,
				IdentificadorDoOrgao = metricaOrgao.IdentificadorDoOrgao,
				InicioEm = metricaOrgao.InicioEm,
				FimEm = metricaOrgao.FimEm,
				DuracaoMs = metricaOrgao.DuracaoMs,
				Status = metricaOrgao.Status,
				MensagemErro = metricaOrgao.MensagemErro,
				ComprasProcessadas = metricaOrgao.ComprasProcessadas,
				ContratosProcessados = metricaOrgao.ContratosProcessados,
				AtasProcessadas = metricaOrgao.AtasProcessadas,
				ItensProcessados = metricaOrgao.ItensProcessados,
				DataInicialProcessada = metricaOrgao.DataInicialProcessada,
				DataFinalProcessada = metricaOrgao.DataFinalProcessada
			});
		}
	}

	public async Task<ExecucaoCarga?> ObterUltimaExecucaoAsync() {
		var sql = @"
			SELECT
				identificador AS Identificador,
				modo_execucao AS ModoExecucao,
				tipo_gatilho AS TipoGatilho,
				inicio_em AS InicioEm,
				fim_em AS FimEm,
				duracao_total_ms AS DuracaoTotalMs,
				status AS Status,
				mensagem_erro AS MensagemErro,
				total_orgaos_processados AS TotalOrgaosProcessados,
				total_orgaos_com_erro AS TotalOrgaosComErro,
				total_compras_processadas AS TotalComprasProcessadas,
				total_contratos_processados AS TotalContratosProcessados,
				total_atas_processadas AS TotalAtasProcessadas,
				total_itens_indexados AS TotalItensIndexados
			FROM public.execucao_carga
			ORDER BY inicio_em DESC
			LIMIT 1;
		";

		return await conexao.QueryFirstOrDefaultAsync<ExecucaoCarga>(sql);
	}

	public async Task<IEnumerable<ExecucaoCarga>> ListarUltimasExecucoesAsync(Int32 limite = 10) {
		var sql = @"
			SELECT
				identificador AS Identificador,
				modo_execucao AS ModoExecucao,
				tipo_gatilho AS TipoGatilho,
				inicio_em AS InicioEm,
				fim_em AS FimEm,
				duracao_total_ms AS DuracaoTotalMs,
				status AS Status,
				mensagem_erro AS MensagemErro,
				total_orgaos_processados AS TotalOrgaosProcessados,
				total_orgaos_com_erro AS TotalOrgaosComErro,
				total_compras_processadas AS TotalComprasProcessadas,
				total_contratos_processados AS TotalContratosProcessados,
				total_atas_processadas AS TotalAtasProcessadas,
				total_itens_indexados AS TotalItensIndexados
			FROM public.execucao_carga
			ORDER BY inicio_em DESC
			LIMIT @Limite;
		";

		return await conexao.QueryAsync<ExecucaoCarga>(sql, new { Limite = limite });
	}

	public async Task<ExecucaoCarga> CriarPendenteAsync(String modoExecucao, String tipoGatilho, ParametrosExecucao? parametros) {
		var sql = @"
			INSERT INTO public.execucao_carga (
				modo_execucao,
				tipo_gatilho,
				status,
				parametros,
				versao_aplicacao,
				hostname,
				criado_em
			) VALUES (
				@ModoExecucao,
				@TipoGatilho,
				@Status,
				@Parametros::jsonb,
				@VersaoAplicacao,
				@Hostname,
				NOW()
			)
			RETURNING identificador, criado_em;
		";

		var versao = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
		var hostname = Environment.MachineName;

		var result = await conexao.QueryFirstAsync<(Int64 Identificador, DateTime CriadoEm)>(sql, new {
			ModoExecucao = modoExecucao,
			TipoGatilho = tipoGatilho,
			Status = StatusExecucao.Pendente,
			Parametros = parametros?.ToJson(),
			VersaoAplicacao = versao,
			Hostname = hostname
		});

		return new ExecucaoCarga {
			Identificador = result.Identificador,
			ModoExecucao = modoExecucao,
			TipoGatilho = tipoGatilho,
			Status = StatusExecucao.Pendente,
			ParametrosJson = parametros?.ToJson(),
			CriadoEm = result.CriadoEm
		};
	}

	public async Task<ExecucaoCarga?> ObterProximaPendenteAsync() {
		var sql = @"
			SELECT
				identificador AS Identificador,
				modo_execucao AS ModoExecucao,
				tipo_gatilho AS TipoGatilho,
				inicio_em AS InicioEm,
				fim_em AS FimEm,
				duracao_total_ms AS DuracaoTotalMs,
				status AS Status,
				mensagem_erro AS MensagemErro,
				total_orgaos_processados AS TotalOrgaosProcessados,
				total_orgaos_com_erro AS TotalOrgaosComErro,
				total_compras_processadas AS TotalComprasProcessadas,
				total_contratos_processados AS TotalContratosProcessados,
				total_atas_processadas AS TotalAtasProcessadas,
				total_itens_indexados AS TotalItensIndexados,
				parametros AS ParametrosJson,
				criado_em AS CriadoEm
			FROM public.execucao_carga
			WHERE status = @Status
			ORDER BY criado_em ASC
			LIMIT 1;
		";

		return await conexao.QueryFirstOrDefaultAsync<ExecucaoCarga>(sql, new { Status = StatusExecucao.Pendente });
	}

	public async Task IniciarProcessamentoAsync(Int64 identificador) {
		var sql = @"
			UPDATE public.execucao_carga
			SET
				status = @Status,
				inicio_em = NOW()
			WHERE identificador = @Identificador;
		";

		await conexao.ExecuteAsync(sql, new {
			Identificador = identificador,
			Status = StatusExecucao.EmAndamento
		});
	}

	public async Task<Boolean> ExisteExecucaoEmAndamentoAsync() {
		var sql = @"
			SELECT EXISTS (
				SELECT 1 FROM public.execucao_carga
				WHERE status = @Status
			);
		";

		return await conexao.ExecuteScalarAsync<Boolean>(sql, new { Status = StatusExecucao.EmAndamento });
	}

	public async Task<ExecucaoCarga?> ObterExecucaoEmAndamentoAsync() {
		var sql = @"
			SELECT
				identificador AS Identificador,
				modo_execucao AS ModoExecucao,
				tipo_gatilho AS TipoGatilho,
				inicio_em AS InicioEm,
				status AS Status
			FROM public.execucao_carga
			WHERE status = @Status
			ORDER BY inicio_em ASC
			LIMIT 1;
		";

		return await conexao.QueryFirstOrDefaultAsync<ExecucaoCarga>(sql, new { Status = StatusExecucao.EmAndamento });
	}
}
