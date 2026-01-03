using Serilog.Events;

namespace EconomIA.CargaDeDados.Configuration;

public sealed class SentryConfiguration {
	public const String SectionName = "Sentry";

	public Boolean Enabled { get; init; } = false;
	public String? Dsn { get; init; }
	public LogEventLevel MinimumEventLevel { get; init; } = LogEventLevel.Error;
}
