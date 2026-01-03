using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EconomIA.CargaDeDados.HealthChecks;

public class WorkerHealthCheck : IHealthCheck {
	private static DateTime? ultimaExecucaoIniciada;
	private static DateTime? ultimaExecucaoFinalizada;
	private static String? statusUltimaExecucao;
	private static Int64? identificadorUltimaExecucao;

	public static void RegistrarInicioExecucao(Int64 identificador) {
		ultimaExecucaoIniciada = DateTime.UtcNow;
		identificadorUltimaExecucao = identificador;
		statusUltimaExecucao = "em_andamento";
	}

	public static void RegistrarFimExecucao(String status) {
		ultimaExecucaoFinalizada = DateTime.UtcNow;
		statusUltimaExecucao = status;
	}

	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
		var dados = new Dictionary<String, Object> {
			["ultima_execucao_iniciada"] = ultimaExecucaoIniciada?.ToString("o") ?? "nunca",
			["ultima_execucao_finalizada"] = ultimaExecucaoFinalizada?.ToString("o") ?? "nunca",
			["status_ultima_execucao"] = statusUltimaExecucao ?? "aguardando",
			["identificador_ultima_execucao"] = identificadorUltimaExecucao?.ToString() ?? "n/a"
		};

		if (statusUltimaExecucao == "em_andamento") {
			return Task.FromResult(HealthCheckResult.Healthy("Worker em execucao", dados));
		}

		if (statusUltimaExecucao == "erro") {
			return Task.FromResult(HealthCheckResult.Degraded("Ultima execucao falhou", data: dados));
		}

		return Task.FromResult(HealthCheckResult.Healthy("Worker saudavel", dados));
	}
}
