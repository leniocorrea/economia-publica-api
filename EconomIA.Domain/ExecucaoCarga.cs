using System;
using System.Collections.Generic;
using EconomIA.Common.Domain;

namespace EconomIA.Domain;

public class ExecucaoCarga : Aggregate {
	public String ModoExecucao { get; set; } = String.Empty;
	public String TipoGatilho { get; set; } = String.Empty;
	public DateTime InicioEm { get; set; }
	public DateTime? FimEm { get; set; }
	public Int64? DuracaoTotalMs { get; set; }
	public String Status { get; set; } = String.Empty;
	public String? MensagemErro { get; set; }
	public String? StackTrace { get; set; }
	public Int32 TotalOrgaosProcessados { get; set; }
	public Int32 TotalOrgaosComErro { get; set; }
	public Int32 TotalComprasProcessadas { get; set; }
	public Int32 TotalContratosProcessados { get; set; }
	public Int32 TotalAtasProcessadas { get; set; }
	public Int32 TotalItensIndexados { get; set; }
	public String? VersaoAplicacao { get; set; }
	public String? Hostname { get; set; }
	public DateTime CriadoEm { get; set; }

	public ICollection<ExecucaoCargaOrgao> Orgaos { get; set; } = new List<ExecucaoCargaOrgao>();
}

public class ExecucaoCargaOrgao : Entity {
	public Int64 IdentificadorDaExecucao { get; set; }
	public Int64 IdentificadorDoOrgao { get; set; }
	public DateTime InicioEm { get; set; }
	public DateTime? FimEm { get; set; }
	public Int64? DuracaoMs { get; set; }
	public String Status { get; set; } = String.Empty;
	public String? MensagemErro { get; set; }
	public Int32 ComprasProcessadas { get; set; }
	public Int64 ComprasDuracaoMs { get; set; }
	public Int32 ContratosProcessados { get; set; }
	public Int64 ContratosDuracaoMs { get; set; }
	public Int32 AtasProcessadas { get; set; }
	public Int64 AtasDuracaoMs { get; set; }
	public Int32 ItensProcessados { get; set; }
	public DateTime? DataInicialProcessada { get; set; }
	public DateTime? DataFinalProcessada { get; set; }
	public DateTime CriadoEm { get; set; }

	public ExecucaoCarga? Execucao { get; set; }
	public Orgao? Orgao { get; set; }
}
