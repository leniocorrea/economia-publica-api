using System;
using EconomIA.Application.Adesao;
using FluentAssertions;
using Xunit;

namespace EconomIA.Application.Tests.Adesao;

public class AvaliadorDeAdesaoTests {
	private static readonly DateOnly Hoje = new DateOnly(2026, 7, 23);

	private static AtaParaAvaliacao Ata(
		String numero,
		DateTime? vigenciaFim,
		Boolean cancelado = false,
		Boolean? possibilidadeAdesao = null) {
		return new AtaParaAvaliacao(numero, cancelado, vigenciaFim, possibilidadeAdesao);
	}

	[Fact]
	public void item_sem_resultado_nao_tem_adesao() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 12, 31), possibilidadeAdesao: true) },
			itemTemResultado: false,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.SemAta);
		resultado.Disponivel.Should().BeFalse();
		resultado.NumeroControlePncpAta.Should().BeNull();
	}

	[Fact]
	public void compra_sem_ata_vigente_nao_tem_adesao() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 1, 1), possibilidadeAdesao: true) },
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.SemAta);
	}

	[Fact]
	public void ata_cancelada_e_ignorada() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 12, 31), cancelado: true, possibilidadeAdesao: true) },
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.SemAta);
	}

	[Fact]
	public void possibilidade_verdadeira_fica_disponivel() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 12, 31), possibilidadeAdesao: true) },
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.Permitida);
		resultado.Disponivel.Should().BeTrue();
		resultado.NumeroControlePncpAta.Should().Be("A");
		resultado.DiasRestantes.Should().Be(161);
	}

	[Fact]
	public void possibilidade_falsa_nao_fica_disponivel() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 12, 31), possibilidadeAdesao: false) },
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.NaoPermitida);
		resultado.Disponivel.Should().BeFalse();
	}

	[Fact]
	public void possibilidade_nula_fica_nao_informada() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 12, 31), possibilidadeAdesao: null) },
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.NaoInformada);
		resultado.Disponivel.Should().BeFalse();
		resultado.VigenciaFim.Should().Be(new DateTime(2026, 12, 31));
	}

	[Fact]
	public void escolhe_ata_que_permite_adesao_mesmo_com_vigencia_menor() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] {
				Ata("SemAdesao", new DateTime(2027, 12, 31), possibilidadeAdesao: false),
				Ata("ComAdesao", new DateTime(2026, 8, 31), possibilidadeAdesao: true)
			},
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.Permitida);
		resultado.Disponivel.Should().BeTrue();
		resultado.NumeroControlePncpAta.Should().Be("ComAdesao");
		resultado.VigenciaFim.Should().Be(new DateTime(2026, 8, 31));
	}

	[Fact]
	public void entre_atas_de_mesma_prioridade_escolhe_a_de_maior_vigencia() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] {
				Ata("Curta", new DateTime(2026, 9, 30), possibilidadeAdesao: true),
				Ata("Longa", new DateTime(2027, 3, 31), possibilidadeAdesao: true)
			},
			itemTemResultado: true,
			Hoje);

		resultado.NumeroControlePncpAta.Should().Be("Longa");
		resultado.VigenciaFim.Should().Be(new DateTime(2027, 3, 31));
	}

	[Fact]
	public void ata_que_vence_hoje_ainda_e_vigente() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 7, 23), possibilidadeAdesao: true) },
			itemTemResultado: true,
			Hoje);

		resultado.Situacao.Should().Be(AvaliadorDeAdesao.Permitida);
		resultado.DiasRestantes.Should().Be(0);
	}

	[Fact]
	public void saldo_disponivel_e_sempre_nulo() {
		var resultado = AvaliadorDeAdesao.Avaliar(
			new[] { Ata("A", new DateTime(2026, 12, 31), possibilidadeAdesao: true) },
			itemTemResultado: true,
			Hoje);

		resultado.SaldoDisponivel.Should().BeNull();
	}
}
