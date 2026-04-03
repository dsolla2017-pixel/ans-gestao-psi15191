/**
 * Configuração do ambiente de produção
 * Sistema de Gestão de ANS - PSI 15191 CAIXA
 *
 * Aponta para a API em produção (IIS/Azure App Service)
 * Alterar apiUrl conforme o servidor de deploy
 */
export const environment = {
  production: true,
  apiUrl: 'https://api-ans.caixa.gov.br/api',
  appTitle: 'CAIXA ANS',
  version: '1.0.0'
};
