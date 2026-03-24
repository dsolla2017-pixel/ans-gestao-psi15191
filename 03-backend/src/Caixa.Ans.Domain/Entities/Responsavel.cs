// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// ============================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// [DDD] Entity dentro do Aggregate de Acordo:
//   Responsável é uma entidade filha que só existe no contexto
//   de um Acordo. Seu ciclo de vida é gerenciado pelo Aggregate Root.
//
// [LGPD] Lei 13.709/2018 — Dados Pessoais:
//   Os campos NuCpf e NoPessoa são dados pessoais protegidos.
//   O acesso deve ser restrito conforme o princípio da necessidade.
//
// [Nomenclatura CAIXA] Guia de Nomenclatura de Objetos SGBD:
//   Prefixos aplicados: CO_ (código), NO_ (nome), NU_ (número),
//   DE_ (descrição), DT_ (data), TP_ (tipo).
// ============================================================

using System;
using Caixa.Ans.Domain.Enums;

namespace Caixa.Ans.Domain.Entities
{
    /// <summary>
    /// Entidade que representa um responsável vinculado ao Acordo de Nível de Serviço.
    /// 
    /// Cada ANS possui múltiplos responsáveis com papéis distintos:
    /// - Responsável Técnico (Fornecedora e Consumidora): ponto focal operacional
    /// - Assinante (Fornecedora e Consumidora): chefe de unidade com poder de assinatura
    /// 
    /// Mapeada para a tabela TB_ANS_RESPONSAVEL conforme o Guia de Nomenclatura CAIXA.
    /// 
    /// Documentação completa: https://gegodtransformacaodosdados.org
    /// </summary>
    public class Responsavel
    {
        // ── Identificação ──────────────────────────────────────────
        // Chave primária surrogate gerada pelo banco de dados.
        public int CoResponsavel { get; set; }

        // Chave estrangeira para o Acordo (Aggregate Root).
        // Garante integridade referencial no nível do banco de dados,
        // complementando a proteção já existente no nível do domínio.
        public int CoAcordo { get; set; }

        // ── Dados do Responsável ───────────────────────────────────
        // Informações de identificação conforme requisito do PSI 15191:
        // "nome, CPF, matrícula (se empregado), telefone e e-mail de contato".

        // Nome completo do responsável (campo obrigatório).
        public string NoPessoa { get; set; } = string.Empty;

        // CPF do responsável — dado pessoal protegido pela LGPD.
        // Armazenado sem formatação para consistência e indexação.
        public string NuCpf { get; set; } = string.Empty;

        // Matrícula funcional — preenchida apenas para empregados CAIXA.
        // Nullable porque responsáveis externos não possuem matrícula.
        public string? NuMatricula { get; set; }

        // Telefone de contato para comunicação operacional.
        public string? NuTelefone { get; set; }

        // E-mail de contato — canal primário de notificação.
        // Utilizado para alertas de vigência e pendências de assinatura.
        public string? DeEmail { get; set; }

        // Cargo ou função do responsável na organização.
        // Relevante para validação de competência de assinatura.
        public string? NoCargoFuncao { get; set; }

        // ── Papel no Acordo ────────────────────────────────────────
        // Define a função do responsável: Técnico ou Assinante,
        // da Fornecedora ou da Consumidora. Essa distinção é essencial
        // para o fluxo de dupla-custódia na assinatura do ANS.
        public TipoPapel TpPapel { get; set; }

        // ── Assinatura Digital ─────────────────────────────────────
        // Registro temporal da assinatura do chefe de unidade.
        // Nullable porque a assinatura ocorre em momento posterior
        // à criação do acordo. O ANS só é ativado quando ambos os
        // assinantes (Fornecedora e Consumidora) registram suas assinaturas.
        public DateTime? DtAssinatura { get; set; }

        // ── Navegação (Entity Framework Core) ──────────────────────
        // Navigation property para o Aggregate Root.
        // Permite acesso bidirecional entre Responsável e Acordo,
        // facilitando consultas LINQ e carregamento lazy/eager.
        public Acordo? Acordo { get; set; }
    }
}
