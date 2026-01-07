using System;
using System.ComponentModel.DataAnnotations;

namespace EconomIA.Configuration;

public sealed class JwtConfiguration {
	public const String SectionName = "Jwt";

	[Required]
	public String Issuer { get; set; } = "EconomIA";

	[Required]
	public String Audience { get; set; } = String.Empty;

	[Required]
	public String Secret { get; set; } = String.Empty;

	public Int32 ExpiracaoEmHoras { get; set; } = 24;
}
