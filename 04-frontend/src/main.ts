/**
 * Ponto de entrada da aplicação Angular
 * Sistema de Gestão de ANS - PSI 15191 CAIXA
 * Autor: Diogo Solla Grawingholt
 */
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';

// Inicializa a aplicação Angular com o módulo raiz
platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch((err: Error) => console.error('Erro ao inicializar a aplicação:', err));
