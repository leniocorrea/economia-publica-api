using System;
using EconomIA.Common.EntityFramework.Mappings;
using EconomIA.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EconomIA.Adapters.Persistence.Repositories.ResultadosDoItem;

public class ResultadoDoItemMapping : EntityMapping<ResultadoDoItem> {
	public override void Configure(EntityTypeBuilder<ResultadoDoItem> builder) {
		base.Configure(builder);

		builder.ToTable("resultado_do_item");

		builder.Property(x => x.Id)
			.HasColumnName("identificador");

		builder.Property(x => x.IdentificadorDoItemDaCompra)
			.HasColumnName("identificador_do_item_da_compra")
			.IsRequired();

		builder.Property(x => x.NiFornecedor)
			.HasColumnName("ni_fornecedor")
			.HasMaxLength(50)
			.IsRequired(false);

		builder.Property(x => x.NomeRazaoSocialFornecedor)
			.HasColumnName("nome_razao_social_fornecedor")
			.HasMaxLength(500)
			.IsRequired(false);

		builder.Property(x => x.QuantidadeHomologada)
			.HasColumnName("quantidade_homologada")
			.HasPrecision(18, 4)
			.IsRequired(false);

		builder.Property(x => x.ValorUnitarioHomologado)
			.HasColumnName("valor_unitario_homologado")
			.HasPrecision(18, 4)
			.IsRequired(false);

		builder.Property(x => x.ValorTotalHomologado)
			.HasColumnName("valor_total_homologado")
			.HasPrecision(18, 2)
			.IsRequired(false);

		builder.Property(x => x.SituacaoCompraItemResultadoNome)
			.HasColumnName("situacao_compra_item_resultado_nome")
			.HasMaxLength(200)
			.IsRequired(false);

		builder.Property(x => x.DataResultado)
			.HasColumnName("data_resultado")
			.IsRequired(false);

		builder.Property(x => x.DataAtualizacao)
			.HasColumnName("data_atualizacao")
			.IsRequired(false);

		builder.Property(x => x.CriadoEm)
			.HasColumnName("criado_em")
			.IsRequired();

		builder.Property(x => x.AtualizadoEm)
			.HasColumnName("atualizado_em")
			.IsRequired();

		builder.HasOne(x => x.ItemDaCompra)
			.WithMany(x => x.Resultados)
			.HasForeignKey(x => x.IdentificadorDoItemDaCompra);

		builder.HasIndex(x => new { x.IdentificadorDoItemDaCompra, x.NiFornecedor })
			.IsUnique()
			.HasDatabaseName("idx_resultado_item_unique");
	}
}
