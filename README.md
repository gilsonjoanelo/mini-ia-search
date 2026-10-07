# 🔍 Mini IA Search - Buscador Semântico RAG Multi-Tenant

Este projeto é um motor de **busca semântica e RAG (Retrieval-Augmented Generation) local** construído em **.NET 10 Web API**. A aplicação realiza a raspagem e indexação de sites via `sitemap.xml`, armazena os dados e vetores de forma isolada por *Tenant* no **SQLite**, e responde às dúvidas dos usuários usando o **Ollama** sem dependências de serviços pagos de IA ou infraestrutura externa pesada.

---

## 🚀 Tecnologias Utilizadas

- **Linguagem / Framework:** C# e .NET 10 Web API
- **Banco de Dados:** SQLite (`Microsoft.Data.Sqlite`) com busca híbrida (Vetores BLOB + FTS5 Full-Text Search)
- **Modelos de IA (Ollama Local):**
  - `nomic-embed-text` (Geração de Embeddings/Vetores)
  - `qwen2.5:7b` (Geração e Síntese de Respostas)
- **Integração de IA:** `Microsoft.Extensions.AI` e `OllamaSharp`
- **Raspagem de Dados:** `HtmlAgilityPack`
- **Comunicação em Tempo Real:** SSE (Server-Sent Events) via HTTP Streaming

---

## 🛠️ Arquitetura do Sistema
```
[ Sitemap.xml ]
│
▼
[ SiteIndexerService ] ──► Limpeza de HTML & Chunking (~500 chars)
│
▼
[ nomic-embed-text ]  ──► Vetorização dos Chunks
│
▼
[ SQLite Database ]   ──► Armazena Vetores (BLOB) + Tabela Virtual FTS5
│
▼
[ SearchController ]  ──► Recebe busca + TenantId
│
├──► 1. Busca Híbrida (Produto Escalar + FTS5)
├──► 2. Injeção de Contexto no System Prompt
└─► 3. Streaming (SSE) via qwen2.5:7b para o Usuário
```

---

## ⚡ Recursos Principais

- 🏢 **Multi-Tenant Nativo:** Isolamento total dos dados por `TenantId`.
- 🔍 **Busca Híbrida (Hybrid Search):** Combina similaridade vetorial (produto escalar) com busca textual por palavras-chave (FTS5).
- 🧩 **Chunking Inteligente:** Divisão de páginas em blocos reduzidos para otimizar o consumo de contexto da LLM e acelerar as respostas.
- ⚡ **Respostas em Streaming (SSE):** Envio dos tokens gerados em tempo real via `text/event-stream` (`IAsyncEnumerable`).
- 🖥️ **100% Local e Privado:** Sem necessidade de chaves de API pagas (OpenAI) ou Docker.

---

## 📋 Pré-requisitos

1. **.NET 10 SDK** instalado.
2. **Ollama** rodando localmente na porta `11434`.

Com o Ollama instalado, faça o download dos modelos necessários:

```bash
ollama pull nomic-embed-text
ollama pull qwen2.5:7b
```

🔧 Como Executar o Projeto
1. Clone o repositório:

```
git clone [https://github.com/seu-usuario/mini-ia-search.git](https://github.com/seu-usuario/mini-ia-search.git)
cd mini-ia-search
```

2. Restaure as dependências:

```
dotnet restore
```

3. Execute a aplicação:

```
dotnet run
```

A API iniciará no endereço http://localhost:5000 (ou na porta configurada em launchSettings.json).

📡 Endpoints da API
1. Indexar Sitemap (Admin)
Dispara o processo de scraping, limpeza, vetorização e salvamento no SQLite em segundo plano.

POST ```/api/admin/indexar-sitemap```

Headers: ```X-Tenant-Id: minha_empresa```

Body:
```
{
  "sitemapUrl": "[https://meusite.com.br/sitemap.xml](https://meusite.com.br/sitemap.xml)"
}
```

2. Realizar Busca em Streaming (SSE)
Retorna a resposta token por token à medida que é sintetizada pela LLM.

GET ```/api/search/stream?q=como funciona o sistema contabil```

Headers: ```X-Tenant-Id: minha_empresa```

Response Header: ```Content-Type: text/event-stream```

🤝 Contribuição
Sinta-se à vontade para abrir Issues ou enviar Pull Requests com melhorias no chunking, otimizações de prompt ou suporte a novos tipos de fontes de dados.