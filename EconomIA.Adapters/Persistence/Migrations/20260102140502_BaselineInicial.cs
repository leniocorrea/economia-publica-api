using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EconomIA.Adapters.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BaselineInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orgao",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    razao_social = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    codigo_natureza_juridica = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    descricao_natureza_juridica = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    poder_id = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    esfera_id = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    situacao_cadastral = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    motivo_situacao_cadastral = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    data_situacao_cadastral = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_validacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    validado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    data_inclusao_pncp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao_pncp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status_ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    justificativa_atualizacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orgao", x => x.identificador);
                });

            migrationBuilder.CreateTable(
                name: "ata",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador_do_orgao = table.Column<long>(type: "bigint", nullable: false),
                    numero_controle_pncp_ata = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    numero_controle_pncp_compra = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    numero_ata_registro_preco = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ano_ata = table.Column<int>(type: "integer", nullable: false),
                    objeto_contratacao = table.Column<string>(type: "text", nullable: true),
                    cancelado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    data_cancelamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_assinatura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    vigencia_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    vigencia_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_publicacao_pncp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_inclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao_global = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ata", x => x.identificador);
                    table.ForeignKey(
                        name: "fk_ata_IdentificadorDoOrgao_to_orgao_Id",
                        column: x => x.identificador_do_orgao,
                        principalTable: "orgao",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "compra",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador_do_orgao = table.Column<long>(type: "bigint", nullable: false),
                    numero_controle_pncp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ano_compra = table.Column<int>(type: "integer", nullable: false),
                    sequencial_compra = table.Column<int>(type: "integer", nullable: false),
                    modalidade_identificador = table.Column<int>(type: "integer", nullable: false),
                    modalidade_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    objeto_compra = table.Column<string>(type: "text", nullable: true),
                    valor_total_estimado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    valor_total_homologado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    situacao_compra_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_inclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_abertura_proposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_encerramento_proposta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    amparo_legal_nome = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    modo_disputa_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    link_pncp = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    data_atualizacao_global = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    itens_carregados = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_compra", x => x.identificador);
                    table.ForeignKey(
                        name: "fk_compra_IdentificadorDoOrgao_to_orgao_Id",
                        column: x => x.identificador_do_orgao,
                        principalTable: "orgao",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contrato",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador_do_orgao = table.Column<long>(type: "bigint", nullable: false),
                    numero_controle_pncp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    numero_controle_pncp_compra = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ano_contrato = table.Column<int>(type: "integer", nullable: false),
                    sequencial_contrato = table.Column<int>(type: "integer", nullable: false),
                    numero_contrato_empenho = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    processo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    objeto_contrato = table.Column<string>(type: "text", nullable: true),
                    tipo_contrato_id = table.Column<int>(type: "integer", nullable: true),
                    tipo_contrato_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    categoria_processo_id = table.Column<int>(type: "integer", nullable: true),
                    categoria_processo_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ni_fornecedor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nome_razao_social_fornecedor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipo_pessoa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    valor_inicial = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    valor_global = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    valor_parcela = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    valor_acumulado = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    numero_parcelas = table.Column<int>(type: "integer", nullable: true),
                    data_assinatura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_vigencia_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_vigencia_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_publicacao_pncp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao_global = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    receita = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    informacao_complementar = table.Column<string>(type: "text", nullable: true),
                    usuario_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contrato", x => x.identificador);
                    table.ForeignKey(
                        name: "fk_contrato_IdentificadorDoOrgao_to_orgao_Id",
                        column: x => x.identificador_do_orgao,
                        principalTable: "orgao",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "unidade",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador_do_orgao = table.Column<long>(type: "bigint", nullable: false),
                    codigo_unidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nome_unidade = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    municipio_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    municipio_codigo_ibge = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    uf_sigla = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    uf_nome = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status_ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    data_inclusao_pncp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao_pncp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    justificativa_atualizacao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_unidade", x => x.identificador);
                    table.ForeignKey(
                        name: "fk_unidade_IdentificadorDoOrgao_to_orgao_Id",
                        column: x => x.identificador_do_orgao,
                        principalTable: "orgao",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item_da_compra",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador_da_compra = table.Column<long>(type: "bigint", nullable: false),
                    numero_item = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    quantidade = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    unidade_medida = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    valor_unitario_estimado = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    valor_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    criterio_julgamento_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    situacao_compra_item_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tem_resultado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_da_compra", x => x.identificador);
                    table.ForeignKey(
                        name: "fk_item_da_compra_IdentificadorDaCompra_to_compra_Id",
                        column: x => x.identificador_da_compra,
                        principalTable: "compra",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resultado_do_item",
                columns: table => new
                {
                    identificador = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador_do_item_da_compra = table.Column<long>(type: "bigint", nullable: false),
                    ni_fornecedor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nome_razao_social_fornecedor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    quantidade_homologada = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    valor_unitario_homologado = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    valor_total_homologado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    situacao_compra_item_resultado_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    data_resultado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resultado_do_item", x => x.identificador);
                    table.ForeignKey(
                        name: "fk_resultado_do_item_IdentificadorDoItemDaCompra_to_item_da_compra_Id",
                        column: x => x.identificador_do_item_da_compra,
                        principalTable: "item_da_compra",
                        principalColumn: "identificador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ata_AnoAta",
                table: "ata",
                column: "ano_ata");

            migrationBuilder.CreateIndex(
                name: "ix_ata_IdentificadorDoOrgao",
                table: "ata",
                column: "identificador_do_orgao");

            migrationBuilder.CreateIndex(
                name: "ix_ata_NumeroControlePncpCompra",
                table: "ata",
                column: "numero_controle_pncp_compra");

            migrationBuilder.CreateIndex(
                name: "un_ata_NumeroControlePncpAta",
                table: "ata",
                column: "numero_controle_pncp_ata",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "un_compra_IdentificadorDoOrgao_AnoCompra_SequencialCompra",
                table: "compra",
                columns: new[] { "identificador_do_orgao", "ano_compra", "sequencial_compra" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contrato_AnoContrato",
                table: "contrato",
                column: "ano_contrato");

            migrationBuilder.CreateIndex(
                name: "ix_contrato_IdentificadorDoOrgao",
                table: "contrato",
                column: "identificador_do_orgao");

            migrationBuilder.CreateIndex(
                name: "ix_contrato_NiFornecedor",
                table: "contrato",
                column: "ni_fornecedor");

            migrationBuilder.CreateIndex(
                name: "un_contrato_NumeroControlePncp",
                table: "contrato",
                column: "numero_controle_pncp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "un_item_da_compra_IdentificadorDaCompra_NumeroItem",
                table: "item_da_compra",
                columns: new[] { "identificador_da_compra", "numero_item" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "un_orgao_Cnpj",
                table: "orgao",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "un_resultado_do_item_IdentificadorDoItemDaCompra_NiFornecedor",
                table: "resultado_do_item",
                columns: new[] { "identificador_do_item_da_compra", "ni_fornecedor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_unidade_MunicipioCodigoIbge",
                table: "unidade",
                column: "municipio_codigo_ibge");

            migrationBuilder.CreateIndex(
                name: "ix_unidade_UfSigla",
                table: "unidade",
                column: "uf_sigla");

            migrationBuilder.CreateIndex(
                name: "un_unidade_IdentificadorDoOrgao_CodigoUnidade",
                table: "unidade",
                columns: new[] { "identificador_do_orgao", "codigo_unidade" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ata");

            migrationBuilder.DropTable(
                name: "contrato");

            migrationBuilder.DropTable(
                name: "resultado_do_item");

            migrationBuilder.DropTable(
                name: "unidade");

            migrationBuilder.DropTable(
                name: "item_da_compra");

            migrationBuilder.DropTable(
                name: "compra");

            migrationBuilder.DropTable(
                name: "orgao");
        }
    }
}
