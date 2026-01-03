using EconomIA.CargaDeDados.Configuration;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace EconomIA.CargaDeDados.Tests.Configuration;

public class SentryConfigurationTests {
	[Fact]
	public void section_name_esta_correto() {
		SentryConfiguration.SectionName.Should().Be("Sentry");
	}

	[Fact]
	public void valores_padrao_estao_corretos() {
		var config = new SentryConfiguration();

		config.Enabled.Should().BeFalse();
		config.Dsn.Should().BeNull();
		config.MinimumEventLevel.Should().Be(LogEventLevel.Error);
	}

	[Fact]
	public void permite_habilitar_sentry() {
		var config = new SentryConfiguration { Enabled = true };

		config.Enabled.Should().BeTrue();
	}

	[Fact]
	public void permite_definir_dsn() {
		var config = new SentryConfiguration { Dsn = "https://key@sentry.io/123" };

		config.Dsn.Should().Be("https://key@sentry.io/123");
	}

	[Fact]
	public void permite_alterar_minimum_event_level() {
		var config = new SentryConfiguration { MinimumEventLevel = LogEventLevel.Warning };

		config.MinimumEventLevel.Should().Be(LogEventLevel.Warning);
	}
}
