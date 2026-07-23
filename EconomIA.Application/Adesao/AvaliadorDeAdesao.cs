using System;
using System.Collections.Generic;
using System.Linq;

namespace EconomIA.Application.Adesao;

public record AtaParaAvaliacao(
	String NumeroControlePncpAta,
	Boolean Cancelado,
	DateTime? VigenciaFim,
	Boolean? PossibilidadeAdesao = null);

public record SituacaoDaAdesao(
	String Situacao,
	Boolean Disponivel,
	DateTime? VigenciaFim,
	Int32? DiasRestantes,
	String? NumeroControlePncpAta,
	Decimal? SaldoDisponivel);

public static class AvaliadorDeAdesao {
	public const String Permitida = "permitida";
	public const String NaoPermitida = "nao_permitida";
	public const String NaoInformada = "nao_informada";
	public const String SemAta = "sem_ata";

	private static readonly SituacaoDaAdesao SemAdesao =
		new SituacaoDaAdesao(SemAta, false, null, null, null, null);

	public static SituacaoDaAdesao Avaliar(
		IReadOnlyCollection<AtaParaAvaliacao> atasDaCompra,
		Boolean itemTemResultado,
		DateOnly dataReferencia) {

		if (!itemTemResultado) {
			return SemAdesao;
		}

		var atasVigentes = atasDaCompra
			.Where(a => !a.Cancelado && a.VigenciaFim.HasValue && DateOnly.FromDateTime(a.VigenciaFim.Value) >= dataReferencia)
			.OrderByDescending(a => Prioridade(a.PossibilidadeAdesao))
			.ThenByDescending(a => a.VigenciaFim!.Value)
			.ToArray();

		if (atasVigentes.Length == 0) {
			return SemAdesao;
		}

		var melhorAta = atasVigentes[0];
		var vigenciaFim = melhorAta.VigenciaFim!.Value;
		var diasRestantes = DateOnly.FromDateTime(vigenciaFim).DayNumber - dataReferencia.DayNumber;

		var situacao = melhorAta.PossibilidadeAdesao switch {
			true => Permitida,
			false => NaoPermitida,
			_ => NaoInformada
		};

		return new SituacaoDaAdesao(
			situacao,
			melhorAta.PossibilidadeAdesao == true,
			vigenciaFim,
			diasRestantes,
			melhorAta.NumeroControlePncpAta,
			null);
	}

	private static Int32 Prioridade(Boolean? possibilidadeAdesao) {
		return possibilidadeAdesao switch {
			true => 2,
			false => 0,
			_ => 1
		};
	}
}
