using CSharpFunctionalExtensions;
using EconomIA.Common.Results;
using MediatR;

namespace EconomIA.Application.Commands;

public interface ICommand : IRequest<UnitResult<HandlerResultError>>;

public interface ICommand<TResponse> : IRequest<Result<TResponse, HandlerResultError>>;
