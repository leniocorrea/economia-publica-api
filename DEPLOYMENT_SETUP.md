# 🚀 Deployment Setup - Economia Publica

## Arquitetura de CI/CD

Este projeto utiliza GitHub Actions para automatizar o deploy na Google Cloud Run com suporte a PostgreSQL e Elasticsearch.

### Componentes

- **GitHub Actions**: Pipeline de CI/CD automático
- - **Google Cloud Run**: Hospedagem da aplicação .NET
  - - **Cloud SQL**: Banco de dados PostgreSQL gerenciado
    - - **Elasticsearch**: Mecanismo de busca (em Compute Engine)
      - - **Artifact Registry**: Registro de imagens Docker
       
        - ## 📋 Configuração Necessária
       
        - ### 1. Criar Workload Identity Federation
       
        - Antes de configurar os Secrets, você precisa configurar a autenticação entre GitHub e Google Cloud.
       
        - ```bash
          # Habilitar APIs necessárias
          gcloud services enable iamcredentials.googleapis.com
          gcloud services enable cloudresourcemanager.googleapis.com
          gcloud services enable sts.googleapis.com

          # Criar Identity Provider
          PROJECT_ID="economia-publica-482719"
          PROVIDER_ID="github"
          LOCATION="global"

          gcloud iam workload-identity-pools create $PROVIDER_ID \
            --project=$PROJECT_ID \
            --location=$LOCATION \
            --display-name="GitHub"

          # Obter o ARN do pool
          WORKLOAD_IDENTITY_POOL_ID=$(gcloud iam workload-identity-pools describe $PROVIDER_ID \
            --project=$PROJECT_ID \
            --location=$LOCATION \
            --format='value(name)')

          # Criar provider
          gcloud iam workload-identity-pools providers create-oidc "github" \
            --project=$PROJECT_ID \
            --location=$LOCATION \
            --workload-identity-pool=$PROVIDER_ID \
            --display-name="GitHub" \
            --attribute-mapping="google.subject=assertion.sub,assertion.aud=assertion.aud,assertion.repository=assertion.repository" \
            --issuer-uri="https://token.actions.githubusercontent.com" \
            --attribute-condition="assertion.aud == 'economia-publica-482719' && assertion.repository == 'leniocorrea/economia-publica'"
          ```

          ### 2. Configurar Service Account

          ```bash
          # Criar service account
          SA_NAME="economia-api-sa"

          gcloud iam service-accounts create $SA_NAME \
            --project=$PROJECT_ID \
            --display-name="Economia API Service Account"

          SA_EMAIL="$SA_NAME@$PROJECT_ID.iam.gserviceaccount.com"

          # Conceder permissões necessárias
          gcloud projects add-iam-policy-binding $PROJECT_ID \
            --member="serviceAccount:$SA_EMAIL" \
            --role="roles/run.admin"

          gcloud projects add-iam-policy-binding $PROJECT_ID \
            --member="serviceAccount:$SA_EMAIL" \
            --role="roles/cloudsql.admin"

          gcloud projects add-iam-policy-binding $PROJECT_ID \
            --member="serviceAccount:$SA_EMAIL" \
            --role="roles/artifactregistry.admin"

          gcloud projects add-iam-policy-binding $PROJECT_ID \
            --member="serviceAccount:$SA_EMAIL" \
            --role="roles/compute.instanceAdmin.v1"

          # Permitir que o GitHub use este service account
          gcloud iam service-accounts add-iam-policy-binding $SA_EMAIL \
            --project=$PROJECT_ID \
            --role="roles/iam.workloadIdentityUser" \
            --member="principalSet://iam.googleapis.com/$WORKLOAD_IDENTITY_POOL_ID/attribute.repository/leniocorrea/economia-publica"
          ```

          ### 3. Configurar Secrets no GitHub

          Acesse: `https://github.com/leniocorrea/economia-publica/settings/secrets/actions`

          Adicione os seguintes Secrets:

          #### Autenticação Google Cloud

          ```
          WORKLOAD_IDENTITY_PROVIDER=projects/PROJECT_NUMBER/locations/global/workloadIdentityPools/github/providers/github
          SERVICE_ACCOUNT_EMAIL=economia-api-sa@economia-publica-482719.iam.gserviceaccount.com
          ```

          **Nota**: Substitua `PROJECT_NUMBER` pelo número do seu projeto (encontre em: https://console.cloud.google.com/home/dashboard)

          #### Banco de Dados

          ```
          CLOUD_SQL_INSTANCE=economia-publica-482719:us-central1:economia-postgres
          DB_USER=economia_user
          DB_PASSWORD=<gere-uma-senha-forte>
          ```

          #### Elasticsearch

          ```
          ELASTICSEARCH_HOST=<IP-externo-da-instancia>
          ELASTICSEARCH_PASSWORD=<gere-uma-senha-forte>
          ```

          ## 🔑 Gerando Senhas Seguras

          ```bash
          # Gerar senha segura (Linux/Mac)
          openssl rand -base64 32

          # Ou Python
          python3 -c "import secrets; print(secrets.token_urlsafe(32))"
          ```

          ## 🚀 Primeiro Deploy

          1. Todos os Secrets configurados? ✓
          2. 2. Faça um push em `master`:
            
             3. ```bash
                git add .
                git commit -m "Configure CI/CD pipeline with Cloud Run deployment"
                git push origin master
                ```

                3. O workflow será acionado automaticamente:
                4.    - Construirá a imagem Docker
                      -    - Fará push para Artifact Registry
                           -    - Na primeira execução:
                                -      - Criará instância Cloud SQL com PostgreSQL
                                -       - Criará instância Compute Engine com Elasticsearch
                                -      - Fará deploy na Cloud Run
                                -     - Gerará URL pública do serviço
                            
                                - ## 📊 Monitoramento
                            
                                - ### Ver logs do GitHub Actions
                            
                                - https://github.com/leniocorrea/economia-publica/actions
                            
                                - ### Ver logs da aplicação
                            
                                - ```bash
                                  gcloud run logs read economia-api --limit=50 --project=economia-publica-482719
                                  ```

                                  ### Verificar instâncias Cloud SQL

                                  ```bash
                                  gcloud sql instances list --project=economia-publica-482719
                                  ```

                                  ### Verificar instâncias Compute Engine

                                  ```bash
                                  gcloud compute instances list --project=economia-publica-482719
                                  ```

                                  ## 🔗 Links Úteis

                                  - [Google Cloud Console](https://console.cloud.google.com)
                                  - - [Seu repositório no GitHub](https://github.com/leniocorrea/economia-publica)
                                    - - [GitHub Actions Docs](https://docs.github.com/en/actions)
                                      - - [Google Cloud Run Docs](https://cloud.google.com/run/docs)
                                       
                                        - ## ❓ Troubleshooting
                                       
                                        - ### Erro: "Project has no billing account"
                                       
                                        - Solução: Configure uma conta de faturamento no Google Cloud Console
                                       
                                        - ### Erro: "Permission denied"
                                       
                                        - Solução: Verifique se o service account tem todas as roles necessárias
                                       
                                        - ### Elasticsearch não conecta
                                       
                                        - Solução: Verifique o IP externo e se o firewall permite porta 9200
