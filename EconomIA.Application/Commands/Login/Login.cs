using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Common.Results;
using EconomIA.Domain.Repositories;
using EconomIA.Domain.Results;

namespace EconomIA.Application.Commands.Login;

public static class Login {
	public record Command(String Email, String Senha) : ICommand<Response>;

	public record Response(
		Guid UsuarioId,
		String Nome,
		String Email,
		String Perfil
	);

	public class Handler(IUsuariosReader usuariosReader) : CommandHandler<Command, Response> {
		private Func<String, String, Boolean>? validarSenha;

		public void SetValidarSenha(Func<String, String, Boolean> func) {
			validarSenha = func;
		}

		public override async Task<Result<Response, HandlerResultError>> Handle(Command command, CancellationToken cancellationToken = default) {
			if (String.IsNullOrWhiteSpace(command.Email)) {
				return Failure<Response>(EconomIAErrorCodes.ArgumentNotProvided, "Email é obrigatório.");
			}

			if (String.IsNullOrWhiteSpace(command.Senha)) {
				return Failure<Response>(EconomIAErrorCodes.ArgumentNotProvided, "Senha é obrigatória.");
			}

			var usuarioResult = await usuariosReader.Find(UsuariosSpecifications.WithEmail(command.Email), cancellationToken);

			if (usuarioResult.IsFailure) {
				return Failure<Response>(EconomIAErrorCodes.InvalidCredentials, "Email ou senha inválidos.");
			}

			var usuario = usuarioResult.Value;

			if (!usuario.Ativo) {
				return Failure<Response>(EconomIAErrorCodes.UsuarioInativo, "Usuário está inativo.");
			}

			if (validarSenha is not null && !validarSenha(command.Senha, usuario.SenhaHash)) {
				return Failure<Response>(EconomIAErrorCodes.InvalidCredentials, "Email ou senha inválidos.");
			}

			usuario.RegistrarAcesso();

			return Success(new Response(
				usuario.IdentificadorExterno,
				usuario.Nome,
				usuario.Email,
				usuario.Perfil
			));
		}
	}
}
