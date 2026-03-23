// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Reactive Forms: Sistema do Angular para criar formularios com validacao.
// - FormGroup: Agrupamento de campos do formulario.
// - Validators: Regras de validacao (campo obrigatorio, formato etc.).
// ============================================================

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { AcordoService } from '../../core/services/acordo.service';

/**
 * Componente de formulario para inclusao de um novo ANS.
 * Utiliza Reactive Forms com validacoes sincronas.
 * Integra-se ao Design System CAIXA (DSC).
 */
@Component({
  selector: 'app-acordo-form',
  template: `
    <div class="caixa-page-header">
      <h1>Incluir Novo Acordo de Nivel de Servico</h1>
    </div>

    <form [formGroup]="form" (ngSubmit)="salvar()" class="caixa-form">

      <!-- Secao: Partes do Acordo -->
      <fieldset class="caixa-fieldset">
        <legend>Partes do Acordo</legend>

        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label for="noFornecedora">Fornecedora dos Dados *</label>
            <input id="noFornecedora" formControlName="noFornecedora" class="caixa-input" />
            <span class="caixa-error" *ngIf="form.get('noFornecedora')?.invalid && form.get('noFornecedora')?.touched">
              Campo obrigatorio.
            </span>
          </div>

          <div class="caixa-form-group">
            <label for="noConsumidora">Consumidora dos Dados *</label>
            <input id="noConsumidora" formControlName="noConsumidora" class="caixa-input" />
          </div>
        </div>

        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label for="noUnidadeFornecedora">Unidade Responsavel (Fornecedora) *</label>
            <input id="noUnidadeFornecedora" formControlName="noUnidadeFornecedora" class="caixa-input" />
          </div>

          <div class="caixa-form-group">
            <label for="noUnidadeConsumidora">Unidade Responsavel (Consumidora) *</label>
            <input id="noUnidadeConsumidora" formControlName="noUnidadeConsumidora" class="caixa-input" />
          </div>
        </div>
      </fieldset>

      <!-- Secao: Vigencia e Classificacao -->
      <fieldset class="caixa-fieldset">
        <legend>Vigencia e Classificacao</legend>

        <div class="caixa-form-row">
          <div class="caixa-form-group">
            <label for="qtDiasVigencia">Periodo de Vigencia (dias) *</label>
            <input id="qtDiasVigencia" type="number" formControlName="qtDiasVigencia" class="caixa-input" />
          </div>

          <div class="caixa-form-group">
            <label for="dePeriodicidade">Periodicidade de Atualizacao *</label>
            <select id="dePeriodicidade" formControlName="dePeriodicidade" class="caixa-select">
              <option value="Diaria">Diaria</option>
              <option value="Semanal">Semanal</option>
              <option value="Mensal">Mensal</option>
              <option value="Trimestral">Trimestral</option>
              <option value="Sob demanda">Sob demanda</option>
            </select>
          </div>

          <div class="caixa-form-group">
            <label for="coGrauSigilo">Grau de Sigilo (MN OR016) *</label>
            <select id="coGrauSigilo" formControlName="coGrauSigilo" class="caixa-select">
              <option value="Publico">Publico</option>
              <option value="Interno">Interno</option>
              <option value="Confidencial">Confidencial</option>
              <option value="Restrito">Restrito</option>
              <option value="Secreto">Secreto</option>
            </select>
          </div>
        </div>

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
              Os dados possuem dados pessoais sensiveis
            </label>
          </div>
        </div>
      </fieldset>

      <!-- Secao: Responsavel Tecnico - Fornecedora -->
      <fieldset class="caixa-fieldset" formGroupName="responsavelFornecedora">
        <legend>Responsavel Tecnico (Fornecedora)</legend>
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
            <label>Matricula</label>
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

      <!-- Secao: Responsavel Tecnico - Consumidora -->
      <fieldset class="caixa-fieldset" formGroupName="responsavelConsumidora">
        <legend>Responsavel Tecnico (Consumidora)</legend>
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
            <label>Matricula</label>
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

      <!-- Botoes -->
      <div class="caixa-form-actions">
        <button type="submit" class="caixa-btn caixa-btn-primary" [disabled]="form.invalid">
          Salvar ANS
        </button>
        <button type="button" class="caixa-btn caixa-btn-secondary" (click)="cancelar()">
          Cancelar
        </button>
      </div>
    </form>
  `
})
export class AcordoFormComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private acordoService: AcordoService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      noFornecedora: ['', Validators.required],
      noConsumidora: ['', Validators.required],
      noUnidadeFornecedora: ['', Validators.required],
      noUnidadeConsumidora: ['', Validators.required],
      qtDiasVigencia: [365, [Validators.required, Validators.min(1)]],
      dePeriodicidade: ['Mensal', Validators.required],
      coGrauSigilo: ['Interno', Validators.required],
      icDadoPessoal: [false],
      icDadoSensivel: [false],
      responsavelFornecedora: this.fb.group({
        noPessoa: ['', Validators.required],
        nuCpf: ['', [Validators.required, Validators.minLength(11)]],
        nuMatricula: [''],
        nuTelefone: [''],
        deEmail: ['', [Validators.required, Validators.email]]
      }),
      responsavelConsumidora: this.fb.group({
        noPessoa: ['', Validators.required],
        nuCpf: ['', [Validators.required, Validators.minLength(11)]],
        nuMatricula: [''],
        nuTelefone: [''],
        deEmail: ['', [Validators.required, Validators.email]]
      })
    });
  }

  salvar(): void {
    if (this.form.valid) {
      this.acordoService.criar(this.form.value).subscribe({
        next: (resultado) => {
          console.log('ANS criado com sucesso:', resultado);
          // Navegar para a listagem
        },
        error: (erro) => {
          console.error('Erro ao criar ANS:', erro);
        }
      });
    }
  }

  cancelar(): void {
    // Navegar de volta para a listagem
    console.log('Cancelar e voltar');
  }
}
