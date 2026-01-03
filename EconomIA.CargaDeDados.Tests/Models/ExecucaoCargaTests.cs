using EconomIA.CargaDeDados.Models;
using FluentAssertions;
using Xunit;

namespace EconomIA.CargaDeDados.Tests.Models;

public class ExecucaoCargaTests {
	[Fact]
	public void status_padrao_eh_em_andamento() {
		var execucao = new ExecucaoCarga();

		execucao.Status.Should().Be(StatusExecucao.EmAndamento);
	}

	[Fact]
	public void contadores_iniciam_zerados() {
		var execucao = new ExecucaoCarga();

		execucao.TotalOrgaosProcessados.Should().Be(0);
		execucao.TotalOrgaosComErro.Should().Be(0);
		execucao.TotalComprasProcessadas.Should().Be(0);
		execucao.TotalContratosProcessados.Should().Be(0);
		execucao.TotalAtasProcessadas.Should().Be(0);
		execucao.TotalItensIndexados.Should().Be(0);
	}

	[Fact]
	public void campos_opcionais_sao_nulos() {
		var execucao = new ExecucaoCarga();

		execucao.FimEm.Should().BeNull();
		execucao.DuracaoTotalMs.Should().BeNull();
		execucao.MensagemErro.Should().BeNull();
		execucao.StackTrace.Should().BeNull();
		execucao.VersaoAplicacao.Should().BeNull();
		execucao.Hostname.Should().BeNull();
	}
}

public class StatusExecucaoTests {
	[Fact]
	public void constantes_estao_corretas() {
		StatusExecucao.EmAndamento.Should().Be("em_andamento");
		StatusExecucao.Sucesso.Should().Be("sucesso");
		StatusExecucao.Erro.Should().Be("erro");
		StatusExecucao.Parcial.Should().Be("parcial");
		StatusExecucao.Cancelado.Should().Be("cancelado");
	}
}

public class ModoExecucaoTests {
	[Fact]
	public void constantes_estao_corretas() {
		ModoExecucao.Diaria.Should().Be("diaria");
		ModoExecucao.Incremental.Should().Be("incremental");
		ModoExecucao.Manual.Should().Be("manual");
		ModoExecucao.Orgaos.Should().Be("orgaos");
	}
}

public class TipoGatilhoTests {
	[Fact]
	public void constantes_estao_corretas() {
		TipoGatilho.Scheduler.Should().Be("scheduler");
		TipoGatilho.Cli.Should().Be("cli");
		TipoGatilho.Api.Should().Be("api");
	}
}
