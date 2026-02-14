using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Common.Domain;
using EconomIA.Common.EntityFramework.Repositories;
using EconomIA.Common.Persistence;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EconomIA.Adapters.Persistence.Repositories.ExecucoesCarga;

public class ExecucoesCargaQueryRepository : QueryRepository<EconomIAQueryDbContext, ExecucaoCarga>, IExecucoesCargaReader {
	private readonly IDbContextFactory<EconomIAQueryDbContext> contextFactory;

	public ExecucoesCargaQueryRepository(IDbContextFactory<EconomIAQueryDbContext> factory) : base(factory) {
		contextFactory = factory;
	}

	public async Task<Result<ExecucaoCarga, RepositoryError>> RetrieveComOrgaos(Int64 id, CancellationToken cancellationToken = default) {
		await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

		var execucao = await context.Set<ExecucaoCarga>()
			.Include(x => x.Orgaos)
				.ThenInclude(x => x.Orgao)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

		if (execucao is null) {
			return RepositoryError.NotFound($"ExecucaoCarga with Id {id} not found.");
		}

		return execucao;
	}
}
