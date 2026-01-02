-- Script para marcar a migration baseline como aplicada
-- Executar APENAS em bancos já existentes (não em novos deploys)

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260102140502_BaselineInicial', '10.0.0-preview.7.25380.108')
ON CONFLICT ("MigrationId") DO NOTHING;
