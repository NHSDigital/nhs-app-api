// eslint-disable-next-line import/no-extraneous-dependencies
const webpack = require('webpack');

const config = {
  build: {
    plugins: [
      new webpack.optimize.LimitChunkCountPlugin({
        maxChunks: 4,
      }),
    ],
  },
  head: {
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width,initial-scale=1,maximum-scale=1,user-scalable=0' },
    ],
  },
  srcDir: 'src',
  modules: [
    // Simple usage
    'cookie-universal-nuxt',
  ],
  plugins: [
    '~/plugins/environmentVariables.js',
    '~/plugins/i18n.js',
    '~/plugins/api.js',
    '~/plugins/filters.js',
    '~/plugins/mixins.js',
    '~/plugins/analytics.js',
    '~/plugins/logging.js',
  ],
  serverMiddleware: [
    './.transpiled/middleware-server/noJsApi.js',
    './.transpiled/middleware-server/handler.js',
    './.transpiled/middleware-server/responseHeaders.js',
  ],
  router: {
    middleware: ['urlResolution', 'throttling', 'auth', 'meta', 'analytics', 'termsAndConditions'],
  },
  link: [
    {
      rel: 'icon',
      type: 'image/x-icon',
      href: 'favicon.ico',
    },
  ],
  env: {
    // URL Formats
    URI_FORMAT_API_CLIENT: 'http://api{host}:8082',
    API_HOST_SERVER: 'http://api.local.bitraft.io:8082',
    URI_FORMAT_CID_REDIRECT_WEB: 'http://web{host}:3000/auth-return',
    URI_FORMAT_CID_REDIRECT_NATIVE: 'nhsapp://web{host}:3000/auth-return',
    // Biometrics
    BIOMETRICS_ENABLED: false,
    // Core
    COOKIE_DOMAIN: '.bitraft.io',
    // Citizen ID
    CID_CLIENT_ID: 'nhs-online',
    CID_AUTH_ENDPOINT: 'https://auth.ext.signin.nhs.uk/authorize',
    // Organ Donation
    ORGAN_DONATION_URL: 'https://www.organdonation.nhs.uk/',
    // Data Opt-Out
    DATA_SHARING_URL: 'https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/',
    DATA_PREFERENCES_URL: 'https://ndopapp-int1.thunderbird.service.nhs.uk/createsession',
    // Symptom Checker
    SYMPTOM_CHECKER_URL: 'https://111.nhs.uk',
    SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS: '?referrer=nhsapp&utm_source=%20&utm_medium=NHS%20App&utm_campaign=%20',
    CONDITIONS_CHECKER_URL: 'https://www.nhs.uk/conditions/',
    // Analytics
    ANALYTICS_SCRIPT_URL: '//assets.adobedtm.com/launch-EN2bcb86c8edd64d5aa2abd8aabdcfb129-development.min.js',
    ANALYTICS_ENVIRONMENT: 'development',
    // Feedback
    HOTJAR_SITE_ID: '859152',
    HOTJAR_SURVEY_URL: 'https://in.hotjar.com/s?siteId=859152&surveyId=95785',
    HOTJAR_SURVEY_VISIBLE: false,
    // Legal
    TERMS_AND_CONDITIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/',
    PRIVACY_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/',
    COOKIES_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/',
    OPEN_SOURCE_LICENCES_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/',
    HELP_AND_SUPPORT_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/',
    ACCESSIBILITY_STATEMENT: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/accessibility-statement/',
    YOUR_NHS_DATA_MATTERS_URL: 'https://www.nhs.uk/your-nhs-data-matters/',
    // Appointments
    CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/medical-abbreviations/',
    // Terms and conditions
    STUB_TERMS_AND_CONDITIONS: true,
    // App Version
    VERSION_TAG: 'dev_web_npm',
    // Throttling
    GP_LOOKUP_API_KEY: '',
    GP_LOOKUP_API_URL: 'https://nhsapiint.azure-api.net/service-search/search?api-version=1',
    GP_LOOKUP_API_RESULTS_LIMIT: 20,
    // Feature Toggles
    THROTTLING_ENABLED: false,
    COMMIT_ID: 'dev',
  },
};

if (process.env.NODE_ENV !== 'production') {
  config.vue = {
    config: {
      devtools: true,
    },
  };
}

config.head.meta.push({ name: 'commit_id', content: config.env.COMMIT_ID });
module.exports = config;
