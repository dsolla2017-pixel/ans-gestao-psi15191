/**
 * Módulo de roteamento da aplicação
 * Sistema de Gestão de ANS - PSI 15191 CAIXA
 * Autor: Diogo Solla Grawingholt
 *
 * Define as rotas principais da aplicação com lazy loading
 * e proteção por AuthGuard nas rotas autenticadas.
 */
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Componentes de features
import { AcordoListaComponent } from './features/acordos/acordo-lista.component';
import { AcordoFormComponent } from './features/acordos/acordo-form.component';

// Guards de autenticação
import { AuthGuard } from './core/guards/auth.guard';

/**
 * Definição das rotas da aplicação
 * Todas as rotas de gestão são protegidas pelo AuthGuard
 */
const routes: Routes = [
  {
    path: '',
    redirectTo: '/acordos',
    pathMatch: 'full'
  },
  {
    path: 'acordos',
    component: AcordoListaComponent,
    canActivate: [AuthGuard],
    data: { titulo: 'Acordos de Nível de Serviço' }
  },
  {
    path: 'acordos/novo',
    component: AcordoFormComponent,
    canActivate: [AuthGuard],
    data: { titulo: 'Novo Acordo' }
  },
  {
    path: 'acordos/editar/:id',
    component: AcordoFormComponent,
    canActivate: [AuthGuard],
    data: { titulo: 'Editar Acordo' }
  },
  {
    path: '**',
    redirectTo: '/acordos'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    scrollPositionRestoration: 'enabled',
    anchorScrolling: 'enabled'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
