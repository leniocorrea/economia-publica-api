using Cronos;
using EconomIA.CargaDeDados.Configuration;
using EconomIA.CargaDeDados.Models;
using EconomIA.CargaDeDados.Observability;
using EconomIA.CargaDeDados.Repositories;
using EconomIA.CargaDeDados.Services;
using EconomIA.CargaDeDados.Workers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace EconomIA.CargaDeDados.Tests.Workers;

public class CargaDiariaWorkerTests {
	private readonly IServiceScopeFactory scopeFactory;
	private readonly ILogger<CargaDiariaWorker> logger;
	private readonly IOptions<WorkerConfiguration> configuracao;

	public CargaDiariaWorkerTests() {
		scopeFactory = Substitute.For<IServiceScopeFactory>();
		logger = new NullLogger<CargaDiariaWorker>();
		configuracao = Options.Create(new WorkerConfiguration {
			CronExpression = "0 2 * * *",
			DiasRetroativos = 1
		});
	}

	[Fact]
	public void cria_worker_com_cron_expression_valida() {
		var action = () => new CargaDiariaWorker(scopeFactory, logger, configuracao);

		action.Should().NotThrow();
	}

	[Fact]
	public void cron_expression_invalida_lanca_excecao() {
		var configInvalida = Options.Create(new WorkerConfiguration {
			CronExpression = "invalido"
		});

		var action = () => new CargaDiariaWorker(scopeFactory, logger, configInvalida);

		action.Should().Throw<CronFormatException>();
	}

	[Fact]
	public async Task cancellation_token_interrompe_execucao() {
		var worker = new CargaDiariaWorker(scopeFactory, logger, configuracao);
		var cts = new CancellationTokenSource();
		cts.Cancel();

		var task = worker.StartAsync(cts.Token);

		await task;
		task.IsCompleted.Should().BeTrue();
	}
}

public class SincronizacaoOrgaosWorkerTests {
	private readonly IServiceScopeFactory scopeFactory;
	private readonly ILogger<SincronizacaoOrgaosWorker> logger;

	public SincronizacaoOrgaosWorkerTests() {
		scopeFactory = Substitute.For<IServiceScopeFactory>();
		logger = new NullLogger<SincronizacaoOrgaosWorker>();
	}

	[Fact]
	public void cria_worker_com_cron_expression_valida() {
		var configuracao = Options.Create(new WorkerConfiguration {
			SincronizarOrgaos = true,
			CronSincronizacaoOrgaos = "0 0 * * 0"
		});

		var action = () => new SincronizacaoOrgaosWorker(scopeFactory, logger, configuracao);

		action.Should().NotThrow();
	}

	[Fact]
	public void cron_expression_invalida_lanca_excecao() {
		var configInvalida = Options.Create(new WorkerConfiguration {
			SincronizarOrgaos = true,
			CronSincronizacaoOrgaos = "invalido"
		});

		var action = () => new SincronizacaoOrgaosWorker(scopeFactory, logger, configInvalida);

		action.Should().Throw<CronFormatException>();
	}

	[Fact]
	public async Task worker_desabilitado_finaliza_imediatamente() {
		var configuracao = Options.Create(new WorkerConfiguration {
			SincronizarOrgaos = false
		});
		var worker = new SincronizacaoOrgaosWorker(scopeFactory, logger, configuracao);
		var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

		await worker.StartAsync(cts.Token);
		await Task.Delay(100);

		cts.IsCancellationRequested.Should().BeFalse();
	}

	[Fact]
	public async Task cancellation_token_interrompe_execucao() {
		var configuracao = Options.Create(new WorkerConfiguration {
			SincronizarOrgaos = true,
			CronSincronizacaoOrgaos = "0 0 * * 0"
		});
		var worker = new SincronizacaoOrgaosWorker(scopeFactory, logger, configuracao);
		var cts = new CancellationTokenSource();
		cts.Cancel();

		var task = worker.StartAsync(cts.Token);

		await task;
		task.IsCompleted.Should().BeTrue();
	}
}
