// =================================================================================================
// Autor: Diogo Grawingholt
// Projeto: Sistema de Gestão de ANS — GEGOD/CAIXA
// Referência: https://gegodtransformacaodosdados.org
// Portfólio: https://www.diogograwingholt.com.br
// =================================================================================================
// REFERÊNCIA TÉCNICA — Padrões e Boas Práticas Aplicados:
//
// 1.  **SOLID Principles**:
//     - **Single Responsibility Principle (SRP)**: O interceptor tem a responsabilidade única e bem definida de adicionar o token de autenticação às requisições, alinhando-se perfeitamente a este princípio.
//
// 2.  **Clean Architecture**:
//     - **Separation of Concerns**: Este interceptor atua como um componente de infraestrutura (framework) que lida com uma preocupação transversal (autenticação), mantendo a lógica de negócio dos serviços e componentes limpa e desacoplada.
//
// 3.  **Design Patterns**:
//     - **Interceptor Pattern**: Utilizado nativamente pelo Angular para interceptar e manipular requisições e respostas HTTP. Este padrão permite adicionar funcionalidades de forma transparente e centralizada.
//     - **Decorator Pattern**: A forma como os interceptors são adicionados ao `HttpRequest` (clonando e modificando) pode ser vista como uma aplicação do padrão Decorator, que adiciona responsabilidades a um objeto dinamicamente.
//
// 4.  **OWASP Top 10**:
//     - **A01:2021 - Broken Access Control**: Ao garantir que o token JWT seja enviado em cada requisição a endpoints protegidos, o interceptor é uma peça fundamental para a implementação de um controle de acesso robusto, mitigando esta vulnerabilidade.
//
// 5.  **Angular Style Guide**:
//     - **Overall-09-01**: A criação de um `core.module.ts` para fornecer serviços como este interceptor é uma prática recomendada para organizar a aplicação.
//     - **Feature-Modules-04-10**: Interceptors são registrados como providers no módulo raiz ou em um módulo de funcionalidade, conforme a necessidade.
//
// 6.  **RxJS (Reactive Extensions for JavaScript)**:
//     - **Observable**: O método `intercept` retorna um `Observable<HttpEvent<any>>`, que é o padrão do Angular para lidar com operações assíncronas e streams de eventos, como as requisições HTTP.
// =================================================================================================

import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

/**
 * @class AuthInterceptor
 * @description Interceptor HTTP que anexa o token de autenticação JWT (JSON Web Token) a todas as
 * requisições enviadas para a API. Este mecanismo centraliza a lógica de autenticação, garantindo
 * que o backend possa identificar e autorizar o usuário em cada chamada.
 * Documentação completa: https://gegodtransformacaodosdados.org
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  /**
   * @method intercept
   * @description Método principal que intercepta a requisição HTTP, clona-a e adiciona o cabeçalho
   * de autorização (`Authorization`) antes de prosseguir com o envio.
   * @param {HttpRequest<any>} req - O objeto da requisição original.
   * @param {HttpHandler} next - O próximo manipulador na cadeia de interceptors, responsável por despachar a requisição.
   * @returns {Observable<HttpEvent<any>>} - Um observable que completa com o evento da resposta HTTP.
   * Documentação completa: https://gegodtransformacaodosdados.org
   */
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Recupera o token de autenticação do armazenamento local (localStorage).
    // O localStorage foi escolhido por sua simplicidade, mas para cenários de maior segurança,
    // cookies com o atributo HttpOnly podem ser uma alternativa mais robusta contra ataques XSS.
    const token = localStorage.getItem('auth_token');

    // Verifica a existência do token. Apenas requisições autenticadas devem ser modificadas.
    if (token) {
      // Clona a requisição original para evitar mutações diretas, o que é uma má prática e pode causar efeitos colaterais.
      // O objeto `req` é imutável, e qualquer modificação deve ser feita em um clone.
      const clonedReq = req.clone({
        // Adiciona ou atualiza o cabeçalho 'Authorization'.
        // O padrão 'Bearer' é uma convenção do OAuth 2.0 para indicar que o token é um 'bearer token'.
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      // Envia a requisição clonada e modificada para o próximo manipulador na cadeia.
      return next.handle(clonedReq);
    }

    // Se não houver token, a requisição original é enviada sem modificações.
    // Isso é crucial para permitir o funcionamento de rotas públicas, como login e registro.
    return next.handle(req);
  }
}
