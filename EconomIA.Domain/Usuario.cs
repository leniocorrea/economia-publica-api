using System;
using EconomIA.Common.Domain;

namespace EconomIA.Domain;

public class Usuario : Aggregate {
	protected Usuario() {
		IdentificadorExterno = Guid.Empty;
		Nome = null!;
		Email = null!;
		SenhaHash = null!;
		Perfil = Perfis.Administrador;
	}

	public Usuario(
		Int64 id,
		Guid identificadorExterno,
		String nome,
		String email,
		String senhaHash,
		String perfil,
		Boolean ativo,
		DateTime? ultimoAcesso,
		DateTime criadoEm,
		DateTime atualizadoEm) : base(id) {
		IdentificadorExterno = identificadorExterno;
		Nome = nome;
		Email = email;
		SenhaHash = senhaHash;
		Perfil = perfil;
		Ativo = ativo;
		UltimoAcesso = ultimoAcesso;
		CriadoEm = criadoEm;
		AtualizadoEm = atualizadoEm;
	}

	public static Usuario Criar(String nome, String email, String senhaHash, String perfil = Perfis.Administrador) {
		return new Usuario(
			id: 0,
			identificadorExterno: Guid.NewGuid(),
			nome: nome,
			email: email,
			senhaHash: senhaHash,
			perfil: perfil,
			ativo: true,
			ultimoAcesso: null,
			criadoEm: DateTime.UtcNow,
			atualizadoEm: DateTime.UtcNow
		);
	}

	public virtual Guid IdentificadorExterno { get; protected set; }
	public virtual String Nome { get; protected set; }
	public virtual String Email { get; protected set; }
	public virtual String SenhaHash { get; protected set; }
	public virtual String Perfil { get; protected set; }
	public virtual Boolean Ativo { get; protected set; }
	public virtual DateTime? UltimoAcesso { get; protected set; }
	public virtual DateTime CriadoEm { get; protected set; }
	public virtual DateTime AtualizadoEm { get; protected set; }

	public void AtualizarNome(String nome) {
		Nome = nome;
		AtualizadoEm = DateTime.UtcNow;
	}

	public void AtualizarEmail(String email) {
		Email = email;
		AtualizadoEm = DateTime.UtcNow;
	}

	public void AtualizarSenha(String senhaHash) {
		SenhaHash = senhaHash;
		AtualizadoEm = DateTime.UtcNow;
	}

	public void AtualizarPerfil(String perfil) {
		Perfil = perfil;
		AtualizadoEm = DateTime.UtcNow;
	}

	public void Ativar() {
		Ativo = true;
		AtualizadoEm = DateTime.UtcNow;
	}

	public void Desativar() {
		Ativo = false;
		AtualizadoEm = DateTime.UtcNow;
	}

	public void RegistrarAcesso() {
		UltimoAcesso = DateTime.UtcNow;
	}
}

public static class Perfis {
	public const String Administrador = "administrador";
}
