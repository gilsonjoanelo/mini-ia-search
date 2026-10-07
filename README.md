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