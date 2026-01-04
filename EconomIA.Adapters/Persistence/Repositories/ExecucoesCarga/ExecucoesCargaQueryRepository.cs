using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EconomIA.Common.Domain;
using EconomIA.Common.EntityFramework.Repositories;
using EconomIA.Common.Persistence;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace EconomIA.Adapters.Persistence.Repositories.ExecucoesCarga;

public class ExecucoesCargaQueryRepository : QueryRepository<EconomIAQueryDbContext, ExecucaoCarga>, IExecucoesCargaReader {
	private readonly IDbContextFactory<EconomIAQueryDbContext> contextFactory;

	public ExecucoesCargaQueryRepository(IDbContextFactory<EconomIAQueryDbContext> factory) : base(factory) {
		contextFactory = factory;
	}

	protected override Task<IDataScope<ExecucaoCarga>> CreateScope(CancellationToken cancellationToken = default) {
		var scope = new ExecucoesCargaQueryScope(contextFactory, cancellationToken);
		return Task.FromResult<IDataScope<ExecucaoCarga>>(scope);
	}
}

file class ExecucoesCargaQueryScope(IDbContextFactory<EconomIAQueryDbContext> factory, CancellationToken cancellationToken = default) : IDataScope<ExecucaoCarga> {
	private EconomIAQueryDbContext? context;
	private Boolean disposed;

	public async Task<IQueryable<ExecucaoCarga>> Query() {
		ObjectDisposedException.ThrowIf(disposed, typeof(ExecucoesCargaQueryScope));

		context ??= await factory.CreateDbContextAsync(cancellationToken);
		return context.Set<ExecucaoCarga>()
			.Include(x => x.Orgaos)
				.ThenInclude(x => x.Orgao)
			.OrderByDescending(x => x.InicioEm)
			.AsQueryable()
			.AsExpandableEFCore();
	}

	public async ValueTask DisposeAsync() {
		if (disposed) {
			return;
		}

		disposed = true;

		if (context is not null) {
			await context.DisposeAsync();
		}
	}
}
