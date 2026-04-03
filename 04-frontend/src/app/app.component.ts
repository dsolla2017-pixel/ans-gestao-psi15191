/**
 * Componente raiz da aplicação
 * Sistema de Gestão de ANS - PSI 15191 CAIXA
 * Autor: Diogo Solla Grawingholt
 *
 * Layout principal com toolbar institucional CAIXA,
 * sidenav de navegação e área de conteúdo com router-outlet.
 */
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <!-- Toolbar institucional CAIXA -->
    <mat-toolbar color="primary" class="app-toolbar">
      <button mat-icon-button (click)="sidenav.toggle()" aria-label="Menu">
        <mat-icon>menu</mat-icon>
      </button>
      <span class="app-title">CAIXA ANS</span>
      <span class="app-subtitle">Sistema de Gestão de Acordos de Nível de Serviço</span>
      <span class="spacer"></span>
      <button mat-icon-button matTooltip="Notificações" aria-label="Notificações">
        <mat-icon matBadge="3" matBadgeColor="warn">notifications</mat-icon>
      </button>
      <button mat-icon-button matTooltip="Perfil" aria-label="Perfil" [matMenuTriggerFor]="userMenu">
        <mat-icon>account_circle</mat-icon>
      </button>
      <mat-menu #userMenu="matMenu">
        <button mat-menu-item>
          <mat-icon>person</mat-icon>
          <span>Meu Perfil</span>
        </button>
        <button mat-menu-item>
          <mat-icon>settings</mat-icon>
          <span>Configurações</span>
        </button>
        <mat-divider></mat-divider>
        <button mat-menu-item>
          <mat-icon>exit_to_app</mat-icon>
          <span>Sair</span>
        </button>
      </mat-menu>
    </mat-toolbar>

    <!-- Layout com sidenav -->
    <mat-sidenav-container class="app-container">
      <!-- Menu lateral de navegação -->
      <mat-sidenav #sidenav mode="side" opened class="app-sidenav">
        <mat-nav-list>
          <a mat-list-item routerLink="/acordos" routerLinkActive="active">
            <mat-icon matListItemIcon>description</mat-icon>
            <span matListItemTitle>Acordos ANS</span>
          </a>
          <a mat-list-item routerLink="/acordos/novo" routerLinkActive="active">
            <mat-icon matListItemIcon>add_circle</mat-icon>
            <span matListItemTitle>Novo Acordo</span>
          </a>
          <mat-divider></mat-divider>
          <a mat-list-item routerLink="/dashboard" routerLinkActive="active">
            <mat-icon matListItemIcon>dashboard</mat-icon>
            <span matListItemTitle>Dashboard</span>
          </a>
          <a mat-list-item routerLink="/relatorios" routerLinkActive="active">
            <mat-icon matListItemIcon>assessment</mat-icon>
            <span matListItemTitle>Relatórios</span>
          </a>
          <mat-divider></mat-divider>
          <a mat-list-item routerLink="/auditoria" routerLinkActive="active">
            <mat-icon matListItemIcon>history</mat-icon>
            <span matListItemTitle>Auditoria</span>
          </a>
          <a mat-list-item routerLink="/configuracoes" routerLinkActive="active">
            <mat-icon matListItemIcon>settings</mat-icon>
            <span matListItemTitle>Configurações</span>
          </a>
        </mat-nav-list>

        <!-- Rodapé do sidenav -->
        <div class="sidenav-footer">
          <small>PSI 15191 | GEGOD</small>
          <br>
          <small>v1.0.0</small>
        </div>
      </mat-sidenav>

      <!-- Área de conteúdo principal -->
      <mat-sidenav-content class="app-content">
        <router-outlet></router-outlet>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    /* Toolbar institucional CAIXA */
    .app-toolbar {
      background-color: #005CA9;
      color: white;
      position: sticky;
      top: 0;
      z-index: 1000;
    }

    .app-title {
      font-size: 20px;
      font-weight: 500;
      margin-left: 8px;
    }

    .app-subtitle {
      font-size: 14px;
      margin-left: 16px;
      opacity: 0.85;
    }

    .spacer {
      flex: 1 1 auto;
    }

    /* Container principal */
    .app-container {
      height: calc(100vh - 64px);
    }

    /* Sidenav */
    .app-sidenav {
      width: 260px;
      background-color: #fafafa;
      border-right: 1px solid #e0e0e0;
    }

    .app-sidenav .active {
      background-color: rgba(0, 92, 169, 0.08);
      color: #005CA9;
      border-right: 3px solid #005CA9;
    }

    .sidenav-footer {
      position: absolute;
      bottom: 16px;
      left: 16px;
      color: #9e9e9e;
    }

    /* Conteúdo */
    .app-content {
      padding: 24px;
      background-color: #f5f5f5;
    }

    /* Responsivo */
    @media (max-width: 768px) {
      .app-subtitle { display: none; }
      .app-sidenav { width: 200px; }
    }
  `]
})
export class AppComponent {
  /** Título da aplicação exibido na toolbar */
  title = 'CAIXA ANS - Sistema de Gestão de Acordos de Nível de Serviço';
}
