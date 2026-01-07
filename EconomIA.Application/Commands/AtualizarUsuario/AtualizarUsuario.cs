using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Application.Extensions;
using EconomIA.Common.Results;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using EconomIA.Domain.Results;

namespace EconomIA.Application.Commands.AtualizarUsuario;

public static class AtualizarUsuario {
	public record Command(
		Guid Id,
		String? Nome,
		String? Email,
		String? Senha,
		String? Perfil,
		Boolean? Ativo) : ICommand<Response>;

	public record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		Boolean Ativo,
		DateTime AtualizadoEm
	);

	public class Handler(IUsuariosReader usuariosReader, IUsuarios usuarios) : CommandHandler<Command, Response> {
		private Func<String, String>? criptografarSenha;

		public void SetCriptografarSenha(Func<String, String> func) {
			criptografarSenha = func;
		}

		public override async Task<Result<Response, HandlerResultError>> Handle(Command command, CancellationToken cancellationToken = default) {
			var usuarioResult = await usuariosReader.Find(UsuariosSpecifications.WithIdentificadorExterno(command.Id), cancellationToken);

			if (usuarioResult.IsFailure) {
				return Failure<Response>(EconomIAErrorCodes.UsuarioNotFound, "Usuário não encontrado.");
			}

			var usuario = usuarioResult.Value;

			if (!String.IsNullOrWhiteSpace(command.Nome)) {
				usuario.AtualizarNome(command.Nome);
			}

			if (!String.IsNullOrWhiteSpace(command.Email)) {
				if (command.Email != usuario.Email) {
					var existenteResult = await usuariosReader.Find(UsuariosSpecifications.WithEmail(command.Email), cancellationToken);

					if (existenteResult.IsSuccess) {
						return Failure<Response>(EconomIAErrorCodes.UsuarioAlreadyExists, "Já existe um usuário com este email.");
					}
				}

				usuario.AtualizarEmail(command.Email);
			}

			if (!String.IsNullOrWhiteSpace(command.Senha)) {
				if (command.Senha.Length < 6) {
					return Failure<Response>(EconomIAErrorCodes.InvalidUsuarioRequest, "Senha deve ter pelo menos 6 caracteres.");
				}

				var senhaHash = criptografarSenha is not null ? criptografarSenha(command.Senha) : command.Senha;
				usuario.AtualizarSenha(senhaHash);
			}

			if (!String.IsNullOrWhiteSpace(command.Perfil)) {
				usuario.AtualizarPerfil(command.Perfil);
			}

			if (command.Ativo.HasValue) {
				if (command.Ativo.Value) {
					usuario.Ativar();
				} else {
					usuario.Desativar();
				}
			}

			var updateResult = await usuarios.Update(usuario, cancellationToken);

			if (updateResult.IsFailure) {
				return Failure<Response>(updateResult.Error.ToUsuarioError());
			}

			return Success(new Response(
				usuario.IdentificadorExterno,
				usuario.Nome,
				usuario.Email,
				usuario.Perfil,
				usuario.Ativo,
				usuario.AtualizadoEm
			));
		}
	}
}
