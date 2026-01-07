using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EconomIA.Common.EntityFramework.Repositories;
using EconomIA.Common.Persistence;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace EconomIA.Adapters.Persistence.Repositories.Usuarios;

public class UsuariosQueryRepository : QueryRepository<EconomIAQueryDbContext, Usuario>, IUsuariosReader {
	private readonly IDbContextFactory<EconomIAQueryDbContext> contextFactory;

	public UsuariosQueryRepository(IDbContextFactory<EconomIAQueryDbContext> factory) : base(factory) {
		contextFactory = factory;
	}

	protected override Task<IDataScope<Usuario>> CreateScope(CancellationToken cancellationToken = default) {
		var scope = new UsuariosQueryScope(contextFactory, cancellationToken);
		return Task.FromResult<IDataScope<Usuario>>(scope);
	}
}

file class UsuariosQueryScope(IDbContextFactory<EconomIAQueryDbContext> factory, CancellationToken cancellationToken = default) : IDataScope<Usuario> {
	private EconomIAQueryDbContext? context;
	private Boolean disposed;

	public async Task<IQueryable<Usuario>> Query() {
		ObjectDisposedException.ThrowIf(disposed, typeof(UsuariosQueryScope));

		context ??= await factory.CreateDbContextAsync(cancellationToken);
		return context.Set<Usuario>()
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
