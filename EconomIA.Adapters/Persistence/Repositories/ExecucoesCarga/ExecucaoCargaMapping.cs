using EconomIA.Common.EntityFramework.Mappings;
using EconomIA.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EconomIA.Adapters.Persistence.Repositories.ExecucoesCarga;

public class ExecucaoCargaMapping : AggregateMapping<ExecucaoCarga> {
	public override void Configure(EntityTypeBuilder<ExecucaoCarga> builder) {
		base.Configure(builder);

		builder.ToTable("execucao_carga");

		builder.Property(x => x.ModoExecucao)
			.HasColumnName("modo_execucao")
			.IsRequired();

		builder.Property(x => x.TipoGatilho)
			.HasColumnName("tipo_gatilho")
			.IsRequired();

		builder.Property(x => x.InicioEm)
			.HasColumnName("inicio_em");

		builder.Property(x => x.FimEm)
			.HasColumnName("fim_em");

		builder.Property(x => x.ParametrosJson)
			.HasColumnName("parametros")
			.HasColumnType("jsonb");

		builder.Property(x => x.DuracaoTotalMs)
			.HasColumnName("duracao_total_ms");

		builder.Property(x => x.Status)
			.HasColumnName("status")
			.IsRequired();

		builder.Property(x => x.MensagemErro)
			.HasColumnName("mensagem_erro");

		builder.Property(x => x.StackTrace)
			.HasColumnName("stack_trace");

		builder.Property(x => x.TotalOrgaosProcessados)
			.HasColumnName("total_orgaos_processados");

		builder.Property(x => x.TotalOrgaosComErro)
			.HasColumnName("total_orgaos_com_erro");

		builder.Property(x => x.TotalComprasProcessadas)
			.HasColumnName("total_compras_processadas");

		builder.Property(x => x.TotalContratosProcessados)
			.HasColumnName("total_contratos_processados");

		builder.Property(x => x.TotalAtasProcessadas)
			.HasColumnName("total_atas_processadas");

		builder.Property(x => x.TotalItensIndexados)
			.HasColumnName("total_itens_indexados");

		builder.Property(x => x.VersaoAplicacao)
			.HasColumnName("versao_aplicacao");

		builder.Property(x => x.Hostname)
			.HasColumnName("hostname");

		builder.Property(x => x.CriadoEm)
			.HasColumnName("criado_em")
			.IsRequired();

		builder.HasMany(x => x.Orgaos)
			.WithOne(x => x.Execucao)
			.HasForeignKey(x => x.IdentificadorDaExecucao)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Ignore(x => x.DomainEvents);
	}
}

public class ExecucaoCargaOrgaoMapping : EntityMapping<ExecucaoCargaOrgao> {
	public override void Configure(EntityTypeBuilder<ExecucaoCargaOrgao> builder) {
		base.Configure(builder);

		builder.ToTable("execucao_carga_orgao");

		builder.Property(x => x.IdentificadorDaExecucao)
			.HasColumnName("identificador_da_execucao")
			.IsRequired();

		builder.Property(x => x.IdentificadorDoOrgao)
			.HasColumnName("identificador_do_orgao")
			.IsRequired();

		builder.Property(x => x.InicioEm)
			.HasColumnName("inicio_em")
			.IsRequired();

		builder.Property(x => x.FimEm)
			.HasColumnName("fim_em");

		builder.Property(x => x.DuracaoMs)
			.HasColumnName("duracao_ms");

		builder.Property(x => x.Status)
			.HasColumnName("status")
			.IsRequired();

		builder.Property(x => x.MensagemErro)
			.HasColumnName("mensagem_erro");

		builder.Property(x => x.ComprasProcessadas)
			.HasColumnName("compras_processadas");

		builder.Property(x => x.ComprasDuracaoMs)
			.HasColumnName("compras_duracao_ms");

		builder.Property(x => x.ContratosProcessados)
			.HasColumnName("contratos_processados");

		builder.Property(x => x.ContratosDuracaoMs)
			.HasColumnName("contratos_duracao_ms");

		builder.Property(x => x.AtasProcessadas)
			.HasColumnName("atas_processadas");

		builder.Property(x => x.AtasDuracaoMs)
			.HasColumnName("atas_duracao_ms");

		builder.Property(x => x.ItensProcessados)
			.HasColumnName("itens_processados");

		builder.Property(x => x.DataInicialProcessada)
			.HasColumnName("data_inicial_processada");

		builder.Property(x => x.DataFinalProcessada)
			.HasColumnName("data_final_processada");

		builder.Property(x => x.CriadoEm)
			.HasColumnName("criado_em")
			.IsRequired();

		builder.HasOne(x => x.Orgao)
			.WithMany()
			.HasForeignKey(x => x.IdentificadorDoOrgao)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
