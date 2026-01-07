using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EconomIA.Adapters.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarParametrosExecucao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "parametros",
                table: "execucao_carga",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parametros",
                table: "execucao_carga");
        }
    }
}
