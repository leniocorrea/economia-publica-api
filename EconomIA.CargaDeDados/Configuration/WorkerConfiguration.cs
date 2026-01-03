namespace EconomIA.CargaDeDados.Configuration;

public sealed class WorkerConfiguration {
	public const String SectionName = "Worker";

	public String CronExpression { get; init; } = "0 2 * * *";
	public Int32 DiasRetroativos { get; init; } = 1;
	public Boolean SincronizarOrgaos { get; init; } = true;
	public String CronSincronizacaoOrgaos { get; init; } = "0 0 * * 0";
	public Int32 MaxConcorrencia { get; init; } = 4;
}
