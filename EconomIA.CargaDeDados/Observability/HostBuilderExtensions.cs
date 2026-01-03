using EconomIA.CargaDeDados.Configuration;
using Serilog;

namespace EconomIA.CargaDeDados.Observability;

public static class HostBuilderExtensions {
	public static IHostBuilder ConfigurarLogging(this IHostBuilder builder, IConfiguration configuration) {
		builder.UseSerilog((context, services, serilogConfiguration) => {
			serilogConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.Enrich.WithEnvironmentName()
				.Enrich.WithMachineName()
				.Enrich.WithProperty("Application", "EconomIA.CargaDeDados")
				.WriteTo.Console(
					outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
				);

			var sentryConfig = configuration.GetSection(SentryConfiguration.SectionName).Get<SentryConfiguration>();

			if (sentryConfig?.Enabled == true && !String.IsNullOrEmpty(sentryConfig.Dsn)) {
				serilogConfiguration.WriteTo.Sentry(options => {
					options.Dsn = sentryConfig.Dsn;
					options.MinimumEventLevel = sentryConfig.MinimumEventLevel;
					options.Environment = context.HostingEnvironment.EnvironmentName;
				});
			}
		});

		return builder;
	}
}
