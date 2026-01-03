using EconomIA.CargaDeDados.HealthChecks;
using EconomIA.CargaDeDados.Models;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace EconomIA.CargaDeDados.Tests.HealthChecks;

public class WorkerHealthCheckTests {
	private readonly WorkerHealthCheck healthCheck;

	public WorkerHealthCheckTests() {
		healthCheck = new WorkerHealthCheck();
		ResetarEstadoEstatico();
	}

	private void ResetarEstadoEstatico() {
		var tipo = typeof(WorkerHealthCheck);
		var campoUltimaExecucaoIniciada = tipo.GetField("ultimaExecucaoIniciada", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		var campoUltimaExecucaoFinalizada = tipo.GetField("ultimaExecucaoFinalizada", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		var campoStatusUltimaExecucao = tipo.GetField("statusUltimaExecucao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		var campoIdentificadorUltimaExecucao = tipo.GetField("identificadorUltimaExecucao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

		campoUltimaExecucaoIniciada?.SetValue(null, null);
		campoUltimaExecucaoFinalizada?.SetValue(null, null);
		campoStatusUltimaExecucao?.SetValue(null, null);
		campoIdentificadorUltimaExecucao?.SetValue(null, null);
	}

	[Fact]
	public async Task retorna_healthy_quando_nunca_executou() {
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Status.Should().Be(HealthStatus.Healthy);
		result.Description.Should().Be("Worker saudavel");
	}

	[Fact]
	public async Task retorna_healthy_quando_em_execucao() {
		WorkerHealthCheck.RegistrarInicioExecucao(123);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Status.Should().Be(HealthStatus.Healthy);
		result.Description.Should().Be("Worker em execucao");
	}

	[Fact]
	public async Task retorna_degraded_quando_ultima_execucao_falhou() {
		WorkerHealthCheck.RegistrarInicioExecucao(123);
		WorkerHealthCheck.RegistrarFimExecucao(StatusExecucao.Erro);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Status.Should().Be(HealthStatus.Degraded);
		result.Description.Should().Be("Ultima execucao falhou");
	}

	[Fact]
	public async Task retorna_healthy_quando_ultima_execucao_sucesso() {
		WorkerHealthCheck.RegistrarInicioExecucao(123);
		WorkerHealthCheck.RegistrarFimExecucao(StatusExecucao.Sucesso);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Status.Should().Be(HealthStatus.Healthy);
		result.Description.Should().Be("Worker saudavel");
	}

	[Fact]
	public async Task dados_contem_ultima_execucao_iniciada() {
		WorkerHealthCheck.RegistrarInicioExecucao(456);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Data.Should().ContainKey("ultima_execucao_iniciada");
		result.Data["ultima_execucao_iniciada"].Should().NotBe("nunca");
	}

	[Fact]
	public async Task dados_contem_identificador_ultima_execucao() {
		WorkerHealthCheck.RegistrarInicioExecucao(789);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Data.Should().ContainKey("identificador_ultima_execucao");
		result.Data["identificador_ultima_execucao"].Should().Be("789");
	}

	[Fact]
	public async Task dados_contem_status_em_andamento() {
		WorkerHealthCheck.RegistrarInicioExecucao(123);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Data.Should().ContainKey("status_ultima_execucao");
		result.Data["status_ultima_execucao"].Should().Be("em_andamento");
	}

	[Fact]
	public async Task registrar_fim_atualiza_status() {
		WorkerHealthCheck.RegistrarInicioExecucao(123);
		WorkerHealthCheck.RegistrarFimExecucao(StatusExecucao.Sucesso);
		var context = new HealthCheckContext();

		var result = await healthCheck.CheckHealthAsync(context);

		result.Data["status_ultima_execucao"].Should().Be(StatusExecucao.Sucesso);
		result.Data["ultima_execucao_finalizada"].Should().NotBe("nunca");
	}
}
