// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Interface (TypeScript): Define a forma dos dados que trafegam no front-end.
// - Export: Torna o modelo disponivel para outros arquivos do projeto.
// ============================================================

/**
 * Modelo de resumo de um ANS para exibicao em listagens.
 */
export interface AcordoResumo {
  coAcordo: number;
  noFornecedora: string;
  noConsumidora: string;
  situacao: string;
  dtCriacao: string;
  dtAssinatura?: string;
  dtFimVigencia?: string;
  qtDiasVigencia: number;
  icDadoPessoal: boolean;
  icDadoSensivel: boolean;
}

/**
 * Modelo completo de um ANS para detalhamento.
 */
export interface AcordoDetalhe extends AcordoResumo {
  noUnidadeFornecedora: string;
  noUnidadeConsumidora: string;
  coGrauSigilo: string;
  dePeriodicidade: string;
  deJustificativaInativacao?: string;
  responsaveis: Responsavel[];
  dadosCompartilhados: DadoCompartilhado[];
}

/**
 * Modelo para criacao de um novo ANS.
 */
export interface CriarAcordo {
  noFornecedora: string;
  noConsumidora: string;
  noUnidadeFornecedora: string;
  noUnidadeConsumidora: string;
  qtDiasVigencia: number;
  coGrauSigilo: string;
  dePeriodicidade: string;
  icDadoPessoal: boolean;
  icDadoSensivel: boolean;
  responsavelFornecedora: Responsavel;
  responsavelConsumidora: Responsavel;
  dadosCompartilhados: DadoCompartilhado[];
}

/**
 * Modelo de um responsavel (tecnico ou assinante).
 */
export interface Responsavel {
  noPessoa: string;
  nuCpf: string;
  nuMatricula?: string;
  nuTelefone?: string;
  deEmail?: string;
  noCargoFuncao?: string;
  tpPapel: string;
  dtAssinatura?: string;
}

/**
 * Modelo de um dado compartilhado no ANS.
 */
export interface DadoCompartilhado {
  noNegocial: string;
  noObjetoFisico: string;
  deLink?: string;
  icDadoPessoal: boolean;
  icDadoSensivel: boolean;
}
