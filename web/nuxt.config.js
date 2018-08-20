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
    '~/plugins/analytics.js',
  ],
  serverMiddleware: [
    './handler.js',
  ],
  router: {
    middleware: ['auth', 'meta', 'analytics'],
  },
  link: [
    {
      rel: 'icon',
      type: 'image/x-icon',
      href: 'favicon.ico',
    },
  ],
  env: {
    NODE_ENV: process.env.NODE_ENV || 'development',
    API_HOST: process.env.API_HOST || 'http://localhost:8082',
    API_HOST_SERVER: process.env.API_HOST_SERVER || process.env.API_HOST || 'http://localhost:8082',
    PORT: process.env.PORT || '3000',
    ORGAN_DONATION_URL:
      process.env.ORGAN_DONATION_URL || 'https://www.organdonation.nhs.uk',
    DATA_SHARING_URL:
      process.env.DATA_SHARING_URL || 'https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/',
    SYMPTOM_CHECKER_URL:
      process.env.SYMPTOM_CHECKER_URL ||
      'https://111.nhs.uk',
    CONDITIONS_CHECKER_URL:
      process.env.CONDITIONS_CHECKER_URL ||
      'https://www.nhs.uk/conditions/',
    TERMS_AND_CONDITIONS_URL:
      process.env.TERMS_AND_CONDITIONS_URL ||
      'http://beta.nhs.uk/',
    PRIVACY_POLICY_URL:
      process.env.PRIVACY_POLICY_URL ||
      'http://beta.nhs.uk/',
    COOKIES_POLICY_URL:
      process.env.COOKIES_POLICY_URL ||
      'http://beta.nhs.uk/',
    OPEN_SOURCE_LICENSES_URL:
      process.env.OPEN_SOURCE_LICENSES_URL ||
      'http://beta.nhs.uk/',
    HELP_AND_SUPPORT_URL:
      process.env.HELP_AND_SUPPORT_URL ||
      'http://beta.nhs.uk/',
    CLINICAL_ABBREVIATIONS_URL:
      process.env.CLINICAL_ABBREVIATIONS_URL ||
      'https://beta.nhs.uk',
    CID_CLIENT_ID: process.env.CID_CLIENT_ID || 'nhs-online-poc',
    CID_REDIRECT_URI:
      process.env.CID_REDIRECT_URI || 'http://localhost:3000/auth-return',
    ANDROID_CID_REDIRECT_URI:
      process.env.ANDROID_CID_REDIRECT_URI || 'nhsapp://10.0.2.2:3000/auth-return',
    CID_AUTH_ENDPOINT:
      process.env.CID_AUTH_ENDPOINT
      || 'https://keycloak.dev1.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/auth',
    CID_REGISTER_ENDPOINT:
      process.env.CID_REGISTER_ENDPOINT
      || 'https://keycloak.dev1.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/registrations',
    ANALYTICS_SCRIPT_URL:
      process.env.ANALYTICS_SCRIPT_URL || '//assets.adobedtm.com/launch-EN2bcb86c8edd64d5aa2abd8aabdcfb129-development.min.js',
    ANALYTICS_ENVIRONMENT:
      process.env.ANALYTICS_ENVIRONMENT || 'development',
  },
};
