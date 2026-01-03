using System.Collections.Concurrent;
using System.Diagnostics;

namespace EconomIA.CargaDeDados.Observability;

public class MetricasExecucao {
	private readonly ConcurrentDictionary<Int64, MetricasOrgao> metricasPorOrgao = new();
	private readonly Stopwatch cronometro = Stopwatch.StartNew();

	public Int64 DuracaoTotalMs => cronometro.ElapsedMilliseconds;

	public Int32 TotalOrgaosProcessados => metricasPorOrgao.Count;
	public Int32 TotalOrgaosComErro => metricasPorOrgao.Values.Count(x => x.Status == "erro");

	public Int32 TotalComprasProcessadas => metricasPorOrgao.Values.Sum(x => x.ComprasProcessadas);
	public Int32 TotalContratosProcessados => metricasPorOrgao.Values.Sum(x => x.ContratosProcessados);
	public Int32 TotalAtasProcessadas => metricasPorOrgao.Values.Sum(x => x.AtasProcessadas);
	public Int32 TotalItensIndexados => metricasPorOrgao.Values.Sum(x => x.ItensProcessados);

	public MetricasOrgao ObterOuCriarMetricasOrgao(Int64 identificadorDoOrgao) {
		return metricasPorOrgao.GetOrAdd(identificadorDoOrgao, id => new MetricasOrgao(id));
	}

	public IEnumerable<MetricasOrgao> ObterTodasMetricas() => metricasPorOrgao.Values;

	public void Finalizar() {
		cronometro.Stop();
	}
}

public class MetricasOrgao {
	public Int64 IdentificadorDoOrgao { get; }
	public String Status { get; set; } = "em_andamento";
	public String? MensagemErro { get; set; }

	public DateTime InicioEm { get; set; } = DateTime.UtcNow;
	public DateTime? FimEm { get; set; }
	public Int64 DuracaoMs => FimEm.HasValue ? (Int64)(FimEm.Value - InicioEm).TotalMilliseconds : 0;

	public Int32 ComprasProcessadas { get; set; }
	public Int64 ComprasDuracaoMs { get; set; }

	public Int32 ContratosProcessados { get; set; }
	public Int64 ContratosDuracaoMs { get; set; }

	public Int32 AtasProcessadas { get; set; }
	public Int64 AtasDuracaoMs { get; set; }

	public Int32 ItensProcessados { get; set; }

	public DateTime? DataInicialProcessada { get; set; }
	public DateTime? DataFinalProcessada { get; set; }

	public MetricasOrgao(Int64 identificadorDoOrgao) {
		IdentificadorDoOrgao = identificadorDoOrgao;
	}

	public void Finalizar(String status, String? mensagemErro = null) {
		FimEm = DateTime.UtcNow;
		Status = status;
		MensagemErro = mensagemErro;
	}
}
