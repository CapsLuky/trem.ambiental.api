# API Processamento de Texto

API responsável pela extração massiva de textos de documentos via OCR, com capacidade de processar diretórios com gigabytes de arquivos, separando páginas e textos para armazenamento em banco de dados.

## Visão geral e objetivos
Construir um repositório de consulta para grandes volumes de documentos:
- Ler diretórios com muitos arquivos (em lote) e processar via OCR.
- Separar páginas, extrair texto e metadados relevantes.
- Persistir resultados no banco de dados para buscas e integrações.
- Disponibilizar endpoints para consulta e automações (busca por texto e metadados, paginação, status de processamento).

## Funcionalidades
Funcionalidades atuais (na Web API):
- Autenticação e autorização via JWT.
- Versionamento de API e documentação interativa (Swagger/OpenAPI).
- Camada de acesso a dados baseada em MySQL.

Funcionalidades alvo (OCR e consultas):
- Ingestão de documentos a partir de diretórios ou uploads.
- Processamento OCR massivo com suporte a paralelismo.
- Armazenamento de texto extraído e metadados por página/documento.
- Endpoints de busca e consulta (full-text e filtros por metadados).
- Monitoramento de filas e status de processamento.

## Arquitetura e camadas

O projeto segue uma arquitetura em camadas (Clean Architecture), organizada da seguinte forma:

```
API.Contratual.Admin/
├── 0.Presentation/
│   └── API.Contratual.WebApi          # Camada de apresentação (Controllers, Endpoints)
├── 1.Application/
│   └── API.Contratual.Application     # Lógica de aplicação e orquestração
├── 2.Dto/
│   └── API.Contratual.Dto             # Data Transfer Objects
├── 3.Domain/
│   └── API.Contratual.Domain          # Entidades, interfaces e regras de negócio
├── 4.Infrastructure/
│   ├── API.Contratual.Data.Mysql      # Acesso a dados (MySQL)
│   └── API.Contratual.Integration.Http # Integrações HTTP externas
├── 5.CrossCutting/
│   ├── API.Contratual.CrossCutting    # Utilitários, notificações, helpers
│   └── API.Contratual.IoC             # Injeção de dependências
└── 6.Tests/
    └── API.Contratual.Test            # Testes unitários e de integração
```

### Camadas

- **Presentation**: Exposição de endpoints REST para consumo da API
- **Application**: Orquestração de casos de uso e fluxos de negócio
- **Dto**: Objetos de transferência de dados entre camadas
- **Domain**: Núcleo da aplicação com entidades, interfaces e regras de negócio
- **Infrastructure**: Implementações de acesso a dados e integrações externas
- **CrossCutting**: Funcionalidades transversais (logging, notificações, helpers)
- **Tests**: Testes automatizados


## Fluxo de Processamento

1. **Configuração**: Define pasta de origem e configurações por empresa/filial
2. **Descoberta**: Busca recursiva de arquivos PDF na pasta configurada
3. **Validação**: Verifica tamanhos de caminhos e nomes de arquivos
4. **Seleção**: Identifica arquivos novos ou que precisam ser processados
5. **Cópia**: Cria backup dos arquivos originais (opcional)
6. **Registro**: Insere registros na tabela de arquivos
7. **Extração**: Processa cada página do PDF extraindo o texto via OCR
8. **Armazenamento**: Salva textos extraídos no banco de dados
9. **Pesquisa**: Executa pesquisas baseadas no dicionário de palavras
10. **Indexação**: Armazena resultados de pesquisa para consulta rápida

## Principais Endpoints

### Extração de Texto
- `POST /api/extracao/extrair-texto` - Inicia o processo de extração de texto dos arquivos
- `POST /api/extracao/reprocessar` - Retenta extração de arquivos com falha

### Pesquisa
- `POST /api/extracao/pesquisar-dicionario` - Executa pesquisa baseada em dicionário de palavras
- `POST /api/extracao/palavras` - Cadastra palavras-chave para pesquisa

## Tecnologias
- .NET 6 (C#)
- ASP.NET Core Web API
- MySQL (driver: `MySqlConnector`)
- Autenticação: JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Documentação: Swagger/Swashbuckle
- Logging: NLog

## Requisitos
- .NET SDK 6.0+
- MySQL 8.x (ou compatível)