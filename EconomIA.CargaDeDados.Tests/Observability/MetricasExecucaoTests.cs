using EconomIA.CargaDeDados.Observability;
using FluentAssertions;
using Xunit;

namespace EconomIA.CargaDeDados.Tests.Observability;

public class MetricasExecucaoTests {
	[Fact]
	public void inicia_sem_orgaos_processados() {
		var metricas = new MetricasExecucao();

		metricas.TotalOrgaosProcessados.Should().Be(0);
		metricas.TotalOrgaosComErro.Should().Be(0);
	}

	[Fact]
	public void inicia_sem_compras_contratos_atas() {
		var metricas = new MetricasExecucao();

		metricas.TotalComprasProcessadas.Should().Be(0);
		metricas.TotalContratosProcessados.Should().Be(0);
		metricas.TotalAtasProcessadas.Should().Be(0);
		metricas.TotalItensIndexados.Should().Be(0);
	}

	[Fact]
	public void duracao_total_aumenta_com_o_tempo() {
		var metricas = new MetricasExecucao();

		Thread.Sleep(10);
		var duracao = metricas.DuracaoTotalMs;

		duracao.Should().BeGreaterThan(0);
	}

	[Fact]
	public void obter_ou_criar_metricas_orgao_cria_nova_metrica() {
		var metricas = new MetricasExecucao();

		var metricaOrgao = metricas.ObterOuCriarMetricasOrgao(123);

		metricaOrgao.Should().NotBeNull();
		metricaOrgao.IdentificadorDoOrgao.Should().Be(123);
		metricas.TotalOrgaosProcessados.Should().Be(1);
	}

	[Fact]
	public void obter_ou_criar_metricas_orgao_retorna_mesma_instancia() {
		var metricas = new MetricasExecucao();

		var metricaOrgao1 = metricas.ObterOuCriarMetricasOrgao(123);
		var metricaOrgao2 = metricas.ObterOuCriarMetricasOrgao(123);

		metricaOrgao1.Should().BeSameAs(metricaOrgao2);
		metricas.TotalOrgaosProcessados.Should().Be(1);
	}

	[Fact]
	public void conta_multiplos_orgaos() {
		var metricas = new MetricasExecucao();

		metricas.ObterOuCriarMetricasOrgao(1);
		metricas.ObterOuCriarMetricasOrgao(2);
		metricas.ObterOuCriarMetricasOrgao(3);

		metricas.TotalOrgaosProcessados.Should().Be(3);
	}

	[Fact]
	public void conta_orgaos_com_erro() {
		var metricas = new MetricasExecucao();

		var orgao1 = metricas.ObterOuCriarMetricasOrgao(1);
		var orgao2 = metricas.ObterOuCriarMetricasOrgao(2);
		var orgao3 = metricas.ObterOuCriarMetricasOrgao(3);

		orgao1.Finalizar("sucesso");
		orgao2.Finalizar("erro", "Falha na conexao");
		orgao3.Finalizar("erro", "Timeout");

		metricas.TotalOrgaosComErro.Should().Be(2);
	}

	[Fact]
	public void soma_compras_de_todos_orgaos() {
		var metricas = new MetricasExecucao();

		var orgao1 = metricas.ObterOuCriarMetricasOrgao(1);
		var orgao2 = metricas.ObterOuCriarMetricasOrgao(2);

		orgao1.ComprasProcessadas = 10;
		orgao2.ComprasProcessadas = 15;

		metricas.TotalComprasProcessadas.Should().Be(25);
	}

	[Fact]
	public void soma_contratos_de_todos_orgaos() {
		var metricas = new MetricasExecucao();

		var orgao1 = metricas.ObterOuCriarMetricasOrgao(1);
		var orgao2 = metricas.ObterOuCriarMetricasOrgao(2);

		orgao1.ContratosProcessados = 5;
		orgao2.ContratosProcessados = 8;

		metricas.TotalContratosProcessados.Should().Be(13);
	}

	[Fact]
	public void soma_atas_de_todos_orgaos() {
		var metricas = new MetricasExecucao();

		var orgao1 = metricas.ObterOuCriarMetricasOrgao(1);
		var orgao2 = metricas.ObterOuCriarMetricasOrgao(2);

		orgao1.AtasProcessadas = 3;
		orgao2.AtasProcessadas = 7;

		metricas.TotalAtasProcessadas.Should().Be(10);
	}

	[Fact]
	public void soma_itens_indexados_de_todos_orgaos() {
		var metricas = new MetricasExecucao();

		var orgao1 = metricas.ObterOuCriarMetricasOrgao(1);
		var orgao2 = metricas.ObterOuCriarMetricasOrgao(2);

		orgao1.ItensProcessados = 100;
		orgao2.ItensProcessados = 200;

		metricas.TotalItensIndexados.Should().Be(300);
	}

	[Fact]
	public void obter_todas_metricas_retorna_todas_as_metricas() {
		var metricas = new MetricasExecucao();

		metricas.ObterOuCriarMetricasOrgao(1);
		metricas.ObterOuCriarMetricasOrgao(2);
		metricas.ObterOuCriarMetricasOrgao(3);

		var todas = metricas.ObterTodasMetricas().ToList();

		todas.Should().HaveCount(3);
		todas.Select(x => x.IdentificadorDoOrgao).Should().BeEquivalentTo(new[] { 1L, 2L, 3L });
	}

	[Fact]
	public void finalizar_para_o_cronometro() {
		var metricas = new MetricasExecucao();

		Thread.Sleep(10);
		metricas.Finalizar();
		var duracaoAposFinalizar = metricas.DuracaoTotalMs;

		Thread.Sleep(10);
		var duracaoDepois = metricas.DuracaoTotalMs;

		duracaoAposFinalizar.Should().Be(duracaoDepois);
	}
}

public class MetricasOrgaoTests {
	[Fact]
	public void inicia_com_status_em_andamento() {
		var metrica = new MetricasOrgao(123);

		metrica.Status.Should().Be("em_andamento");
		metrica.MensagemErro.Should().BeNull();
	}

	[Fact]
	public void inicia_com_contadores_zerados() {
		var metrica = new MetricasOrgao(123);

		metrica.ComprasProcessadas.Should().Be(0);
		metrica.ContratosProcessados.Should().Be(0);
		metrica.AtasProcessadas.Should().Be(0);
		metrica.ItensProcessados.Should().Be(0);
	}

	[Fact]
	public void inicia_com_duracoes_zeradas() {
		var metrica = new MetricasOrgao(123);

		metrica.ComprasDuracaoMs.Should().Be(0);
		metrica.ContratosDuracaoMs.Should().Be(0);
		metrica.AtasDuracaoMs.Should().Be(0);
	}

	[Fact]
	public void inicia_sem_datas_processadas() {
		var metrica = new MetricasOrgao(123);

		metrica.DataInicialProcessada.Should().BeNull();
		metrica.DataFinalProcessada.Should().BeNull();
	}

	[Fact]
	public void duracao_ms_eh_zero_antes_de_finalizar() {
		var metrica = new MetricasOrgao(123);

		metrica.DuracaoMs.Should().Be(0);
	}

	[Fact]
	public void finalizar_define_status_e_fim_em() {
		var metrica = new MetricasOrgao(123);

		metrica.Finalizar("sucesso");

		metrica.Status.Should().Be("sucesso");
		metrica.FimEm.Should().NotBeNull();
		metrica.DuracaoMs.Should().BeGreaterThanOrEqualTo(0);
	}

	[Fact]
	public void finalizar_com_erro_define_mensagem() {
		var metrica = new MetricasOrgao(123);

		metrica.Finalizar("erro", "Falha na conexao com API");

		metrica.Status.Should().Be("erro");
		metrica.MensagemErro.Should().Be("Falha na conexao com API");
	}

	[Fact]
	public void permite_definir_datas_processadas() {
		var metrica = new MetricasOrgao(123);
		var dataInicial = new DateTime(2025, 1, 1);
		var dataFinal = new DateTime(2025, 1, 31);

		metrica.DataInicialProcessada = dataInicial;
		metrica.DataFinalProcessada = dataFinal;

		metrica.DataInicialProcessada.Should().Be(dataInicial);
		metrica.DataFinalProcessada.Should().Be(dataFinal);
	}

	[Fact]
	public void permite_incrementar_contadores() {
		var metrica = new MetricasOrgao(123);

		metrica.ComprasProcessadas++;
		metrica.ContratosProcessados += 5;
		metrica.AtasProcessadas = 10;
		metrica.ItensProcessados = 100;

		metrica.ComprasProcessadas.Should().Be(1);
		metrica.ContratosProcessados.Should().Be(5);
		metrica.AtasProcessadas.Should().Be(10);
		metrica.ItensProcessados.Should().Be(100);
	}
}
