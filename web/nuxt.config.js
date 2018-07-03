module.exports = {
  head: {
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    ],
  },
  srcDir: 'src',
  modules: [
    // Simple usage
    'cookie-universal-nuxt',
  ],
  plugins: [
    { src: '~/plugins/persistedState.js' },
    '~/plugins/i18n.js',
    '~/plugins/api.js',
    '~/plugins/filters.js',
    '~/plugins/mixins.js',
  ],
  serverMiddleware: [
    './handler.js',
  ],
  router: {
    middleware: ['analytics'],
  },
  env: {
    NODE_ENV: process.env.NODE_ENV || 'development',
    API_HOST: process.env.API_HOST || 'http://localhost:8082',
    ORGAN_DONATION_URL:
      process.env.ORGAN_DONATION_URL || 'https://www.organdonation.nhs.uk',
    PORT: process.env.PORT || '3000',
    SYMPTOM_CHECKER_URL:
      process.env.SYMPTOM_CHECKER_URL ||
      'https://111-int2.staging.111.service.nhs.uk​',
    CID_CLIENT_ID: process.env.CID_CLIENT_ID || 'nhs-online-poc',
    CID_REDIRECT_URI:
      process.env.CID_REDIRECT_URI || 'http://localhost:3000/auth-return',
    CID_AUTH_ENDPOINT:
      process.env.CID_AUTH_ENDPOINT ||
      'https://keycloak.dev1.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/auth',
    CID_REGISTER_ENDPOINT:
      process.env.CID_REGISTER_ENDPOINT ||
      'https://keycloak.dev1.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/registrations',
  },
};
