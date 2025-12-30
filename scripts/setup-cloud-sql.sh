#!/bin/bash
set -e

echo "🚀 Iniciando setup do Cloud SQL..."

# Habilitar Cloud SQL Admin API
gcloud services enable sqladmin.googleapis.com --project=$PROJECT_ID

# Criar instância Cloud SQL
INSTANCE_NAME="economia-postgres"
REGION="us-central1"

echo "📦 Criando instância Cloud SQL (PostgreSQL)..."
gcloud sql instances create $INSTANCE_NAME \
  --database-version=POSTGRES_15 \
    --tier=db-f1-micro \
      --region=$REGION \
        --project=$PROJECT_ID \
          --availability-type=ZONAL \
            --enable-bin-log=false || echo "Instância já existe, continuando..."

            # Aguardar disponibilidade da instância
            echo "⏳ Aguardando instância ficar disponível..."
            gcloud sql instances wait-until-ready $INSTANCE_NAME \
              --project=$PROJECT_ID

              # Criar banco de dados
              echo "🗄️ Criando banco de dados..."
              gcloud sql databases create $DB_NAME \
                --instance=$INSTANCE_NAME \
                  --project=$PROJECT_ID || echo "Banco já existe, continuando..."

                  # Criar usuário do banco
                  echo "👤 Criando usuário do banco..."
                  gcloud sql users create $DB_USER \
                    --instance=$INSTANCE_NAME \
                      --password=$DB_PASSWORD \
                        --project=$PROJECT_ID || echo "Usuário já existe, atualizando senha..."

                        # Atualizar instância para permitir conexões públicas
                        echo "🔓 Configurando acesso público..."
                        gcloud sql instances patch $INSTANCE_NAME \
                          --require-ssl=false \
                            --database-flags=cloudsql_iam_authentication=off \
                              --project=$PROJECT_ID \
                                --async

                                echo "✅ Setup do Cloud SQL concluído!"
                                echo "Instância: $INSTANCE_NAME"
                                echo "Banco: $DB_NAME"
                                echo "Usuário: $DB_USER"
