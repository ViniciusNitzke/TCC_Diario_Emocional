# Diario Emocional Inteligente

Aplicacao web responsiva para TCC em Engenharia de Software aplicada a saude mental digital. O sistema permite cadastro, login, registro emocional contextual, historico, relatorios visuais, lembretes inspirados no Experience Sampling Method, sugestoes simples de autocuidado, modo de acolhimento e configuracoes de privacidade.

> O produto apoia autorreflexao e autocuidado. Ele nao realiza diagnostico, nao prescreve tratamento e nao substitui psicologos, medicos ou servicos de emergencia.

## Tecnologias

- Frontend: React + Vite
- Backend: ASP.NET Core Web API (.NET 8)
- Comunicacao: REST + JSON
- Persistencia demonstrativa: JSON local em `backend/data/diario-data.json`
- Persistencia prevista para producao: Firebase Cloud Firestore
- Autenticacao demonstrativa: token de sessao gerado pela API
- Autenticacao prevista para producao: Firebase Authentication ou OAuth 2.0

## Arquitetura

O backend foi organizado em camadas para facilitar manutencao, testes e evolucao:

```text
backend/
  Api/                 Endpoints REST e adaptadores HTTP
  Application/         Casos de uso, DTOs, contratos e estrategias
  Domain/              Entidades e interfaces de repositorio
  Infrastructure/      Persistencia JSON, seguranca e exportacao PDF
  Program.cs           Composition Root e configuracao da API
```

O frontend tambem foi separado por responsabilidade:

```text
frontend/src/
  components/          Telas e componentes reutilizaveis
  constants/           Opcoes fixas da interface
  services/            Cliente HTTP da API
  App.jsx              Orquestracao de estado e fluxo de navegacao
  styles.css           Estilos responsivos
```

## SOLID e padroes de projeto aplicados

- Single Responsibility Principle: entidades, servicos, repositorios, exportador PDF e componentes React possuem responsabilidades separadas.
- Open/Closed Principle: novas sugestoes de autocuidado podem ser adicionadas criando outra estrategia, sem alterar o fluxo principal.
- Liskov Substitution Principle: contratos como `IUserRepository` e `IEmotionalEntryRepository` podem ser implementados por JSON, Firestore ou outro banco.
- Interface Segregation Principle: interfaces pequenas foram separadas por caso de uso, como autenticacao, relatorio, entrada emocional e configuracoes.
- Dependency Inversion Principle: a camada Application depende de abstracoes do Domain, nao de detalhes de banco ou framework.
- Repository Pattern: acesso a usuarios, sessoes e registros fica encapsulado nos repositorios.
- Strategy Pattern: regras de autocuidado variam por emocao e intensidade.
- Factory Pattern: `SelfCareSuggestionFactory` seleciona a estrategia adequada.
- Dependency Injection: `Infrastructure/DependencyInjection.cs` centraliza a composicao das dependencias.
- DTO Pattern: requests e responses da API ficam separados das entidades internas.

## Funcionalidades

- Cadastro, login, sessao por token e logout.
- Registro diario de emocoes com intensidade, gatilhos, situacao, observacao e origem do registro.
- Catalogo emocional inspirado na Roda das Emocoes de Plutchik.
- Historico emocional em linha do tempo.
- Dashboard com resumo do dia, sequencia de uso e proximo lembrete ESM.
- Relatorio semanal com emocao mais frequente, intensidade media, gatilhos recorrentes e insight automatico.
- Exportacao de relatorio em PDF.
- Sugestoes automaticas de autocuidado por estrategia emocional.
- Modo de acolhimento com passos de estabilizacao e orientacao de busca de ajuda.
- Configuracoes de ESM e privacidade, incluindo exclusao de registros.

## Documentacao para o TCC

A pasta `docs/` contem uma versao pronta da secao de desenvolvimento:

- `docs/04_DESENVOLVIMENTO.md`: texto base do topico 4, com arquitetura, tecnologias, requisitos de implantacao, trechos de codigo e espaco para prints.
- `docs/diagramas/caso_de_uso.mmd`: diagrama de caso de uso.
- `docs/diagramas/arquitetura.mmd`: diagrama de arquitetura.
- `docs/diagramas/classes.mmd`: diagrama de classes simplificado.

## Como executar

### Backend

```powershell
cd backend
dotnet run --urls http://localhost:5000
```

API:

```text
http://localhost:5000/api
```

### Frontend

Em outro terminal:

```powershell
cd frontend
npm install
npm run dev
```

Interface:

```text
http://127.0.0.1:5173
```

## Uso demonstrativo

1. Abra a interface.
2. Clique em "Criar conta".
3. Informe qualquer e-mail valido e senha com pelo menos 6 caracteres.
4. Registre emocoes, acesse historico, gere relatorio e teste ajustes de ESM/privacidade.

## Endpoints principais

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/auth/me`
- `GET /api/catalogo/emocoes`
- `POST /api/registros`
- `GET /api/registros`
- `DELETE /api/registros/{id}`
- `GET /api/dashboard`
- `GET /api/relatorios/semanal`
- `GET /api/relatorios/pdf`
- `GET /api/autocuidado/sugestoes`
- `GET /api/crise`
- `GET /api/esm/configuracao`
- `PUT /api/esm/configuracao`
- `PUT /api/privacidade`
- `DELETE /api/privacidade/dados`

## Escalabilidade e evolucao

A versao atual usa JSON local para permitir avaliacao sem credenciais externas. Para producao, substitua os repositorios em `Infrastructure/Persistence/Repositories` por implementacoes Firestore mantendo as interfaces do dominio. O restante do sistema continua igual, pois os casos de uso dependem de abstracoes.

Melhorias futuras recomendadas:

- Implementar Firestore e Firebase Authentication.
- Adicionar testes automatizados de servicos e endpoints.
- Criar pipeline CI/CD no GitHub Actions.
- Aplicar HTTPS e CORS restritivo em producao.
- Revisar sugestoes de autocuidado com profissional de saude mental.
- Ampliar acessibilidade, internacionalizacao e notificacoes adaptativas.

## Validacao tecnica

Comandos usados para verificar o projeto:

```powershell
dotnet build
npm run build
```
