using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EconomIA.Adapters.Persistence;

public class MigrationDbContextFactory : IDesignTimeDbContextFactory<EconomIACommandDbContext> {
	private const String DefaultConnectionString = "Host=localhost;Port=5432;Database=economia;Username=postgres;Password=postgres;Include Error Detail=true";

	public EconomIACommandDbContext CreateDbContext(String[] args) {
		var connectionString = GetConnectionString(args);
		var optionsBuilder = new DbContextOptionsBuilder<EconomIACommandDbContext>();
		optionsBuilder.UseNpgsql(connectionString);

		return new EconomIACommandDbContext(optionsBuilder.Options);
	}

	private static String GetConnectionString(String[] args) {
		var fromArgs = GetConnectionStringFromArgs(args);

		if (fromArgs is not null) {
			return fromArgs;
		}

		var fromEnv = Environment.GetEnvironmentVariable("ECONOMIA_CONNECTION_STRING");

		if (fromEnv is not null) {
			return fromEnv;
		}

		return DefaultConnectionString;
	}

	private static String? GetConnectionStringFromArgs(String[] args) {
		for (var i = 0; i < args.Length - 1; i++) {
			if (args[i] is "--connection" or "-c") {
				return args[i + 1];
			}
		}

		return null;
	}
}
