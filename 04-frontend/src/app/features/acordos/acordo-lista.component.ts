// =================================================================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// =================================================================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// 1.  **Single Responsibility Principle (SRP) - SOLID**: Este componente tem a responsabilidade única
//     de apresentar e gerenciar a lista de Acordos de Nível de Serviço (ANS). A lógica de
//     negócio para buscar os dados está encapsulada no `AcordoService`, em conformidade com o SRP.
//
// 2.  **Clean Architecture**: A estrutura separa claramente as camadas de apresentação (template HTML),
//     lógica de visualização (classe do componente) e serviços de dados (`AcordoService`).
//     Isso promove um baixo acoplamento e alta coesão, facilitando a manutenção e os testes.
//
// 3.  **Component-Based Architecture (Angular)**: O código é modularizado em um componente (`AcordoListaComponent`),
//     promovendo o reuso e a organização do código, conforme preconiza o Angular Style Guide.
//
// 4.  **Reactive Programming (RxJS)**: A chamada ao serviço (`acordoService.listar`) retorna um
//     Observable. A inscrição (`.subscribe()`) é uma prática reativa que lida com fluxos de
//     dados assíncronos de forma eficiente e declarativa.
//
// 5.  **Separation of Concerns**: O template HTML está contido diretamente no decorator `@Component`
//     (usando `template`) para componentes pequenos e focados. Para templates maiores, a boa
//     prática seria usar `templateUrl`, separando o HTML em seu próprio arquivo.
//
// 6.  **OWASP Top 10 - Security**: Embora este componente não lide diretamente com dados sensíveis,
//     a utilização do framework Angular já mitiga riscos comuns como Cross-Site Scripting (XSS)
//     através da sanitização automática de dados interpolados no template (e.g., `{{ acordo.coAcordo }}`).
//
// 7.  **Angular Style Guide**: O nome do arquivo (`acordo-lista.component.ts`), o seletor
//     (`app-acordo-lista`) e o nome da classe (`AcordoListaComponent`) seguem as convenções
//     oficiais, melhorando a legibilidade e a consistência do projeto.
// =================================================================================================

// Importações de módulos e dependências essenciais do Angular e do projeto.
import { Component, OnInit } from '@angular/core'; // `Component` para definir metadados e `OnInit` para o ciclo de vida.
import { AcordoService } from '../../core/services/acordo.service'; // Serviço que encapsula a lógica de acesso aos dados de acordos.
import { AcordoResumo } from '../../shared/models/acordo.model'; // Modelo de dados que define a estrutura de um resumo de acordo.

/**
 * @Component AcordoListaComponent
 * @description
 * Componente responsável pela visualização e interação com a lista de Acordos de Nível de Serviço (ANS).
 * Ele orquestra a exibição de dados em uma tabela, permite a aplicação de filtros, ordenação e
 * dispara ações como a visualização de detalhes ou a criação de um novo acordo.
 * A interface do usuário é construída em alinhamento com o Design System da CAIXA (DSC) para
 * garantir consistência visual e usabilidade.
 * @see Documentação completa: https://gegodtransformacaodosdados.org
 */
@Component({
  // Seletor CSS que permite o uso deste componente em outros templates HTML, como `<app-acordo-lista></app-acordo-lista>`.
  selector: 'app-acordo-lista',

  // Template inline: define a estrutura HTML e o data binding do componente.
  // Esta abordagem é ideal para componentes com uma marcação concisa e diretamente acoplada à sua lógica.
  template: `
    <!-- Cabeçalho da Página: Contextualiza o usuário sobre a funcionalidade da tela. -->
    <div class="caixa-page-header">
      <h1>Acordos de Nível de Serviço (ANS)</h1>
      <p>Gerencie os acordos de compartilhamento de dados do conglomerado CAIXA.</p>
    </div>

    <!-- Barra de Filtros: Oferece controles para refinar a lista de acordos exibida. -->
    <div class="caixa-filter-bar">
      <!-- Campo de pesquisa textual: Two-way data binding com a propriedade 'filtro'. -->
      <div class="caixa-input-group">
        <label for="filtro">Pesquisar</label>
        <input
          id="filtro"
          type="text"
          class="caixa-input"
          placeholder="Buscar por fornecedora, consumidora..."
          [(ngModel)]="filtro" <!-- Sincroniza o valor do input com a propriedade 'filtro'. -->
          (input)="pesquisar()" /> <!-- Dispara a pesquisa a cada caractere digitado. -->
      </div>

      <!-- Filtro por Situação: Permite selecionar o status do acordo. -->
      <div class="caixa-input-group">
        <label for="situacao">Situação</label>
        <select id="situacao" class="caixa-select" [(ngModel)]="situacao" (change)="pesquisar()">
          <option value="">Todas</option>
          <option value="Pendente">Pendente</option>
          <option value="Ativo">Ativo</option>
          <option value="Inativo">Inativo</option>
          <option value="PendenteInativacao">Pendente Inativação</option>
        </select>
      </div>

      <!-- Ordenação da Lista: Define o critério para ordenar os resultados. -->
      <div class="caixa-input-group">
        <label for="ordenar">Ordenar por</label>
        <select id="ordenar" class="caixa-select" [(ngModel)]="ordenarPor" (change)="pesquisar()">
          <option value="criacao">Data de Criação</option>
          <option value="vigencia">Vigência</option>
          <option value="coparticipada">Coparticipada</option>
        </select>
      </div>

      <!-- Botão de Ação Principal: Inicia o fluxo de criação de um novo acordo. -->
      <button class="caixa-btn caixa-btn-primary" (click)="novoAcordo()">
        + Novo ANS
      </button>
    </div>

    <!-- Tabela de Resultados: Exibe os dados dos acordos de forma estruturada. -->
    <table class="caixa-table">
      <thead>
        <tr>
          <th>Código</th>
          <th>Fornecedora</th>
          <th>Consumidora</th>
          <th>Situação</th>
          <th>Vigência (dias)</th>
          <th>Dados Pessoais</th>
          <th>Ações</th>
        </tr>
      </thead>
      <tbody>
        <!-- Itera sobre a lista de 'acordos' e renderiza uma linha para cada item. -->
        <tr *ngFor="let acordo of acordos">
          <td>{{ acordo.coAcordo }}</td>
          <td>{{ acordo.noFornecedora }}</td>
          <td>{{ acordo.noConsumidora }}</td>
          <td>
            <!-- Badge de Situação: Usa uma classe CSS dinâmica para colorir o status. -->
            <span [class]="'caixa-badge caixa-badge-' + acordo.situacao.toLowerCase()">
              {{ acordo.situacao }}
            </span>
          </td>
          <td>{{ acordo.qtDiasVigencia }}</td>
          <td>{{ acordo.icDadoPessoal ? 'Sim' : 'Não' }}</td> <!-- Operador ternário para exibir texto amigável. -->
          <td>
            <!-- Ação de Detalhes: Navega para a visualização completa do acordo. -->
            <button class="caixa-btn-sm" (click)="verDetalhes(acordo.coAcordo)">Detalhes</button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class AcordoListaComponent implements OnInit {
  // Propriedade para armazenar a lista de acordos recuperada do serviço.
  acordos: AcordoResumo[] = [];

  // Propriedades para o two-way data binding com os controles de filtro e ordenação.
  filtro = '';
  situacao = '';
  ordenarPor = 'criacao'; // Valor padrão para a ordenação.

  /**
   * @constructor
   * @description
   * O construtor é responsável pela Injeção de Dependência (DI).
   * O `AcordoService` é injetado para que o componente possa consumir seus métodos,
   * desacoplando a lógica de busca de dados da lógica de apresentação.
   * @param acordoService Instância do serviço que provê os dados de acordos.
   */
  constructor(private acordoService: AcordoService) {}

  /**
   * @method ngOnInit
   * @description
   * Gancho do ciclo de vida do Angular, executado uma vez após a inicialização do componente.
   * É o local ideal para realizar a carga inicial de dados.
   */
  ngOnInit(): void {
    // Dispara a pesquisa inicial para popular a tabela assim que o componente é carregado.
    this.pesquisar();
  }

  /**
   * @method pesquisar
   * @description
   * Executa a consulta de acordos utilizando o `AcordoService`.
   * Os parâmetros de filtro e ordenação são passados para o serviço, que retorna um Observable.
   * A inscrição neste Observable atualiza a propriedade `acordos` com os dados recebidos.
   */
  pesquisar(): void {
    // Chama o método `listar` do serviço, passando os valores atuais dos filtros.
    this.acordoService.listar(this.filtro, this.situacao, this.ordenarPor)
      // O `.subscribe()` escuta a resposta do Observable. Quando os dados chegam, a função de callback é executada.
      .subscribe(dados => this.acordos = dados);
  }

  /**
   * @method verDetalhes
   * @description
   * Função chamada ao clicar no botão 'Detalhes'.
   * Em uma implementação completa, esta função utilizaria o serviço `Router` do Angular
   * para navegar para uma nova rota, passando o ID do acordo como parâmetro.
   * @param id O identificador único do acordo a ser detalhado.
   */
  verDetalhes(id: number): void {
    // Placeholder para a lógica de navegação. Ex: this.router.navigate(['/acordos', id]);
    console.log('Navegação para detalhes do acordo:', id);
  }

  /**
   * @method novoAcordo
   * @description
   * Função chamada ao clicar no botão '+ Novo ANS'.
   * Seria responsável por navegar para o formulário de criação de um novo acordo.
   */
  novoAcordo(): void {
    // Placeholder para a lógica de navegação. Ex: this.router.navigate(['/acordos/novo']);
    console.log('Navegação para o formulário de um novo acordo.');
  }
}
