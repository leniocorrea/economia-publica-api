using System;
using System.Linq.Expressions;
using EconomIA.Common.Domain;
using EconomIA.Common.Persistence;

namespace EconomIA.Domain.Repositories;

public interface IExecucoesCargaReader : IReadRepository<ExecucaoCarga>;

public static class ExecucoesCargaSpecifications {
	public static Specification<ExecucaoCarga> All() => new All();
	public static Specification<ExecucaoCarga> WithId(Int64 id) => new WithId(id);
	public static Specification<ExecucaoCarga> ComStatus(String status) => new ComStatus(status);
	public static Specification<ExecucaoCarga> Sucesso() => new ComStatus("sucesso");
	public static Specification<ExecucaoCarga> Erro() => new ComStatus("erro");
	public static Specification<ExecucaoCarga> EmAndamento() => new ComStatus("em_andamento");
}

file class All : Specification<ExecucaoCarga> {
	public override Expression<Func<ExecucaoCarga, Boolean>> Rule() => x => true;
}

file class WithId(Int64 id) : Specification<ExecucaoCarga> {
	public override Expression<Func<ExecucaoCarga, Boolean>> Rule() => x => x.Id == id;
}

file class ComStatus(String status) : Specification<ExecucaoCarga> {
	public override Expression<Func<ExecucaoCarga, Boolean>> Rule() => x => x.Status == status;
}
