using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EconomIA.Adapters.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarModoCargaAutomatica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "modo_carga_automatica",
                table: "configuracao_carga",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "brasil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modo_carga_automatica",
                table: "configuracao_carga");
        }
    }
}
