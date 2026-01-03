using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EconomIA.CargaDeDados.HealthChecks;

public static class HealthCheckEndpoints {
	public static WebApplication MapHealthCheckEndpoints(this WebApplication app) {
		app.MapGet("/health", async (HealthCheckService healthCheckService) => {
			var report = await healthCheckService.CheckHealthAsync();

			var response = new {
				status = report.Status.ToString(),
				checks = report.Entries.Select(entry => new {
					name = entry.Key,
					status = entry.Value.Status.ToString(),
					description = entry.Value.Description,
					duration = entry.Value.Duration.TotalMilliseconds,
					data = entry.Value.Data
				}),
				totalDuration = report.TotalDuration.TotalMilliseconds
			};

			return Results.Json(response, statusCode: report.Status == HealthStatus.Healthy ? 200 : 503);
		});

		app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));

		app.MapGet("/health/ready", async (HealthCheckService healthCheckService) => {
			var report = await healthCheckService.CheckHealthAsync(predicate: check => check.Tags.Contains("ready"));

			return report.Status == HealthStatus.Healthy
				? Results.Ok(new { status = "Ready" })
				: Results.Json(new { status = "NotReady" }, statusCode: 503);
		});

		return app;
	}
}
