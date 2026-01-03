using System.ComponentModel.DataAnnotations.Schema;

namespace EconomIA.CargaDeDados.Models;

[Table("execucao_carga_orgao")]
public class ExecucaoCargaOrgao {
	[Column("identificador")]
	public Int64 Identificador { get; set; }

	[Column("identificador_da_execucao")]
	public Int64 IdentificadorDaExecucao { get; set; }

	[Column("identificador_do_orgao")]
	public Int64 IdentificadorDoOrgao { get; set; }

	[Column("inicio_em")]
	public DateTime InicioEm { get; set; }

	[Column("fim_em")]
	public DateTime? FimEm { get; set; }

	[Column("duracao_ms")]
	public Int64? DuracaoMs { get; set; }

	[Column("status")]
	public String Status { get; set; } = StatusExecucao.EmAndamento;

	[Column("mensagem_erro")]
	public String? MensagemErro { get; set; }

	[Column("compras_processadas")]
	public Int32 ComprasProcessadas { get; set; }

	[Column("contratos_processados")]
	public Int32 ContratosProcessados { get; set; }

	[Column("atas_processadas")]
	public Int32 AtasProcessadas { get; set; }

	[Column("itens_processados")]
	public Int32 ItensProcessados { get; set; }

	[Column("data_inicial_processada")]
	public DateTime? DataInicialProcessada { get; set; }

	[Column("data_final_processada")]
	public DateTime? DataFinalProcessada { get; set; }

	[Column("criado_em")]
	public DateTime CriadoEm { get; set; }
}
