using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Common.Results;
using EconomIA.Domain.Results;

namespace EconomIA.Application.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand {
	public abstract Task<UnitResult<HandlerResultError>> Handle(TCommand command, CancellationToken cancellationToken = default);

	protected static UnitResult<HandlerResultError> Success() => UnitResult.Success<HandlerResultError>();

	protected static UnitResult<HandlerResultError> Failure(EconomIAErrorCodes code, String message, String? hint = null) =>
		UnitResult.Failure<HandlerResultError>(new EconomIAApplicationError(code, message, hint));

	protected static UnitResult<HandlerResultError> Failure(HandlerResultError error) => UnitResult.Failure(error);
}

public abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse> {
	public abstract Task<Result<TResponse, HandlerResultError>> Handle(TCommand command, CancellationToken cancellationToken = default);

	protected static Result<TResponse, HandlerResultError> Success(TResponse value) =>
		Result.Success<TResponse, HandlerResultError>(value);

	protected static UnitResult<HandlerResultError> Failure(EconomIAErrorCodes code, String message, String? hint = null) =>
		UnitResult.Failure<HandlerResultError>(new EconomIAApplicationError(code, message, hint));

	protected static Result<TResponse, HandlerResultError> Failure<T>(EconomIAErrorCodes code, String message, String? hint = null) =>
		Result.Failure<TResponse, HandlerResultError>(new EconomIAApplicationError(code, message, hint));
}
