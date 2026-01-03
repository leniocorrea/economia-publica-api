using EconomIA.CargaDeDados.Configuration;
using FluentAssertions;
using Xunit;

namespace EconomIA.CargaDeDados.Tests.Configuration;

public class WorkerConfigurationTests {
	[Fact]
	public void section_name_esta_correto() {
		WorkerConfiguration.SectionName.Should().Be("Worker");
	}

	[Fact]
	public void valores_padrao_estao_corretos() {
		var config = new WorkerConfiguration();

		config.CronExpression.Should().Be("0 2 * * *");
		config.DiasRetroativos.Should().Be(1);
		config.SincronizarOrgaos.Should().BeTrue();
		config.CronSincronizacaoOrgaos.Should().Be("0 0 * * 0");
		config.MaxConcorrencia.Should().Be(4);
	}

	[Fact]
	public void permite_alterar_cron_expression() {
		var config = new WorkerConfiguration { CronExpression = "0 3 * * *" };

		config.CronExpression.Should().Be("0 3 * * *");
	}

	[Fact]
	public void permite_alterar_dias_retroativos() {
		var config = new WorkerConfiguration { DiasRetroativos = 7 };

		config.DiasRetroativos.Should().Be(7);
	}

	[Fact]
	public void permite_desabilitar_sincronizacao_de_orgaos() {
		var config = new WorkerConfiguration { SincronizarOrgaos = false };

		config.SincronizarOrgaos.Should().BeFalse();
	}

	[Fact]
	public void permite_alterar_max_concorrencia() {
		var config = new WorkerConfiguration { MaxConcorrencia = 8 };

		config.MaxConcorrencia.Should().Be(8);
	}
}
