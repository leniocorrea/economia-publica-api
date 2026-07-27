using System.Data;
using Dapper;
using EconomIA.CargaDeDados.Models;

namespace EconomIA.CargaDeDados.Repositories;

public class Atas {
	private readonly IDbConnection conexao;

	public Atas(IDbConnection conexao) {
		this.conexao = conexao;
	}

	public async Task<long> UpsertAsync(Ata ata) {
		var sql = @"
			insert into public.ata (
				identificador_do_orgao,
				numero_controle_pncp_ata,
				numero_controle_pncp_compra,
				numero_ata_registro_preco,
				ano_ata,
				objeto_contratacao,
				cancelado,
				data_cancelamento,
				data_assinatura,
				vigencia_inicio,
				vigencia_fim,
				data_publicacao_pncp,
				data_inclusao,
				data_atualizacao,
				data_atualizacao_global,
				usuario,
				possibilidade_adesao,
				atualizado_em
			) values (
				@IdentificadorDoOrgao,
				@NumeroControlePncpAta,
				@NumeroControlePncpCompra,
				@NumeroAtaRegistroPreco,
				@AnoAta,
				@ObjetoContratacao,
				@Cancelado,
				@DataCancelamento,
				@DataAssinatura,
				@VigenciaInicio,
				@VigenciaFim,
				@DataPublicacaoPncp,
				@DataInclusao,
				@DataAtualizacao,
				@DataAtualizacaoGlobal,
				@Usuario,
				@PossibilidadeAdesao,
				now()
			)
			on conflict (numero_controle_pncp_ata) do update
			set
				numero_controle_pncp_compra = excluded.numero_controle_pncp_compra,
				numero_ata_registro_preco = excluded.numero_ata_registro_preco,
				objeto_contratacao = excluded.objeto_contratacao,
				cancelado = excluded.cancelado,
				data_cancelamento = excluded.data_cancelamento,
				data_assinatura = excluded.data_assinatura,
				vigencia_inicio = excluded.vigencia_inicio,
				vigencia_fim = excluded.vigencia_fim,
				data_publicacao_pncp = excluded.data_publicacao_pncp,
				data_inclusao = excluded.data_inclusao,
				data_atualizacao = excluded.data_atualizacao,
				data_atualizacao_global = excluded.data_atualizacao_global,
				usuario = excluded.usuario,
				possibilidade_adesao = excluded.possibilidade_adesao,
				atualizado_em = now()
			returning identificador;
		";

		return await conexao.ExecuteScalarAsync<long>(sql, ata);
	}

	public async Task<List<string>> ObterComprasOrfasComAdesaoAsync() {
		var sql = @"
			select distinct a.numero_controle_pncp_compra
			from public.ata a
			left join public.compra c on c.numero_controle_pncp = a.numero_controle_pncp_compra
			where a.cancelado = false
			  and a.vigencia_fim >= current_date
			  and a.possibilidade_adesao = true
			  and a.numero_controle_pncp_compra is not null
			  and (c.identificador is null or c.itens_carregados is not true);
		";

		var resultado = await conexao.QueryAsync<string>(sql, commandTimeout: 300);
		return resultado.ToList();
	}
}
