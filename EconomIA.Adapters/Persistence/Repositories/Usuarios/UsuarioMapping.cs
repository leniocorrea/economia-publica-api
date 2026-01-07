using EconomIA.Common.EntityFramework.Mappings;
using EconomIA.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EconomIA.Adapters.Persistence.Repositories.Usuarios;

public class UsuarioMapping : AggregateMapping<Usuario> {
	public override void Configure(EntityTypeBuilder<Usuario> builder) {
		base.Configure(builder);

		builder.ToTable("usuario");

		builder.Property(x => x.IdentificadorExterno)
			.HasColumnName("identificador_externo")
			.IsRequired();

		builder.Property(x => x.Nome)
			.HasColumnName("nome")
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(x => x.Email)
			.HasColumnName("email")
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(x => x.SenhaHash)
			.HasColumnName("senha_hash")
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(x => x.Perfil)
			.HasColumnName("perfil")
			.HasMaxLength(50)
			.IsRequired()
			.HasDefaultValue(Perfis.Administrador);

		builder.Property(x => x.Ativo)
			.HasColumnName("ativo")
			.HasDefaultValue(true);

		builder.Property(x => x.UltimoAcesso)
			.HasColumnName("ultimo_acesso");

		builder.Property(x => x.CriadoEm)
			.HasColumnName("criado_em")
			.IsRequired();

		builder.Property(x => x.AtualizadoEm)
			.HasColumnName("atualizado_em")
			.IsRequired();

		builder.HasIndex(x => x.IdentificadorExterno)
			.IsUnique()
			.HasDatabaseName("un_usuario_identificador_externo");

		builder.HasIndex(x => x.Email)
			.IsUnique()
			.HasDatabaseName("un_usuario_email");

		builder.Ignore(x => x.DomainEvents);
	}
}
