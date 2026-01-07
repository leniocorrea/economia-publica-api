using System;
using System.Linq.Expressions;
using EconomIA.Common.Domain;
using EconomIA.Common.Persistence;

namespace EconomIA.Domain.Repositories;

public interface IUsuariosReader : IReadRepository<Usuario>;
public interface IUsuarios : IRepository<Usuario>;

public static class UsuariosSpecifications {
	public static Specification<Usuario> All() => new All();
	public static Specification<Usuario> WithId(Int64 id) => new WithId(id);
	public static Specification<Usuario> WithIdentificadorExterno(Guid identificadorExterno) => new WithIdentificadorExterno(identificadorExterno);
	public static Specification<Usuario> WithEmail(String email) => new WithEmail(email);
	public static Specification<Usuario> Ativos() => new Ativos();
}

file class All : Specification<Usuario> {
	public override Expression<Func<Usuario, Boolean>> Rule() => x => true;
}

file class WithId(Int64 id) : Specification<Usuario> {
	public override Expression<Func<Usuario, Boolean>> Rule() => x => x.Id == id;
}

file class WithIdentificadorExterno(Guid identificadorExterno) : Specification<Usuario> {
	public override Expression<Func<Usuario, Boolean>> Rule() => x => x.IdentificadorExterno == identificadorExterno;
}

file class WithEmail(String email) : Specification<Usuario> {
	public override Expression<Func<Usuario, Boolean>> Rule() => x => x.Email == email;
}

file class Ativos : Specification<Usuario> {
	public override Expression<Func<Usuario, Boolean>> Rule() => x => x.Ativo;
}
