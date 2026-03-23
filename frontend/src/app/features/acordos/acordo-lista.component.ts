// ============================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Component (Angular): Bloco visual da interface (uma tela ou parte dela).
// - OnInit: Metodo executado automaticamente quando o componente e carregado.
// - Template: Codigo HTML que define a aparencia do componente.
// ============================================================

import { Component, OnInit } from '@angular/core';
import { AcordoService } from '../../core/services/acordo.service';
import { AcordoResumo } from '../../shared/models/acordo.model';

/**
 * Componente de listagem de Acordos de Nivel de Servico.
 * Exibe uma tabela com filtros, ordenacao e acoes rapidas.
 * Integra-se ao Design System CAIXA (DSC) para a interface visual.
 */
@Component({
  selector: 'app-acordo-lista',
  template: `
    <!-- Cabecalho da pagina -->
    <div class="caixa-page-header">
      <h1>Acordos de Nivel de Servico (ANS)</h1>
      <p>Gerencie os acordos de compartilhamento de dados do conglomerado CAIXA.</p>
    </div>

    <!-- Barra de filtros -->
    <div class="caixa-filter-bar">
      <div class="caixa-input-group">
        <label for="filtro">Pesquisar</label>
        <input
          id="filtro"
          type="text"
          class="caixa-input"
          placeholder="Buscar por fornecedora, consumidora..."
          [(ngModel)]="filtro"
          (input)="pesquisar()" />
      </div>

      <div class="caixa-input-group">
        <label for="situacao">Situacao</label>
        <select id="situacao" class="caixa-select" [(ngModel)]="situacao" (change)="pesquisar()">
          <option value="">Todas</option>
          <option value="Pendente">Pendente</option>
          <option value="Ativo">Ativo</option>
          <option value="Inativo">Inativo</option>
          <option value="PendenteInativacao">Pendente Inativacao</option>
        </select>
      </div>

      <div class="caixa-input-group">
        <label for="ordenar">Ordenar por</label>
        <select id="ordenar" class="caixa-select" [(ngModel)]="ordenarPor" (change)="pesquisar()">
          <option value="criacao">Data de Criacao</option>
          <option value="vigencia">Vigencia</option>
          <option value="coparticipada">Coparticipada</option>
        </select>
      </div>

      <button class="caixa-btn caixa-btn-primary" (click)="novoAcordo()">
        + Novo ANS
      </button>
    </div>

    <!-- Tabela de resultados -->
    <table class="caixa-table">
      <thead>
        <tr>
          <th>Codigo</th>
          <th>Fornecedora</th>
          <th>Consumidora</th>
          <th>Situacao</th>
          <th>Vigencia (dias)</th>
          <th>Dados Pessoais</th>
          <th>Acoes</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let acordo of acordos">
          <td>{{ acordo.coAcordo }}</td>
          <td>{{ acordo.noFornecedora }}</td>
          <td>{{ acordo.noConsumidora }}</td>
          <td>
            <span [class]="'caixa-badge caixa-badge-' + acordo.situacao.toLowerCase()">
              {{ acordo.situacao }}
            </span>
          </td>
          <td>{{ acordo.qtDiasVigencia }}</td>
          <td>{{ acordo.icDadoPessoal ? 'Sim' : 'Nao' }}</td>
          <td>
            <button class="caixa-btn-sm" (click)="verDetalhes(acordo.coAcordo)">Detalhes</button>
          </td>
        </tr>
      </tbody>
    </table>
  `
})
export class AcordoListaComponent implements OnInit {
  acordos: AcordoResumo[] = [];
  filtro = '';
  situacao = '';
  ordenarPor = 'criacao';

  constructor(private acordoService: AcordoService) {}

  ngOnInit(): void {
    this.pesquisar();
  }

  pesquisar(): void {
    this.acordoService.listar(this.filtro, this.situacao, this.ordenarPor)
      .subscribe(dados => this.acordos = dados);
  }

  verDetalhes(id: number): void {
    // Navegar para a tela de detalhes (Router)
    console.log('Navegar para detalhes do acordo:', id);
  }

  novoAcordo(): void {
    // Navegar para o formulario de inclusao
    console.log('Navegar para novo acordo');
  }
}
