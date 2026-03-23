// ============================================================
// Autor: Solla, Diogo
// Projeto: Sistema de Gestao de ANS - CAIXA
// ============================================================
// GLOSSARIO PARA LEIGO:
// - Service (Angular): Classe que centraliza a comunicacao com a API.
// - HttpClient: Ferramenta do Angular para fazer requisicoes HTTP.
// - Observable: Fluxo de dados assincrono (similar a uma Promise).
// - Injectable: Marca a classe para ser injetada automaticamente.
// ============================================================

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AcordoResumo, AcordoDetalhe, CriarAcordo } from '../../shared/models/acordo.model';

/**
 * Servico responsavel pela comunicacao com a API de Acordos.
 * Centraliza todas as chamadas HTTP em um unico ponto.
 */
@Injectable({
  providedIn: 'root'
})
export class AcordoService {
  private readonly apiUrl = '/api/v1/acordos';

  constructor(private http: HttpClient) {}

  /**
   * Lista todos os ANS com filtros opcionais.
   */
  listar(filtro?: string, situacao?: string, ordenarPor?: string): Observable<AcordoResumo[]> {
    let params = new HttpParams();
    if (filtro) params = params.set('filtro', filtro);
    if (situacao) params = params.set('situacao', situacao);
    if (ordenarPor) params = params.set('ordenarPor', ordenarPor);

    return this.http.get<AcordoResumo[]>(this.apiUrl, { params });
  }

  /**
   * Obtem os detalhes completos de um ANS.
   */
  obterDetalhes(id: number): Observable<AcordoDetalhe> {
    return this.http.get<AcordoDetalhe>(`${this.apiUrl}/${id}`);
  }

  /**
   * Cria um novo ANS (situacao Pendente).
   */
  criar(acordo: CriarAcordo): Observable<AcordoResumo> {
    return this.http.post<AcordoResumo>(this.apiUrl, acordo);
  }

  /**
   * Registra a assinatura de uma das partes.
   */
  assinar(id: number, papel: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/assinar`, null, {
      params: new HttpParams().set('papel', papel)
    });
  }

  /**
   * Solicita a inativacao precoce (dupla-custodia).
   */
  solicitarInativacao(id: number, justificativa: string, matriculaGerente: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/solicitar-inativacao`, {
      deJustificativa: justificativa,
      nuMatriculaGerenteAprovador: matriculaGerente
    });
  }

  /**
   * Gerente avalia (aprova/recusa) a inativacao.
   */
  avaliarInativacao(id: number, aprovado: boolean): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/avaliar-inativacao`, { aprovado });
  }

  /**
   * Exclui logicamente um ANS.
   */
  excluir(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
