// =================================================================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// =================================================================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados
//
// Este arquivo de modelos TypeScript demonstra a aplicação de princípios sólidos de engenharia de
// software para garantir um código limpo, manutenível e escalável. A seguir, detalhamos as
// principais referências que nortearam a sua concepção:
//
// - Clean Architecture: A separação dos modelos de dados (interfaces) em um arquivo dedicado
//   reflete a preocupação com a separação de responsabilidades (SoC), um pilar da Clean
//   Architecture. Os modelos são entidades puras que representam o domínio da aplicação, 
//   independentes de frameworks ou UI.
//
// - SOLID Principles:
//   - Single Responsibility Principle (SRP): Cada interface define uma estrutura de dados
//     específica e coesa (AcordoResumo, AcordoDetalhe, etc.), evitando a criação de modelos
//     genéricos e sobrecarregados.
//   - Interface Segregation Principle (ISP): A utilização de interfaces distintas para diferentes
//     contextos (resumo vs. detalhe) evita que os componentes consumidores dependam de
//     propriedades que não utilizam, otimizando a performance e a clareza.
//
// - TypeScript Best Practices & Angular Style Guide:
//   - Interfaces for Data Modeling: O uso de `interface` em vez de `class` para definir a forma
//     dos dados é uma prática recomendada pelo TypeScript e pelo Angular Style Guide. Interfaces
//     são "apagadas" durante a transpilação para JavaScript, resultando em um bundle final mais
//     leve, pois não geram código executável, servindo apenas para a verificação de tipos em
//     tempo de desenvolvimento.
//   - Naming Conventions: A nomeclatura `PascalCase` para interfaces e `camelCase` para
//     propriedades segue as convenções universalmente adotadas, promovendo a legibilidade e a
//     consistência do código.
//
// - Design Patterns:
//   - Data Transfer Object (DTO): As interfaces aqui definidas atuam como DTOs, encapsulando e
//     transportando dados entre as camadas da aplicação (serviços, componentes) de forma
//     tipada e segura. A herança de `AcordoDetalhe` a partir de `AcordoResumo` é uma otimização
//     que promove o reuso de código (DRY - Don't Repeat Yourself).
//
// - Documentação (JSDoc): A documentação clara e concisa de cada interface e propriedade
//   facilita o entendimento e a manutenção do código por outros desenvolvedores, alinhando-se
//   às melhores práticas de desenvolvimento colaborativo.
//   Documentação completa: https://gegodtransformacaodosdados.org
// =================================================================================================

/**
 * @interface AcordoResumo
 * @description Define a estrutura de dados para a visualização resumida de um Acordo de Nível de
 *              Serviço (ANS). Este modelo é otimizado para listagens e exibições em grade,
 *              contendo apenas as informações essenciais para identificação e status.
 * @see Documentação completa: https://gegodtransformacaodosdados.org
 */
export interface AcordoResumo {
  // Chave primária que identifica unicamente o acordo no sistema. Essencial para operações de CRUD.
  coAcordo: number;

  // Nome da unidade organizacional que atua como fornecedora dos dados ou serviços.
  noFornecedora: string;

  // Nome da unidade organizacional que consome os dados ou serviços disponibilizados.
  noConsumidora: string;

  // Representa o estado atual do acordo no seu ciclo de vida (ex: 'Em Elaboração', 'Vigente', 'Inativo').
  situacao: string;

  // Timestamp da criação do registro do acordo no sistema, em formato ISO (string).
  dtCriacao: string;

  // Data opcional da assinatura do acordo, marcando o início de sua validade legal ou operacional.
  dtAssinatura?: string;

  // Data opcional que define o término da vigência do acordo.
  dtFimVigencia?: string;

  // Duração da vigência do acordo em dias, calculada a partir das datas de assinatura e fim de vigência.
  qtDiasVigencia: number;

  // Indicador booleano que sinaliza se o acordo envolve o compartilhamento de dados pessoais.
  icDadoPessoal: boolean;

  // Indicador booleano que sinaliza se o acordo envolve o compartilhamento de dados sensíveis (subcategoria de dados pessoais).
  icDadoSensivel: boolean;
}

/**
 * @interface AcordoDetalhe
 * @description Estende o modelo `AcordoResumo` para fornecer uma visão completa e detalhada de um
 *              ANS. Inclui informações adicionais como unidades específicas, responsáveis e os
 *              dados efetivamente compartilhados. Utilizado em telas de detalhamento.
 * @extends AcordoResumo
 * @see Documentação completa: https://gegodtransformacaodosdados.org
 */
export interface AcordoDetalhe extends AcordoResumo {
  // Nome detalhado da unidade fornecedora, pode incluir subníveis ou especificações.
  noUnidadeFornecedora: string;

  // Nome detalhado da unidade consumidora, para maior granularidade da informação.
  noUnidadeConsumidora: string;

  // Código que representa o grau de sigilo da informação compartilhada (ex: 'Público', 'Restrito').
  coGrauSigilo: string;

  // Descrição da frequência com que os dados são compartilhados (ex: 'Diária', 'Sob Demanda').
  dePeriodicidade: string;

  // Justificativa textual para a inativação do acordo, preenchida quando o status é alterado para 'Inativo'.
  deJustificativaInativacao?: string;

  // Array de objetos `Responsavel`, contendo os contatos técnicos e assinantes do acordo.
  responsaveis: Responsavel[];

  // Array de objetos `DadoCompartilhado`, detalhando cada item de dado que é objeto do acordo.
  dadosCompartilhados: DadoCompartilhado[];
}

/**
 * @interface CriarAcordo
 * @description Define a estrutura de dados necessária para a criação de um novo Acordo de Nível
 *              de Serviço. Este modelo serve como um DTO (Data Transfer Object) para o endpoint
 *              de criação, garantindo que todos os campos obrigatórios sejam fornecidos.
 * @see Documentação completa: https://gegodtransformacaodosdados.org
 */
export interface CriarAcordo {
  // Nome da unidade fornecedora. Campo obrigatório para a criação do acordo.
  noFornecedora: string;

  // Nome da unidade consumidora. Campo obrigatório.
  noConsumidora: string;

  // Nome específico da unidade fornecedora, para maior detalhamento.
  noUnidadeFornecedora: string;

  // Nome específico da unidade consumidora.
  noUnidadeConsumidora: string;

  // Período de vigência do acordo em dias. Essencial para o controle do ciclo de vida.
  qtDiasVigencia: number;

  // Código que define o nível de sigilo das informações transacionadas.
  coGrauSigilo: string;

  // Descrição da periodicidade do compartilhamento de dados.
  dePeriodicidade: string;

  // Flag que indica a presença de dados pessoais no escopo do acordo.
  icDadoPessoal: boolean;

  // Flag que indica a presença de dados sensíveis, exigindo tratamento especial.
  icDadoSensivel: boolean;

  // Objeto contendo os dados do responsável pela área fornecedora.
  responsavelFornecedora: Responsavel;

  // Objeto contendo os dados do responsável pela área consumidora.
  responsavelConsumidora: Responsavel;

  // Lista detalhada dos dados que serão compartilhados no âmbito deste acordo.
  dadosCompartilhados: DadoCompartilhado[];
}

/**
 * @interface Responsavel
 * @description Modela a figura de um responsável, seja ele técnico ou assinante, dentro do
 *              contexto de um ANS. Contém informações de identificação e contato.
 * @see Documentação completa: https://gegodtransformacaodosdados.org
 */
export interface Responsavel {
  // Nome completo do responsável.
  noPessoa: string;

  // Número do Cadastro de Pessoas Físicas (CPF), utilizado como identificador único.
  nuCpf: string;

  // Matrícula funcional do responsável na empresa (opcional).
  nuMatricula?: string;

  // Número de telefone para contato (opcional).
  nuTelefone?: string;

  // Endereço de e-mail profissional para comunicações formais (opcional).
  deEmail?: string;

  // Cargo ou função ocupada pelo responsável na organização (opcional).
  noCargoFuncao?: string;

  // Define o papel do responsável no acordo (ex: 'Técnico', 'Assinante').
  tpPapel: string;

  // Data da assinatura, aplicável apenas a responsáveis com papel de 'Assinante' (opcional).
  dtAssinatura?: string;
}

/**
 * @interface DadoCompartilhado
 * @description Descreve um item de dado específico que é objeto de compartilhamento em um ANS.
 *              Inclui tanto a sua descrição de negócio quanto a sua representação física.
 * @see Documentação completa: https://gegodtransformacaodosdados.org
 */
export interface DadoCompartilhado {
  // Nome de negócio do dado, como ele é conhecido e utilizado pelas áreas de negócio.
  noNegocial: string;

  // Nome técnico do objeto físico (tabela, arquivo, etc.) onde o dado reside.
  noObjetoFisico: string;

  // URL ou caminho de rede para acessar o recurso ou sua documentação (opcional).
  deLink?: string;

  // Indicador booleano que classifica o dado como pessoal, sujeito à LGPD.
  icDadoPessoal: boolean;

  // Indicador booleano que classifica o dado como sensível, requerendo proteção adicional.
  icDadoSensivel: boolean;
}
