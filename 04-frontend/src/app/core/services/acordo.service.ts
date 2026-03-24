// =================================================================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// =================================================================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados
//
// 1.  SOLID - Princípio da Responsabilidade Única (SRP):
//     Este serviço tem a responsabilidade exclusiva de gerenciar a comunicação com a API de Acordos.
//     Qualquer outra funcionalidade (ex: manipulação de estado, lógica de UI) é delegada a outros componentes.
//
// 2.  Clean Architecture - Camada de Infraestrutura:
//     O `AcordoService` atua como um gateway para a fonte de dados externa (API), isolando o restante
//     da aplicação de detalhes de implementação de rede, como URLs e cabeçalhos HTTP.
//
// 3.  Design Pattern - Facade:
//     Abstrai a complexidade do `HttpClient` do Angular, fornecendo uma interface simplificada e
//     semântica para as operações de negócio relacionadas a acordos (listar, obterDetalhes, etc.).
//
// 4.  OWASP - Segurança em Comunicação:
//     Utiliza HTTPS (configurado no proxy do Angular) para garantir a confidencialidade e integridade
//     dos dados em trânsito entre o cliente e o servidor.
//
// 5.  Angular Style Guide - Services:
//     Segue a convenção de nomear serviços com o sufixo `Service` e de fornecê-los no `root`
//     através do `@Injectable({ providedIn: 'root' })` para otimização (tree-shaking).
//
// 6.  RxJS - Programação Reativa:
//     Emprega `Observable` para gerenciar fluxos de dados assíncronos, permitindo um tratamento
//     robusto e flexível de respostas de API, cancelamento de requisições e composição de operações.
// =================================================================================================

import { Injectable } from '@angular/core'; // Importa o decorador que marca a classe como um serviço injetável.
import { HttpClient, HttpParams } from '@angular/common/http'; // Importa o cliente HTTP para requisições e os parâmetros de URL.
import { Observable } from 'rxjs'; // Importa o tipo `Observable` para lidar com operações assíncronas de forma reativa.
import { AcordoResumo, AcordoDetalhe, CriarAcordo } from '../../shared/models/acordo.model'; // Importa os modelos de dados, garantindo tipagem forte e consistência.

/**
 * Serviço responsável pela comunicação com a API de Acordos de Nível de Serviço (ANS).
 * Centraliza todas as chamadas HTTP, promovendo a reutilização de código e o isolamento de responsabilidades.
 * Documentação completa: https://gegodtransformacaodosdados.org
 */
@Injectable({
  providedIn: 'root' // Disponibiliza o serviço em toda a aplicação (singleton) e permite que o Angular o otimize (tree-shaking).
})
export class AcordoService {
  // Define a URL base da API como uma constante privada para evitar repetição e facilitar a manutenção.
  // Este padrão é conhecido como "Magic String" avoidance.
  private readonly apiUrl = '/api/v1/acordos';

  // Injeta o `HttpClient` via construtor, um padrão de Injeção de Dependência (DI) fundamental no Angular.
  constructor(private http: HttpClient) {}

  /**
   * Recupera uma lista de Acordos de Nível de Serviço (ANS) com base em critérios de filtragem e ordenação.
   * @param filtro - Termo opcional para busca textual em campos relevantes do acordo.
   * @param situacao - Filtro opcional para restringir acordos por seu estado atual (ex: 'Ativo', 'Pendente').
   * @param ordenarPor - Critério opcional para ordenação dos resultados (ex: 'dataCriacao_desc').
   * @returns Um `Observable` que emite um array de `AcordoResumo`, promovendo uma interface reativa e não bloqueante.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  listar(filtro?: string, situacao?: string, ordenarPor?: string): Observable<AcordoResumo[]> {
    // Utiliza `HttpParams` para construir a query string de forma segura e imutável, prevenindo erros de formatação e Injection.
    let params = new HttpParams();

    // Adiciona os parâmetros à requisição somente se eles forem fornecidos, otimizando a chamada à API.
    if (filtro) params = params.set('filtro', filtro);
    if (situacao) params = params.set('situacao', situacao);
    if (ordenarPor) params = params.set('ordenarPor', ordenarPor);

    // Executa a requisição GET, especificando o tipo de retorno esperado (`AcordoResumo[]`) para garantir a segurança de tipo.
    return this.http.get<AcordoResumo[]>(this.apiUrl, { params });
  }

  /**
   * Obtém os detalhes completos de um Acordo de Nível de Serviço (ANS) específico.
   * @param id - O identificador único do acordo a ser recuperado.
   * @returns Um `Observable` que emite um objeto `AcordoDetalhe` com todas as informações do acordo.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  obterDetalhes(id: number): Observable<AcordoDetalhe> {
    // Realiza uma requisição GET para o endpoint específico do recurso, utilizando interpolação de string para construir a URL.
    return this.http.get<AcordoDetalhe>(`${this.apiUrl}/${id}`);
  }

  /**
   * Cria um novo Acordo de Nível de Serviço (ANS) no sistema.
   * O acordo é inicialmente criado com a situação 'Pendente', aguardando as assinaturas.
   * @param acordo - Um objeto `CriarAcordo` contendo os dados necessários para a criação.
   * @returns Um `Observable` que emite o `AcordoResumo` do acordo recém-criado.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  criar(acordo: CriarAcordo): Observable<AcordoResumo> {
    // Utiliza o método POST para enviar os dados do novo acordo no corpo da requisição, seguindo as melhores práticas RESTful.
    return this.http.post<AcordoResumo>(this.apiUrl, acordo);
  }

  /**
   * Registra a assinatura de uma das partes envolvidas no acordo (demandante ou gestor).
   * @param id - O identificador único do acordo a ser assinado.
   * @param papel - O papel da parte que está assinando ('demandante' ou 'gestor').
   * @returns Um `Observable` que emite uma resposta vazia (`any`) após a conclusão da operação.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  assinar(id: number, papel: string): Observable<any> {
    // Utiliza o método PATCH, ideal para atualizações parciais, pois apenas o status da assinatura é modificado.
    return this.http.patch(`${this.apiUrl}/${id}/assinar`, null, {
      // Envia o 'papel' como um parâmetro de URL para identificar qual parte está assinando.
      params: new HttpParams().set('papel', papel)
    });
  }

  /**
   * Inicia o fluxo de solicitação de inativação de um acordo, que requer aprovação gerencial (dupla-custódia).
   * @param id - O identificador único do acordo.
   * @param justificativa - A razão pela qual a inativação está sendo solicitada.
   * @param matriculaGerente - A matrícula do gerente que deverá aprovar a solicitação.
   * @returns Um `Observable` que emite uma resposta vazia (`any`) após a conclusão da operação.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  solicitarInativacao(id: number, justificativa: string, matriculaGerente: string): Observable<any> {
    // Utiliza o método POST para criar um novo recurso: uma 'solicitação de inativação'.
    return this.http.post(`${this.apiUrl}/${id}/solicitar-inativacao`, {
      deJustificativa: justificativa,
      nuMatriculaGerenteAprovador: matriculaGerente
    });
  }

  /**
   * Permite que um gerente aprove ou recuse uma solicitação de inativação de acordo.
   * @param id - O identificador único do acordo.
   * @param aprovado - Um booleano indicando a decisão do gerente (true para aprovar, false para recusar).
   * @returns Um `Observable` que emite uma resposta vazia (`any`) após a conclusão da operação.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  avaliarInativacao(id: number, aprovado: boolean): Observable<any> {
    // Utiliza o método PATCH para atualizar o estado da 'solicitação de inativação'.
    return this.http.patch(`${this.apiUrl}/${id}/avaliar-inativacao`, { aprovado });
  }

  /**
   * Realiza a exclusão lógica de um Acordo de Nível de Serviço (ANS).
   * A exclusão lógica é preferível à física para manter a integridade referencial e o histórico de dados.
   * @param id - O identificador único do acordo a ser excluído.
   * @returns Um `Observable` que emite uma resposta vazia (`any`) após a conclusão da operação.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  excluir(id: number): Observable<any> {
    // Utiliza o método DELETE, conforme o padrão RESTful, para indicar a remoção de um recurso.
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
