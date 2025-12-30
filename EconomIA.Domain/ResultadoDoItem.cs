using System;
using EconomIA.Common.Domain;

namespace EconomIA.Domain;

public class ResultadoDoItem : Entity {
	protected ResultadoDoItem() {
	}

	public ResultadoDoItem(
		Int64 id,
		Int64 identificadorDoItemDaCompra,
		DateTime criadoEm,
		DateTime atualizadoEm,
		String? niFornecedor = null,
		String? nomeRazaoSocialFornecedor = null,
		Decimal? quantidadeHomologada = null,
		Decimal? valorUnitarioHomologado = null,
		Decimal? valorTotalHomologado = null,
		String? situacaoCompraItemResultadoNome = null,
		DateTime? dataResultado = null,
		DateTime? dataAtualizacao = null) : base(id) {
		IdentificadorDoItemDaCompra = identificadorDoItemDaCompra;
		CriadoEm = criadoEm;
		AtualizadoEm = atualizadoEm;
		NiFornecedor = niFornecedor;
		NomeRazaoSocialFornecedor = nomeRazaoSocialFornecedor;
		QuantidadeHomologada = quantidadeHomologada;
		ValorUnitarioHomologado = valorUnitarioHomologado;
		ValorTotalHomologado = valorTotalHomologado;
		SituacaoCompraItemResultadoNome = situacaoCompraItemResultadoNome;
		DataResultado = dataResultado;
		DataAtualizacao = dataAtualizacao;
	}

	public virtual Int64 IdentificadorDoItemDaCompra { get; protected set; }
	public virtual String? NiFornecedor { get; protected set; }
	public virtual String? NomeRazaoSocialFornecedor { get; protected set; }
	public virtual Decimal? QuantidadeHomologada { get; protected set; }
	public virtual Decimal? ValorUnitarioHomologado { get; protected set; }
	public virtual Decimal? ValorTotalHomologado { get; protected set; }
	public virtual String? SituacaoCompraItemResultadoNome { get; protected set; }
	public virtual DateTime? DataResultado { get; protected set; }
	public virtual DateTime? DataAtualizacao { get; protected set; }
	public virtual DateTime CriadoEm { get; protected set; }
	public virtual DateTime AtualizadoEm { get; protected set; }

	public virtual ItemDaCompra? ItemDaCompra { get; protected set; }
}
