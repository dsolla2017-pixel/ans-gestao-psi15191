// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Guard: Porteiro que verifica se o usuario tem permissao para acessar uma tela.
// - CanActivate: Metodo que retorna true (pode acessar) ou false (bloqueado).
// - Router: Servico que controla a navegacao entre telas.
// ============================================================

import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

/**
 * Guard de autenticacao que verifica se o usuario esta logado.
 * Em producao, valida o token JWT armazenado no localStorage.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(): boolean {
    const token = localStorage.getItem('auth_token');
    if (token) {
      return true;
    }
    // Redirecionar para a tela de login
    this.router.navigate(['/login']);
    return false;
  }
}
