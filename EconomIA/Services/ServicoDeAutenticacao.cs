using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EconomIA.Configuration;
using EconomIA.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EconomIA.Services;

public interface IServicoDeAutenticacao {
	String GerarToken(Usuario usuario);
	Boolean ValidarSenha(String senha, String senhaHash);
	String CriptografarSenha(String senha);
}

public class ServicoDeAutenticacao : IServicoDeAutenticacao {
	private readonly JwtConfiguration configuracao;

	public ServicoDeAutenticacao(IOptions<JwtConfiguration> options) {
		configuracao = options.Value;
	}

	public String GerarToken(Usuario usuario) {
		var claims = new List<Claim> {
			new(ClaimTypes.NameIdentifier, usuario.IdentificadorExterno.ToString()),
			new(ClaimTypes.Name, usuario.Nome),
			new(ClaimTypes.Email, usuario.Email),
			new(ClaimTypes.Role, usuario.Perfil),
		};

		var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracao.Secret));
		var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
		var expiracao = DateTime.UtcNow.AddHours(configuracao.ExpiracaoEmHoras);

		var token = new JwtSecurityToken(
			issuer: configuracao.Issuer,
			audience: configuracao.Audience,
			claims: claims,
			expires: expiracao,
			signingCredentials: credenciais
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public Boolean ValidarSenha(String senha, String senhaHash) {
		return BCrypt.Net.BCrypt.Verify(senha, senhaHash);
	}

	public String CriptografarSenha(String senha) {
		return BCrypt.Net.BCrypt.HashPassword(senha, 11);
	}
}
