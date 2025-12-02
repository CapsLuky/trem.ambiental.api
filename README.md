# 🌱 Tren Ambiental API

API RESTful desenvolvida em .NET Core para gerenciar um sistema de reciclagem gamificado, onde usuários ganham pontos ao doar materiais recicláveis e podem trocá-los por produtos no catálogo.

## 📋 Sobre o Projeto

O Tren Ambiental é uma plataforma que incentiva a reciclagem através de um sistema de pontuação. A API consumido por dois frontends distintos:

### 👥 Portal do Cliente
- Visualização do catálogo de produtos disponíveis para troca
- Acompanhamento de pontos acumulados
- Histórico de doações e pesagens
- Sistema de carrinho e pedidos
- Ranking de usuários mais engajados

### 🔧 Portal Administrativo
- Gestão completa do catálogo de produtos
- Controle de estoque e disponibilidade
- Cadastro e configuração de tipos de materiais recicláveis
- Definição de pontuação por quilo de cada material
- Lançamento de pesagens e doações recebidas
- Gerenciamento de pedidos e entregas
- Relatórios e análises

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design (DDD)**, organizado em camadas bem definidas:

```
API.TrenAmbiental/
├── 0.WebApi/              # Camada de Apresentação
│   └── API.TrenAmbiental.WebApi
│       ├── Controllers/   # Endpoints da API
│       ├── Configurations/# Configurações (Swagger, DI, etc)
│       └── Bases/         # Controllers base
│
├── 1.DTO/                 # Camada de Transferência de Dados
│   └── API.TrenAmbiental.DTO
│       ├── Model/         # DTOs de entrada
│       ├── ViewModel/     # DTOs de saída
│       ├── Entidade/      # Entidades de domínio
│       └── DomainObjects/ # Objetos de valor e interfaces
│
├── 2.Domain/              # Camada de Domínio
│   └── API.TrenAmbiental.Domain
│       └── Services/      # Regras de negócio
│
└── 3.Infrastructure/      # Camada de Infraestrutura
    └── API.TrenAmbiental.Data.Mysql
        ├── Repositories/  # Acesso a dados
        └── Interfaces/    # Contratos de repositórios
```

### Padrões e Práticas Implementadas

- **Repository Pattern**: Abstração da camada de acesso a dados
- **Dependency Injection**: Inversão de controle e baixo acoplamento
- **Service Layer**: Encapsulamento da lógica de negócio
- **DTO Pattern**: Separação entre modelos de domínio e transferência
- **Notification Pattern**: Tratamento centralizado de erros e validações
- **JWT Authentication**: Autenticação stateless baseada em tokens
- **API Versioning**: Versionamento de endpoints para evolução controlada

## 🚀 Funcionalidades Principais

### Autenticação e Autorização
- Login com JWT (JSON Web Token)
- Recuperação e redefinição de senha
- Controle de acesso baseado em roles (perfis)
- Tokens com expiração configurável

### Gestão de Usuários
- Cadastro de clientes e administradores
- Perfis diferenciados (Cliente, Administrador, etc)
- Ativação/desativação de contas

### Sistema de Pontuação
- Cálculo automático de pontos por material reciclado
- Consulta de saldo de pontos
- Histórico de pontuações
- Ranking mensal de usuários

### Pesagem e Coleta
- Registro de materiais recebidos
- Conversão automática de peso em pontos
- Histórico de pesagens por usuário
- Tipos de materiais configuráveis

### Catálogo de Produtos
- CRUD completo de produtos
- Upload de imagens
- Controle de estoque
- Produtos ativos/inativos
- Alertas de estoque baixo

### Sistema de Pedidos (Carrinho)
- Carrinho de compras com pontos
- Validação de saldo antes da finalização
- Histórico de pedidos
- Status de pedidos (Pendente, Aprovado, Entregue, etc)
- Expiração automática de pedidos não finalizados
- Gestão administrativa de pedidos

## 🛠️ Tecnologias Utilizadas

- **.NET Core 3.1+** - Framework principal
- **ASP.NET Core Web API** - Construção da API RESTful
- **MySQL** - Banco de dados relacional
- **Dapper** - Micro ORM para acesso a dados
- **JWT Bearer** - Autenticação e autorização
- **Swagger/OpenAPI** - Documentação interativa da API
- **NLog** - Sistema de logging
- **Newtonsoft.Json** - Serialização JSON
- **API Versioning** - Versionamento de endpoints

## 📦 Estrutura de Dependências

```
WebApi → Domain → DTO
  ↓        ↓
Infrastructure
```

## 🔐 Segurança

- Autenticação JWT com chave secreta configurável
- Autorização baseada em roles
- Validação de modelos em todos os endpoints
- CORS configurável
- HTTPS recomendado para produção
- Tokens de recuperação de senha com expiração

## 📊 Endpoints Principais

### Autenticação
- `POST /api/v1/Autenticacao/login` - Login de usuário
- `POST /api/v1/Autenticacao/login/alterarsenha` - Alterar senha
- `GET /api/v1/Autenticacao/login/recriarSenha/{email}` - Recuperar senha
- `POST /api/v1/Autenticacao/login/redefinirSenha` - Redefinir senha com token

### Cadastro
- `POST /api/v1/Cadastro` - Criar novo usuário
- `GET /api/v1/Cadastro/{id}` - Buscar usuário
- `PUT /api/v1/Cadastro` - Atualizar usuário

### Catálogo
- `GET /api/v1/Catalogo` - Listar produtos
- `POST /api/v1/Catalogo` - Criar produto
- `PUT /api/v1/Catalogo` - Atualizar produto
- `DELETE /api/v1/Catalogo/{id}` - Remover produto

### Pesagem
- `POST /api/v1/Pesagem` - Registrar pesagem
- `GET /api/v1/Pesagem/historico/{idUsuario}` - Histórico de pesagens

### Pontuação
- `POST /api/v1/Pontuacao` - Consultar pontuação
- `GET /api/v1/Pontuacao/Saldo` - Consultar saldo
- `GET /api/v1/Pontuacao/BuscarRanking` - Ranking de usuários

### Carrinho/Pedidos
- `GET /api/v1/Carrinho/meuCarrinho/{idUsuario}` - Obter carrinho
- `POST /api/v1/Carrinho/AdicionarItem` - Adicionar item
- `POST /api/v1/Carrinho/finalizarPedido` - Finalizar pedido
- `GET /api/v1/Carrinho/historicoDePedido/{idUsuario}` - Histórico

## ⚙️ Configuração e Instalação

### Pré-requisitos
- .NET Core SDK 3.1 ou superior
- MySQL Server 5.7+
- Visual Studio 2019+ ou VS Code

### Passos para Execução

### Roles (Perfis)
- `1`: Administrador Master
- `2`: Administrador
- `3`: Operador
- `4`: Cliente

## 🧪 Testes

O projeto está estruturado para facilitar a implementação de testes:
- Injeção de dependências permite mock de serviços
- Separação clara de responsabilidades
- Interfaces bem definidas para cada camada


---

## 📞 Contato

Para mais informações sobre este projeto, entre em contato:

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/lucianorodriguess/)

---