// eslint-disable-next-line import/no-extraneous-dependencies
const webpack = require('webpack');

const APP_VERSION_TAG = 'dev_web_npm';

const config = {
  build: {
    plugins: [
      new webpack.optimize.LimitChunkCountPlugin({
        maxChunks: 4,
      }),
      new webpack.NormalModuleReplacementPlugin(
        /moment-timezone\/data\/packed\/latest\.json/,
        require.resolve('./src/lib/timezones.json'),
      ),
    ],
    publicPath: `/_nuxt/${APP_VERSION_TAG}`.replace(/\./g, '-'),
    babel: {
      babelrc: true,
      configFile: true,
    },
  },
  head: {
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1, maximum-scale=1.0, minimum-scale=1.0, user-scalable=0' },
    ],
  },
  srcDir: 'src',
  modules: [
    // Simple usage
    'cookie-universal-nuxt',
  ],
  css: [
    '~/style/_nhsukfrontend.scss',
  ],
  plugins: [
    '~/plugins/routing.js',
    '~/plugins/environmentVariables.js',
    '~/plugins/i18n.js',
    '~/plugins/api.js',
    '~/plugins/filters.js',
    '~/plugins/mixins.js',
    '~/plugins/analytics.js',
    '~/plugins/logging.js',
    '~/plugins/directives.js',
  ],
  serverMiddleware: [
    './.transpiled/middleware-server/noJsApi.js',
    './.transpiled/middleware-server/setRequestId.js',
    './.transpiled/middleware-server/handler.js',
    './.transpiled/middleware-server/responseHeaders.js',
  ],
  router: {
    middleware: [
      'setSource',
      'noJsState',
      'urlResolution',
      'auth',
      'serviceJourneyRules',
      'session',
      'conditionalRedirect',
      'sjrRedirect',
      'meta',
      'proxyRecovery',
      'termsAndConditions',
      'myRecordAcceptance',
      'urlResolution',
      'onlineConsultations',
      'knownServices',
    ],
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
    URI_FORMAT_API_CLIENT: 'http://api{host}:8089',
    API_HOST_SERVER: 'http://api.local.bitraft.io:8089',
    URI_FORMAT_CID_REDIRECT_WEB: 'http://web{host}:3000/auth-return',
    URI_FORMAT_CID_REDIRECT_NATIVE: 'nhsapp://web{host}:3000/auth-return',
    // Cookie settings
    SECURE_COOKIES: false,
    // Biometrics
    BIOMETRICS_ENABLED: false,
    // Core
    COOKIE_DOMAIN: '.bitraft.io',
    // Citizen ID
    CID_CLIENT_ID: 'nhs-online',
    CID_AUTH_ENDPOINT: 'https://auth.ext.signin.nhs.uk/authorize',
    // Organ Donation
    ORGAN_DONATION_FIND_OUT_MORE_URL: 'https://www.organdonation.nhs.uk/app/app-donation/',
    ORGAN_DONATION_URL: 'https://www.organdonation.nhs.uk/',
    BLOOD_DONATION_URL: 'https://www.blood.co.uk/',
    ORGAN_DONATION_ALREADY_REGISTERED_URL: 'https://www.organdonation.nhs.uk/app/app-check/',
    ORGAN_DONATION_SHARE_DECISION_URL: 'https://www.organdonation.nhs.uk/app/app-share/',
    ORGAN_DONATION_TELL_FAMILY_URL: 'https://www.organdonation.nhs.uk/app/app-tell/',
    ORGAN_DONATION_PRIVACY_URL: 'https://www.organdonation.nhs.uk/app/app-privacy/',
    // Data Opt-Out
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
    TERMS_AND_CONDITIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms/',
    PRIVACY_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/',
    COOKIES_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies/',
    COOKIES_BANNER_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy#manage',
    COOKIES_BANNER_EXPIRY_DAYS: 365,
    // Contact us
    CONTACT_US_URL: 'https://www.nhs.uk/contact-us/nhs-app-contact-us',
    OPEN_SOURCE_LICENCES_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source/',
    HELP_AND_SUPPORT_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',
    ACCESSIBILITY_STATEMENT: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/accessibility/',
    YOUR_NHS_DATA_MATTERS_URL: 'https://www.nhs.uk/your-nhs-data-matters/',
    // Appointments
    CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/',
    ONLINE_CONSULTATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/',
    CORONA_SERVICE_URL: 'https://111.nhs.uk/service/COVID-19/',
    // App Version
    VERSION_TAG: 'dev_web_npm',
    COMMIT_ID: 'dev',
    CE_MARK_ENABLED: false,
    ORGAN_DONATION_INTEGRATION_ENABLED: true,
    // Session
    SESSION_EXPIRING_WARNING_SECONDS: 60,
    // Click debounce delay in ms
    DEBOUNCE_SHORT: 500,
    DEBOUNCE_MEDIUM: 2000,
    DEBOUNCE_LONG: 5000,
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
