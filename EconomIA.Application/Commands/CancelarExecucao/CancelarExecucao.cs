using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Common.Results;
using EconomIA.Domain.Repositories;
using static EconomIA.Domain.Results.EconomIAErrorCodes;

namespace EconomIA.Application.Commands.CancelarExecucao;

public static class CancelarExecucao {
	public record Command(Int64 Id) : ICommand;

	public class Handler(IExecucoesCarga execucoesCarga) : CommandHandler<Command> {
		public override async Task<UnitResult<HandlerResultError>> Handle(Command command, CancellationToken cancellationToken = default) {
			var result = await execucoesCarga.Retrieve(command.Id, cancellationToken);

			if (result.IsFailure) {
				return Failure(ExecucaoCargaNotFound, $"Execução {command.Id} não encontrada.");
			}

			var execucao = result.Value;

			if (!execucao.PodeCancelar()) {
				return Failure(InvalidExecucaoRequest, $"Execução {command.Id} não pode ser cancelada. Status atual: {execucao.Status}.");
			}

			execucao.Cancelar();

			await execucoesCarga.Update(execucao, cancellationToken);

			return Success();
		}
	}
}
