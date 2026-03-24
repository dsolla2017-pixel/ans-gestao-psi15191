// =================================================================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// =================================================================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados
//
// 1. SOLID - Princípio da Responsabilidade Única (SRP):
//    - O AuthGuard tem uma única responsabilidade: proteger rotas, verificando a existência de um
//      token de autenticação. Ele não se envolve com a lógica de login ou validação do token,
//      delegando essas tarefas a serviços específicos.
//
// 2. Clean Architecture - Camada de Interface Adapters:
//    - Este Guard atua como um "Interface Adapter", conectando a camada de UI (framework Angular)
//      com a lógica de aplicação (regras de autenticação), garantindo que a navegação ocorra
//      apenas sob as condições corretas.
//
// 3. Design Patterns - Strategy Pattern (implícito):
//    - O sistema de Guards do Angular permite a implementação de diferentes estratégias de
//      proteção de rotas (CanActivate, CanLoad, etc.). O AuthGuard implementa a estratégia
//      `CanActivate`, que pode ser substituída ou estendida conforme novas regras de negócio.
//
// 4. OWASP Top 10 - A01: Quebra de Controle de Acesso:
//    - Este Guard é uma contramedida fundamental para mitigar falhas de controle de acesso,
//      assegurando que apenas usuários autenticados acessem rotas protegidas. A verificação
//      centralizada do token em um único ponto (o Guard) fortalece a segurança.
//
// 5. Angular Style Guide - Guards:
//    - A implementação segue as melhores práticas do Angular, como o uso do decorator `@Injectable`
//      e a implementação da interface `CanActivate`. A injeção de dependência do `Router` também
//      está alinhada com as diretrizes do framework.
//
// 6. RxJS - Programação Reativa (potencial):
//    - Embora este Guard use uma abordagem síncrona, ele poderia ser estendido para retornar um
//      `Observable<boolean>` ou `Promise<boolean>`, permitindo validações assíncronas, como
//      uma chamada HTTP para verificar a validade do token no backend.
// =================================================================================================

import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

/**
 * @class AuthGuard
 * @description
 * Guardião de rota que implementa a interface `CanActivate` para proteger o acesso a
 * componentes e módulos da aplicação. Sua principal função é verificar a presença de um
 * token de autenticação (JWT) no `localStorage` do navegador, garantindo que apenas
 * usuários autenticados possam prosseguir.
 *
 * @see {@link https://gegodtransformacaodosdados.org | Documentação completa do projeto}
 */
@Injectable({
  // O decorator `@Injectable` marca a classe como um serviço que pode ser gerenciado pelo
  // injetor de dependências do Angular.
  providedIn: 'root'
  // A propriedade `providedIn: 'root'` registra o serviço no provedor raiz da aplicação.
  // Isso o torna um "singleton", garantindo que uma única instância do AuthGuard seja
  // compartilhada por toda a aplicação, otimizando o uso de memória e mantendo um estado consistente.
})
export class AuthGuard implements CanActivate {

  /**
   * @constructor
   * @param {Router} router - O serviço de roteamento do Angular, injetado para permitir o redirecionamento do usuário.
   */
  constructor(private router: Router) {}

  /**
   * @method canActivate
   * @description
   * Método principal do Guard, executado pelo Angular antes de ativar uma rota. Ele determina
   * se a navegação para a rota solicitada é permitida.
   *
   * @returns {boolean} Retorna `true` se o usuário estiver autenticado (token existe) e a navegação
   * for autorizada. Retorna `false` se o token não for encontrado, bloqueando a navegação e
   * redirecionando o usuário para a página de login.
   */
  canActivate(): boolean {
    // Acessa o `localStorage` para buscar o token de autenticação. O uso de `localStorage`
    // é uma estratégia comum para persistir o estado de autenticação entre sessões do navegador.
    const token = localStorage.getItem('auth_token');

    // Realiza a verificação da existência do token. Se o token for uma string não vazia (`truthy`),
    // a condição é satisfeita, indicando que o usuário está autenticado.
    if (token) {
      // Retorna `true` para sinalizar ao Angular que a navegação para a rota protegida é permitida.
      return true;
    }

    // Caso o token não seja encontrado (`null` ou `undefined`), o fluxo de negação é iniciado.
    // O serviço `Router` é utilizado para redirecionar o usuário de forma programática.
    this.router.navigate(['/login']);

    // Retorna `false`, informando ao Angular que a navegação deve ser bloqueada. Isso impede
    // o acesso não autorizado ao componente ou módulo protegido.
    return false;
  }
}
