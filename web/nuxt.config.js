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
      'upliftRedirect',
      'conditionalRedirect',
      'sjrRedirect',
      'meta',
      'proxyRecovery',
      'termsAndConditions',
      'myRecordAcceptance',
      'viewedPreRegInstructions',
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
    // About us
    ACCESSIBILITY_STATEMENT_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/accessibility/',
    OPEN_SOURCE_LICENCES_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source/',

    // Additional Services
    CORONA_SERVICE_URL: 'https://111.nhs.uk/service/COVID-19/',

    // Analytics
    ANALYTICS_ENVIRONMENT: 'development',
    ANALYTICS_SCRIPT_URL: '//assets.adobedtm.com/launch-EN2bcb86c8edd64d5aa2abd8aabdcfb129-development.min.js',

    // Citizen ID
    CID_AUTH_ENDPOINT_URL: 'https://auth.ext.signin.nhs.uk/authorize',
    CID_CLIENT_ID: 'nhs-online',
    CID_P5_VECTOR_OF_TRUST_ENABLED: false,

    // Cookie settings
    SECURE_COOKIES: false,

    // Core
    COOKIE_DOMAIN: '.bitraft.io',
    COMMIT_ID: 'dev',

    // Data urls
    DATA_PREFERENCES_URL: 'https://ndopapp-int1.thunderbird.service.nhs.uk/createsession',
    OTHER_WAYS_TO_MAKE_A_CHOICE_URL: 'https://www.nhs.uk/your-nhs-data-matters/manage-your-choice/other-ways-to-manage-your-choice/',
    YOUR_NHS_DATA_MATTERS_URL: 'https://www.nhs.uk/your-nhs-data-matters/',

    // Feature toggles
    USER_RESEARCH_ENABLED: false,

    // Feedback
    HOTJAR_SITE_ID: '859152',
    HOTJAR_SURVEY_URL: 'https://in.hotjar.com/s?siteId=859152&surveyId=95785',
    HOTJAR_SURVEY_VISIBLE: false,

    // Help urls
    CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/',
    CONTACT_US_URL: 'https://www.nhs.uk/contact-us/nhs-app-contact-us',
    HELP_AND_SUPPORT_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help/',

    // Legal
    CE_MARK_ENABLED: false,
    COOKIES_BANNER_EXPIRY_DAYS: 365,
    COOKIES_BANNER_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy#manage',
    COOKIES_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies/',
    ONLINE_CONSULTATIONS_PRIVACY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/online-consultations/',
    PRIVACY_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/',
    TERMS_AND_CONDITIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms/',

    // Modals
    SESSION_EXPIRING_WARNING_SECONDS: 60,

    // Native Application
    VERSION_TAG: 'dev_web_npm',

    // Nominated Pharmacy
    NOMINATED_PHARMACY_DSP_URL: 'https://www.nhs.uk/Service-Search/other-services/pharmacies/internetpharmacies',

    // Organ Donation
    BLOOD_DONATION_URL: 'https://www.blood.co.uk/',
    ORGAN_DONATION_ALREADY_REGISTERED_URL: 'https://www.organdonation.nhs.uk/app/app-check/',
    ORGAN_DONATION_FIND_OUT_MORE_URL: 'https://www.organdonation.nhs.uk/app/app-donation/',
    ORGAN_DONATION_LAW_CHANGE_URL: 'https://www.organdonation.nhs.uk/app/app-law/',
    ORGAN_DONATION_PRIVACY_URL: 'https://www.organdonation.nhs.uk/app/app-privacy/',
    ORGAN_DONATION_SHARE_DECISION_URL: 'https://www.organdonation.nhs.uk/app/app-share/',
    ORGAN_DONATION_TELL_FAMILY_URL: 'https://www.organdonation.nhs.uk/app/app-tell/',
    ORGAN_DONATION_URL: 'https://www.organdonation.nhs.uk/',

    // Symptom Checker
    CONDITIONS_CHECKER_URL: 'https://www.nhs.uk/conditions/',
    SYMPTOM_CHECKER_URL: 'https://111.nhs.uk',
    SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS: '?referrer=nhsapp&utm_source=%20&utm_medium=NHS%20App&utm_campaign=%20',

    // URL Formats
    API_BASE_URL: 'http://api.local.bitraft.io:8089',
    URI_FORMAT_API_CLIENT: 'http://api{host}:8089',
    URI_FORMAT_CID_REDIRECT_NATIVE: 'nhsapp://web{host}:3000/auth-return',
    URI_FORMAT_CID_REDIRECT_WEB: 'http://web{host}:3000/auth-return',
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
