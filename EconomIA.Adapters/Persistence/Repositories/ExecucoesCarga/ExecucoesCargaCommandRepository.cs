using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Common.EntityFramework.Repositories;
using EconomIA.Common.Persistence;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;

namespace EconomIA.Adapters.Persistence.Repositories.ExecucoesCarga;

public class ExecucoesCargaCommandRepository : CommandRepository<EconomIACommandDbContext, ExecucaoCarga>, IExecucoesCarga {
	private readonly EconomIACommandDbContext database;

	public ExecucoesCargaCommandRepository(EconomIACommandDbContext database) : base(database) {
		this.database = database;
	}

	public async Task<Result<ExecucaoCarga, RepositoryError>> CriarPendenteAsync(
		String modoExecucao,
		String tipoGatilho,
		ParametrosExecucao? parametros,
		CancellationToken cancellationToken = default) {

		var execucao = ExecucaoCarga.CriarPendente(modoExecucao, tipoGatilho, parametros);

		await database.AddAsync(execucao, cancellationToken);
		await database.SaveChangesAsync(cancellationToken);

		return Result.Success<ExecucaoCarga, RepositoryError>(execucao);
	}
}
