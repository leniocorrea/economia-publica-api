using System;
using System.Threading;
using System.Threading.Tasks;

namespace EconomIA.Domain.Repositories;

public interface IEstatisticas {
	Task<EstatisticasGerais> ObterEstatisticasGeraisAsync(CancellationToken cancellationToken = default);
}

public record EstatisticasGerais(
	Int64 TotalCompras,
	Int64 TotalContratos,
	Int64 TotalAtas,
	Int64 TotalItens,
	Int64 TotalOrgaosMonitorados);
