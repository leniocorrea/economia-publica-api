using System;
using System.Threading;
using System.Threading.Tasks;
using EconomIA.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EconomIA.Adapters.Persistence.Repositories.Estatisticas;

public class EstatisticasQueryRepository(IDbContextFactory<EconomIAQueryDbContext> contextFactory) : IEstatisticas {
	public async Task<EstatisticasGerais> ObterEstatisticasGeraisAsync(CancellationToken cancellationToken = default) {
		await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
		var conexao = context.Database.GetDbConnection();
		await conexao.OpenAsync(cancellationToken);

		var sql = @"
			SELECT
				(SELECT reltuples::bigint FROM pg_class WHERE relname = 'compra') AS total_compras,
				(SELECT reltuples::bigint FROM pg_class WHERE relname = 'contrato') AS total_contratos,
				(SELECT reltuples::bigint FROM pg_class WHERE relname = 'ata') AS total_atas,
				(SELECT reltuples::bigint FROM pg_class WHERE relname = 'item_da_compra') AS total_itens,
				(SELECT COUNT(*) FROM orgao_monitorado WHERE ativo = true) AS total_orgaos_monitorados
		";

		await using var comando = conexao.CreateCommand();
		comando.CommandText = sql;

		await using var leitor = await comando.ExecuteReaderAsync(cancellationToken);
		await leitor.ReadAsync(cancellationToken);

		return new EstatisticasGerais(
			TotalCompras: leitor.GetInt64(0),
			TotalContratos: leitor.GetInt64(1),
			TotalAtas: leitor.GetInt64(2),
			TotalItens: leitor.GetInt64(3),
			TotalOrgaosMonitorados: leitor.GetInt64(4));
	}
}
