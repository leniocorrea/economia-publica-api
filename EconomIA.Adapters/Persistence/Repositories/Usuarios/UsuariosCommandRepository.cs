using EconomIA.Common.EntityFramework.Repositories;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;

namespace EconomIA.Adapters.Persistence.Repositories.Usuarios;

public class UsuariosCommandRepository(EconomIACommandDbContext database) : CommandRepository<EconomIACommandDbContext, Usuario>(database), IUsuarios;
