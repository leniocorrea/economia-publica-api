using CSharpFunctionalExtensions;
using EconomIA.Common.Results;
using MediatR;

namespace EconomIA.Application.Commands;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, UnitResult<HandlerResultError>> where TCommand : ICommand;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse, HandlerResultError>> where TCommand : ICommand<TResponse>;
