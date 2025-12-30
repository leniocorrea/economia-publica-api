#!/bin/bash
set -e

echo "🚀 Iniciando setup do Elasticsearch..."

# Criar instância Compute Engine com Elasticsearch
INSTANCE_NAME="elasticsearch-server"
ZONE="us-central1-a"
MACHINE_TYPE="e2-medium"
IMAGE_PROJECT="debian-cloud"
IMAGE_FAMILY="debian-11"

echo "🖥️ Criando instância Compute Engine..."
gcloud compute instances create $INSTANCE_NAME \
  --zone=$ZONE \
    --machine-type=$MACHINE_TYPE \
      --image-project=$IMAGE_PROJECT \
        --image-family=$IMAGE_FAMILY \
          --scopes=https://www.googleapis.com/auth/cloud-platform \
            --metadata-from-file startup-script=/dev/stdin \
              --project=$PROJECT_ID << 'EOF'
              #!/bin/bash
              sudo apt-get update
              sudo apt-get install -y apt-transport-https curl gnupg

              # Adicionar repositório Elasticsearch
              curl -fsSL https://artifacts.elastic.co/GPG-KEY-elasticsearch | sudo gpg --dearmor -o /usr/share/keyrings/elasticsearch-archive-keyring.gpg
              echo "deb [signed-by=/usr/share/keyrings/elasticsearch-archive-keyring.gpg] https://artifacts.elastic.co/packages/8.x/apt stable main" | sudo tee /etc/apt/sources.list.d/elastic-8.x.list

              # Instalar Elasticsearch
              sudo apt-get update
              sudo apt-get install -y elasticsearch

              # Configurar Elasticsearch para escutar em 0.0.0.0
              sudo sed -i 's/#network.host: 192.168.1.1/network.host: 0.0.0.0/' /etc/elasticsearch/elasticsearch.yml
              sudo sed -i 's/#http.port: 9200/http.port: 9200/' /etc/elasticsearch/elasticsearch.yml

              # Iniciar Elasticsearch
              sudo systemctl daemon-reload
              sudo systemctl enable elasticsearch
              sudo systemctl start elasticsearch

              echo "✅ Elasticsearch instalado com sucesso!"
              EOF

              echo "⏳ Aguardando instância ficar pronta..."
              sleep 60

              # Obter IP externo da instância
              EXTERNAL_IP=$(gcloud compute instances describe $INSTANCE_NAME \
                --zone=$ZONE \
                  --format='get(networkInterfaces[0].accessConfigs[0].natIP)' \
                    --project=$PROJECT_ID)

                    echo "✅ Setup do Elasticsearch concluído!"
                    echo "Instância: $INSTANCE_NAME"
                    echo "Endereço IP: $EXTERNAL_IP"
                    echo "Elasticsearch disponível em: http://$EXTERNAL_IP:9200"
