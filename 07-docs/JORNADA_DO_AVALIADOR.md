# Jornada do Avaliador: Sistema de Gestão de ANS

> **Autor:** Diogo Grawingholt | **Projeto:** Transformação Digital na Governança de Dados — GEGOD/CAIXA
> **Site:** [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org) | **Portfólio:** [diogograwingholt.com.br](https://www.diogograwingholt.com.br)

Este documento apresenta um roteiro estruturado para navegação pela proposta técnica do desafio PSI 15191. O objetivo é oferecer uma visão clara das decisões arquiteturais, dos entregáveis e do impacto institucional do projeto. A documentação interativa completa está disponível em [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org).

## 1. Mapa de Navegação

A tabela abaixo direciona você aos pontos focais da avaliação, otimizando seu tempo e destacando os entregáveis críticos.

| O que avaliar | Onde encontrar | Tempo estimado |
| :--- | :--- | :--- |
| **Arquitetura e Decisões Técnicas** | `PROPOSTA_DESAFIO.md` (Seções 3 e 4) | 5 min |
| **Modelagem e Nomenclatura Padrão** | `PROPOSTA_DESAFIO.md` (Seção 9) | 3 min |
| **Regras de Negócio e Fluxos** | `PROPOSTA_DESAFIO.md` (Seções 6 e 8) | 4 min |
| **Código Fonte Back-end (C#)** | Repositório `03-backend/src/` | 10 min |
| **Código Fonte Front-end (Angular)** | Repositório `04-frontend/src/` | 10 min |
| **Scripts de Deploy e Banco de Dados** | Pastas `01-setup/`, `02-database/` e `06-deploy/` | 2 min |
| **Painel Web Interativo** | [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org) | 5 min |

## 2. Objetivo do Projeto

O Sistema de Gestão de Acordos de Nível de Serviço (ANS) foi concebido para centralizar e digitalizar o processo de compartilhamento de dados entre a CAIXA e as empresas do conglomerado. A solução elimina a dependência de controles paralelos, assegurando que todas as trocas de informações ocorram sob um amparo legal, rastreável e auditável.

Alinhado ao mapa estratégico da instituição, o projeto fortalece a governança de dados, mitiga riscos de conformidade (especialmente relacionados à LGPD e sigilo bancário) e promove a eficiência operacional através de fluxos de aprovação automatizados e seguros.

## 3. Dor Identificada e Solução

| Dor Identificada | Impacto Direto | Evidência de Solução |
| :--- | :--- | :--- |
| Descentralização dos acordos de dados. | Risco de compartilhamento indevido e dificuldade em auditorias. | Repositório único (banco de dados relacional) com trilha de auditoria completa. |
| Lentidão na aprovação de novos ANS. | Atraso no acesso a dados críticos para o negócio das coparticipadas. | Fluxo de assinaturas digitais integrado ao sistema, com alertas de pendência. |
| Inativações sem controle rigoroso. | Perda de rastreabilidade sobre quem autorizou o fim de um compartilhamento. | Fluxo de dupla-custódia exclusivo para a GEGOD com exigência de justificativa. |

## 4. Ações Práticas Implementadas

### Adoção de Clean Architecture
A separação rigorosa de responsabilidades garante que o núcleo do negócio (Domain) permaneça imune a mudanças de frameworks ou infraestrutura. Isso facilita testes unitários e a manutenção a longo prazo, reduzindo o custo de propriedade do software.

### Conformidade com Padrões CAIXA
Toda a modelagem de banco de dados foi construída seguindo estritamente o Guia de Nomenclatura de Objetos e o Guia de Expressões Regulares da CAIXA. No front-end, a integração nativa com o Design System CAIXA (DSC) garante a identidade visual institucional e acessibilidade.

### Segurança e Rastreabilidade
A implementação de perfis de acesso baseados em funções (RBAC) e o registro automático de histórico (tabela `TB_ANS_HISTORICO`) garantem que cada ação de criação, assinatura ou inativação seja perfeitamente rastreável até o empregado responsável.

## 5. Legado Institucional

| Legado | Descrição | Beneficiário |
| :--- | :--- | :--- |
| **Governança Fortalecida** | Visibilidade total sobre quais dados trafegam entre a CAIXA e o conglomerado, seus responsáveis e o grau de sigilo envolvido. | Alta Administração e Auditoria. |
| **Arquitetura de Referência** | Código estruturado que serve como modelo de boas práticas (Clean Architecture, Angular) para futuros projetos da GEGOD. | Equipe de Desenvolvimento. |
| **Agilidade Operacional** | Redução do tempo de tramitação de acordos através de fluxos digitais e eliminação de papel/planilhas. | Unidades Fornecedoras e Consumidoras. |

## 6. Impacto na Sociedade e Conglomerado

| Indicador | Conexão com o Projeto |
| :--- | :--- |
| **Segurança de Dados Pessoais** | A exigência de classificação de dados sensíveis no ANS garante o cumprimento da LGPD, protegendo os dados dos clientes da CAIXA. |
| **Sinergia do Conglomerado** | Facilita a criação de novos produtos e serviços integrados entre a CAIXA e suas participadas, agilizando o compartilhamento legal de informações. |
| **Sustentabilidade (ESG)** | Digitalização de 100% do processo de acordos, eliminando a necessidade de impressões e tramitações físicas de documentos. |

---

> **Documentação interativa e painel completo:** [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org)
> **Portfólio do autor:** [diogograwingholt.com.br](https://www.diogograwingholt.com.br)
> **Repositório:** [github.com/dsolla2017-pixel/ans-gestao-psi15191](https://github.com/dsolla2017-pixel/ans-gestao-psi15191)
>
> © 2025 Diogo Grawingholt — Todos os direitos reservados
