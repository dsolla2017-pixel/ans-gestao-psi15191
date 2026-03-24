# Roadmap — MVP 02: Evolução do Sistema de Gestão de ANS

> **Autor:** Diogo Grawingholt | **Projeto:** Transformação Digital na Governança de Dados — GEGOD/CAIXA
> **Site:** [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org) | **Portfólio:** [diogograwingholt.com.br](https://www.diogograwingholt.com.br)

---

Este documento consolida as evoluções planejadas para o **MVP 02** do Sistema de Gestão de ANS. Os itens abaixo representam melhorias identificadas durante a construção do MVP 01 e estão priorizados conforme valor de negócio e viabilidade técnica.

---

## Visão Geral

| MVP | Escopo | Status |
| :--- | :--- | :--- |
| **MVP 01** | CRUD completo de ANS, dupla-custódia, auditoria, deploy IIS, painel interativo | Entregue |
| **MVP 02** | Personas, mockups, integrações, dashboards analíticos | Planejado |

---

## 1. Mockups de Telas do Sistema Angular

Gerar screenshots de alta fidelidade das telas da aplicação Angular (formulário de inclusão de ANS, listagem com filtros, detalhamento do acordo, fluxo de assinatura) e apresentá-los como cards visuais interativos na seção de Arquitetura do painel web. Essa adição permite ao avaliador visualizar a experiência do usuário final sem a necessidade de executar a aplicação localmente.

**Entregáveis:**
- 4 mockups de telas (Inclusão, Listagem, Detalhes, Assinatura)
- Integração visual no painel web (seção Arquitetura)

---

## 2. Seção de Personas e Jornadas de Usuário

Criar cards com os perfis dos três tipos de usuário do sistema, detalhando suas jornadas específicas e permissões dentro da aplicação. Essa seção demonstra domínio de UX Research e Design Centrado no Usuário.

| Persona | Papel no Sistema | Jornada Principal |
| :--- | :--- | :--- |
| **Empregado GEGOD** | Cadastra, edita e solicita inativação de ANS | Inclusão → Acompanhamento → Solicitação de Inativação |
| **Chefe de Unidade** | Assina o ANS conforme papel da unidade (fornecedora ou consumidora) | Revisão → Assinatura → Ativação do Acordo |
| **Gestor GEGOD** | Autoriza ou recusa inativações (dupla-custódia) | Análise de Justificativa → Autorização/Recusa |

**Entregáveis:**
- 3 cards de persona com avatar, descrição e fluxo
- Integração visual no painel web (nova seção entre Arquitetura e Roadmap)

---

## 3. Dashboard Analítico com Power BI Embedded

Desenvolver um dashboard gerencial com indicadores operacionais dos ANS, integrado ao Microsoft Fabric/Power BI. O dashboard apresenta métricas como quantidade de acordos por situação, tempo médio de tramitação, acordos próximos do vencimento e distribuição por coparticipada.

**Entregáveis:**
- Modelo semântico no Power BI Desktop
- Relatório com 3 páginas (Visão Geral, Operacional, Conformidade)
- Embed no painel web via iframe autenticado

---

## 4. Integração com Catálogo de Dados Corporativo

Conectar o campo "Objeto Físico" do formulário de inclusão ao Catálogo Corporativo de Dados da CAIXA, permitindo que o empregado selecione tabelas e arquivos a partir de uma lista padronizada e validada, em vez de digitação livre.

**Entregáveis:**
- Endpoint de integração no back-end (`GET /api/v1/catalogo/objetos`)
- Componente Angular de autocomplete conectado ao catálogo
- Documentação da API de integração

---

## 5. Alertas Proativos de Vencimento

Implementar workers assíncronos (Hangfire ou Azure Functions) para notificar os responsáveis técnicos via e-mail e Microsoft Teams quando um ANS estiver próximo do vencimento (30, 15 e 5 dias antes), sugerindo renovação ou inativação.

**Entregáveis:**
- Worker de notificação com Hangfire
- Templates de e-mail responsivos
- Integração com webhook do Microsoft Teams

---

## 6. Animação de Entrada do Mascote GEGOD

Adicionar animação de bounce e wave no mascote GEGOD ao carregar a página do painel web, tornando a experiência mais dinâmica e memorável para o avaliador. Incluir micro-interações ao passar o mouse sobre o mascote.

**Entregáveis:**
- Animação CSS/Framer Motion no componente MascotBubble
- Interação hover com tooltip contextual

---

## Priorização

| Prioridade | Item | Esforço Estimado | Valor de Negócio |
| :--- | :--- | :--- | :--- |
| Alta | 1. Mockups de Telas | 4h | Impacto visual direto na avaliação |
| Alta | 2. Seção de Personas | 3h | Demonstra domínio de UX e visão de produto |
| Média | 3. Dashboard Power BI | 8h | Diferencial analítico para gestores |
| Média | 4. Catálogo de Dados | 6h | Integração corporativa estratégica |
| Baixa | 5. Alertas de Vencimento | 5h | Automação operacional |
| Baixa | 6. Animação GEGOD | 2h | Refinamento visual |

---

> **Documentação completa e painel interativo:** [gegodtransformacaodosdados.org](https://gegodtransformacaodosdados.org)
> **Portfólio do autor:** [diogograwingholt.com.br](https://www.diogograwingholt.com.br)
> **Repositório:** [github.com/dsolla2017-pixel/ans-gestao-psi15191](https://github.com/dsolla2017-pixel/ans-gestao-psi15191)
>
> © 2025 Diogo Grawingholt — Todos os direitos reservados
