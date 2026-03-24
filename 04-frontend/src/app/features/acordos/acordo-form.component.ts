// =================================================================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// =================================================================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados
//
// 1.  **SOLID Principles**:
//     -   **Single Responsibility Principle (SRP)**: O componente `AcordoFormComponent` tem a
//         responsabilidade única de gerenciar o formulário de acordos (criação, validação e submissão),
//         delegando a persistência dos dados ao `AcordoService`.
//
// 2.  **Clean Architecture**:
//     -   A separação de responsabilidades entre o componente (camada de apresentação) e o serviço
//         (camada de aplicação/infraestrutura) adere aos princípios da Clean Architecture, promovendo
//         baixo acoplamento e alta coesão.
//
// 3.  **Design Patterns**:
//     -   **Reactive Forms**: Utiliza o padrão de formulários reativos do Angular, que favorece a
//         imutabilidade e o rastreamento de mudanças de forma explícita e síncrona.
//     -   **Builder Pattern**: O `FormBuilder` é uma implementação do padrão Builder, simplificando a
//         construção de instâncias complexas de `FormGroup` e `FormControl`.
//
// 4.  **Angular Style Guide**:
//     -   O código segue as diretrizes oficiais do Angular, como o uso de `ngOnInit` para inicialização
//         de lógica complexa e a nomeação de seletores com prefixo `app-`.
//
// 5.  **RxJS (Reactive Extensions for JavaScript)**:
//     -   O método `subscribe` é utilizado para consumir o `Observable` retornado pelo `AcordoService`,
//         permitindo uma manipulação assíncrona e reativa das respostas HTTP.
//
// 6.  **OWASP (Open Web Application Security Project)**:
//     -   **Data Validation**: A validação de todos os dados de entrada no frontend (`Validators`)
//         é uma prática de segurança essencial para prevenir ataques como Injeção de Dados.
//         A validação robusta deve ser sempre replicada no backend.
// =================================================================================================

// Importa os módulos essenciais do Angular para a criação de componentes e formulários reativos.
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

// Importa o serviço responsável pela comunicação com a API de Acordos, abstraindo a lógica de negócio.
import { AcordoService } from '../../core/services/acordo.service';

/**
 * @class AcordoFormComponent
 * @description Componente responsável pela interface de criação e edição de um Acordo de Nível de Serviço (ANS).
 * Este componente utiliza o padrão Reactive Forms do Angular para um controle robusto e escalável do estado do formulário.
 * A estrutura segue as melhores práticas de arquitetura, separando a lógica de apresentação da lógica de negócio.
 * Documentação completa: https://gegodtransformacaodosdados.org
 */
@Component({
  // Define o seletor CSS que será usado para instanciar este componente no HTML.
  selector: 'app-acordo-form',
  // O template é definido inline para manter a simplicidade e coesão, ideal para componentes com pouca marcação.
  template: `
    <!-- Cabeçalho da página, seguindo o padrão de design visual da CAIXA. -->
    <div class="caixa-page-header">
      <h1>Incluir Novo Acordo de Nível de Serviço</h1>
    </div>

    <!-- O formulário é associado ao FormGroup 'form' e o evento 'ngSubmit' é mapeado para o método 'salvar'. -->
    <form [formGroup]="form" (ngSubmit)="salvar()" class="caixa-form">

      <!-- Agrupamento de campos relacionados às partes envolvidas no acordo. -->
      <fieldset class="caixa-fieldset">
        <legend>Partes do Acordo</legend>

        <!-- Linha do formulário para organizar os campos horizontalmente. -->
        <div class="caixa-form-row">
          <!-- Campo para a área fornecedora dos dados. -->
          <div class="caixa-form-group">
            <label for="noFornecedora">Fornecedora dos Dados *</label>
            <input id="noFornecedora" formControlName="noFornecedora" class="caixa-input" />
            <!-- Mensagem de erro exibida apenas se o campo for inválido e tocado pelo usuário. -->
            <span class="caixa-error" *ngIf="form.get('noFornecedora')?.invalid && form.get('noFornecedora')?.touched">
              Campo obrigatório.
            </span>
          </div>

          <!-- Campo para a área consumidora dos dados. -->
          <div class="caixa-form-group">
            <label for="noConsumidora">Consumidora dos Dados *</label>
            <input id="noConsumidora" formControlName="noConsumidora" class="caixa-input" />
          </div>
        </div>

        <!-- Segunda linha de campos para as unidades responsáveis. -->
        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label for="noUnidadeFornecedora">Unidade Responsável (Fornecedora) *</label>
            <input id="noUnidadeFornecedora" formControlName="noUnidadeFornecedora" class="caixa-input" />
          </div>

          <div class="caixa-form-group">
            <label for="noUnidadeConsumidora">Unidade Responsável (Consumidora) *</label>
            <input id="noUnidadeConsumidora" formControlName="noUnidadeConsumidora" class="caixa-input" />
          </div>
        </div>
      </fieldset>

      <!-- Agrupamento de campos relacionados à vigência e classificação do acordo. -->
      <fieldset class="caixa-fieldset">
        <legend>Vigência e Classificação</legend>

        <div class="caixa-form-row">
          <!-- Campo numérico para o período de vigência do acordo. -->
          <div class="caixa-form-group">
            <label for="qtDiasVigencia">Período de Vigência (dias) *</label>
            <input id="qtDiasVigencia" type="number" formControlName="qtDiasVigencia" class="caixa-input" />
          </div>

          <!-- Campo de seleção para a periodicidade de atualização dos dados. -->
          <div class="caixa-form-group">
            <label for="dePeriodicidade">Periodicidade de Atualização *</label>
            <select id="dePeriodicidade" formControlName="dePeriodicidade" class="caixa-select">
              <option value="Diaria">Diária</option>
              <option value="Semanal">Semanal</option>
              <option value="Mensal">Mensal</option>
              <option value="Trimestral">Trimestral</option>
              <option value="Sob demanda">Sob demanda</option>
            </select>
          </div>

          <!-- Campo de seleção para o grau de sigilo da informação, conforme normativo. -->
          <div class="caixa-form-group">
            <label for="coGrauSigilo">Grau de Sigilo (MN OR016) *</label>
            <select id="coGrauSigilo" formControlName="coGrauSigilo" class="caixa-select">
              <option value="Publico">Público</option>
              <option value="Interno">Interno</option>
              <option value="Confidencial">Confidencial</option>
              <option value="Restrito">Restrito</option>
              <option value="Secreto">Secreto</option>
            </select>
          </div>
        </div>

        <!-- Campos de checkbox para classificação de dados conforme a LGPD. -->
        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label>
              <input type="checkbox" formControlName="icDadoPessoal" />
              Os dados possuem dados pessoais (LGPD)
            </label>
          </div>
          <div class="caixa-form-group">
            <label>
              <input type="checkbox" formControlName="icDadoSensivel" />
              Os dados possuem dados pessoais sensíveis
            </label>
          </div>
        </div>
      </fieldset>

      <!-- Seção dedicada ao responsável técnico da área fornecedora, utilizando um FormGroup aninhado. -->
      <fieldset class="caixa-fieldset" formGroupName="responsavelFornecedora">
        <legend>Responsável Técnico (Fornecedora)</legend>
        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label>Nome *</label>
            <input formControlName="noPessoa" class="caixa-input" />
          </div>
          <div class="caixa-form-group">
            <label>CPF *</label>
            <input formControlName="nuCpf" class="caixa-input" maxlength="11" />
          </div>
          <div class="caixa-form-group">
            <label>Matrícula</label>
            <input formControlName="nuMatricula" class="caixa-input" />
          </div>
        </div>
        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label>Telefone</label>
            <input formControlName="nuTelefone" class="caixa-input" />
          </div>
          <div class="caixa-form-group">
            <label>E-mail *</label>
            <input formControlName="deEmail" class="caixa-input" type="email" />
          </div>
        </div>
      </fieldset>

      <!-- Seção para o responsável técnico da área consumidora, espelhando a estrutura da fornecedora. -->
      <fieldset class="caixa-fieldset" formGroupName="responsavelConsumidora">
        <legend>Responsável Técnico (Consumidora)</legend>
        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label>Nome *</label>
            <input formControlName="noPessoa" class="caixa-input" />
          </div>
          <div class="caixa-form-group">
            <label>CPF *</label>
            <input formControlName="nuCpf" class="caixa-input" maxlength="11" />
          </div>
          <div class="caixa-form-group">
            <label>Matrícula</label>
            <input formControlName="nuMatricula" class="caixa-input" />
          </div>
        </div>
        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label>Telefone</label>
            <input formControlName="nuTelefone" class="caixa-input" />
          </div>
          <div class="caixa-form-group">
            <label>E-mail *</label>
            <input formControlName="deEmail" class="caixa-input" type="email" />
          </div>
        </div>
      </fieldset>

      <!-- Bloco de ações do formulário, contendo os botões de submissão e cancelamento. -->
      <div class="caixa-form-actions">
        <!-- O botão de salvar é desabilitado dinamicamente se o formulário for inválido, prevenindo submissões incorretas. -->
        <button type="submit" class="caixa-btn caixa-btn-primary" [disabled]="form.invalid">
          Salvar ANS
        </button>
        <!-- O botão de cancelar dispara o método 'cancelar' para reverter a ação. -->
        <button type="button" class="caixa-btn caixa-btn-secondary" (click)="cancelar()">
          Cancelar
        </button>
      </div>
    </form>
  `
})
export class AcordoFormComponent implements OnInit {
  // Declaração da propriedade 'form' que abrigará a instância do FormGroup.
  // O '!' (Non-null assertion operator) indica ao TypeScript que esta propriedade será inicializada em tempo de execução (no ngOnInit).
  form!: FormGroup;

  /**
   * @constructor
   * @param {FormBuilder} fb - Injeção de dependência do FormBuilder para facilitar a criação de formulários reativos.
   * @param {AcordoService} acordoService - Injeção do serviço que encapsula a lógica de negócio e acesso a dados de acordos.
   */
  constructor(
    private fb: FormBuilder,
    private acordoService: AcordoService
  ) {}

  /**
   * @method ngOnInit
   * @description Lifecycle hook do Angular chamado após a inicialização do componente.
   * É o local ideal para construir o formulário e definir suas validações, garantindo que o modelo esteja pronto antes da renderização.
   */
  ngOnInit(): void {
    // Utiliza o FormBuilder para criar a estrutura do formulário com seus respectivos campos e validadores.
    this.form = this.fb.group({
      // --- Campos Principais do Acordo ---
      noFornecedora: ['', Validators.required], // Nome da área fornecedora é obrigatório.
      noConsumidora: ['', Validators.required], // Nome da área consumidora é obrigatório.
      noUnidadeFornecedora: ['', Validators.required], // Unidade da fornecedora é obrigatória.
      noUnidadeConsumidora: ['', Validators.required], // Unidade da consumidora é obrigatória.
      qtDiasVigencia: [365, [Validators.required, Validators.min(1)]], // Vigência padrão de 365 dias, obrigatória e no mínimo 1.
      dePeriodicidade: ['Mensal', Validators.required], // Periodicidade padrão 'Mensal', obrigatória.
      coGrauSigilo: ['Interno', Validators.required], // Grau de sigilo padrão 'Interno', obrigatório.
      icDadoPessoal: [false], // Indicador de dado pessoal (LGPD), não obrigatório.
      icDadoSensivel: [false], // Indicador de dado sensível (LGPD), não obrigatório.

      // --- FormGroup Aninhado para o Responsável da Fornecedora ---
      responsavelFornecedora: this.fb.group({
        noPessoa: ['', Validators.required], // Nome do responsável é obrigatório.
        nuCpf: ['', [Validators.required, Validators.minLength(11)]], // CPF com 11 dígitos é obrigatório.
        nuMatricula: [''], // Matrícula é opcional.
        nuTelefone: [''], // Telefone é opcional.
        deEmail: ['', [Validators.required, Validators.email]] // E-mail é obrigatório e deve ter formato válido.
      }),

      // --- FormGroup Aninhado para o Responsável da Consumidora ---
      responsavelConsumidora: this.fb.group({
        noPessoa: ['', Validators.required], // Nome do responsável é obrigatório.
        nuCpf: ['', [Validators.required, Validators.minLength(11)]], // CPF com 11 dígitos é obrigatório.
        nuMatricula: [''], // Matrícula é opcional.
        nuTelefone: [''], // Telefone é opcional.
        deEmail: ['', [Validators.required, Validators.email]] // E-mail é obrigatório e deve ter formato válido.
      })
    });
  }

  /**
   * @method salvar
   * @description Método chamado na submissão do formulário.
   * Realiza uma última verificação de validade e, se positivo, envia os dados para o serviço de persistência.
   */
  salvar(): void {
    // A verificação 'this.form.valid' garante que a submissão só ocorra se todas as regras de validação forem atendidas.
    if (this.form.valid) {
      // Chama o método 'criar' do serviço, passando o valor bruto do formulário ('this.form.value').
      this.acordoService.criar(this.form.value).subscribe({
        // 'next' é chamado em caso de sucesso na requisição HTTP.
        next: (resultado) => {
          console.log('ANS criado com sucesso:', resultado); // Log de sucesso para fins de depuração.
          // TODO: Implementar navegação para a tela de listagem ou exibir uma mensagem de sucesso ao usuário.
        },
        // 'error' é chamado se a requisição falhar.
        error: (erro) => {
          console.error('Erro ao criar ANS:', erro); // Log de erro detalhado para facilitar a investigação.
          // TODO: Implementar uma estratégia de feedback ao usuário (ex: Toast, Modal) informando sobre o erro.
        }
      });
    }
  }

  /**
   * @method cancelar
   * @description Método para cancelar a operação de criação/edição.
   * Atualmente, apenas registra no console, mas deve ser expandido para retornar à tela anterior.
   */
  cancelar(): void {
    // Ação de cancelamento, idealmente, deveria redirecionar o usuário.
    console.log('Operação cancelada pelo usuário.');
    // TODO: Implementar a navegação para a rota anterior (ex: /acordos/listar) usando o Router do Angular.
  }
}
