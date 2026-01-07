using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using EconomIA.Application.Extensions;
using EconomIA.Common.Results;
using EconomIA.Domain;
using EconomIA.Domain.Repositories;
using EconomIA.Domain.Results;

namespace EconomIA.Application.Commands.CriarUsuario;

public static class CriarUsuario {
	public record Command(String Nome, String Email, String Senha, String Perfil = Perfis.Administrador) : ICommand<Response>;

	public record Response(
		Guid Id,
		String Nome,
		String Email,
		String Perfil,
		DateTime CriadoEm
	);

	public class Handler(IUsuariosReader usuariosReader, IUsuarios usuarios) : CommandHandler<Command, Response> {
		private Func<String, String>? criptografarSenha;

		public void SetCriptografarSenha(Func<String, String> func) {
			criptografarSenha = func;
		}

		public override async Task<Result<Response, HandlerResultError>> Handle(Command command, CancellationToken cancellationToken = default) {
			if (String.IsNullOrWhiteSpace(command.Nome)) {
				return Failure<Response>(EconomIAErrorCodes.ArgumentNotProvided, "Nome é obrigatório.");
			}

			if (String.IsNullOrWhiteSpace(command.Email)) {
				return Failure<Response>(EconomIAErrorCodes.ArgumentNotProvided, "Email é obrigatório.");
			}

			if (String.IsNullOrWhiteSpace(command.Senha)) {
				return Failure<Response>(EconomIAErrorCodes.ArgumentNotProvided, "Senha é obrigatória.");
			}

			if (command.Senha.Length < 6) {
				return Failure<Response>(EconomIAErrorCodes.InvalidUsuarioRequest, "Senha deve ter pelo menos 6 caracteres.");
			}

			var existenteResult = await usuariosReader.Find(UsuariosSpecifications.WithEmail(command.Email), cancellationToken);

			if (existenteResult.IsSuccess) {
				return Failure<Response>(EconomIAErrorCodes.UsuarioAlreadyExists, "Já existe um usuário com este email.");
			}

			var senhaHash = criptografarSenha is not null ? criptografarSenha(command.Senha) : command.Senha;
			var usuario = Usuario.Criar(command.Nome, command.Email, senhaHash, command.Perfil);

			var addResult = await usuarios.Add(usuario, cancellationToken);

			if (addResult.IsFailure) {
				return Failure<Response>(addResult.Error.ToUsuarioError());
			}

			return Success(new Response(
				usuario.IdentificadorExterno,
				usuario.Nome,
				usuario.Email,
				usuario.Perfil,
				usuario.CriadoEm
			));
		}
	}
}
