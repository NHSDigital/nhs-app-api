module.exports = {
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
  ],
  serverMiddleware: [
    './handler.js',
  ],
  router: {
    middleware: ['auth', 'meta', 'analytics', 'termsAndConditions'],
  },
  link: [
    {
      rel: 'icon',
      type: 'image/x-icon',
      href: 'favicon.ico',
    },
  ],
  env: {
    // Core
    API_HOST: 'http://api.local.bitraft.io:8082',
    API_HOST_SERVER: 'http://api.local.bitraft.io:8082',
    COOKIE_DOMAIN: '.bitraft.io',
    // Citizen ID
    CID_CLIENT_ID: 'nhs-online',
    CID_AUTH_ENDPOINT: 'https://auth.ext.signin.nhs.uk/authorize',
    CID_REDIRECT_URI: 'http://web.local.bitraft.io:3000/auth-return',
    NATIVE_CID_REDIRECT_URI: 'nhsapp://android.local.bitraft.io:3000/auth-return',
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
    HOTJAR_FILENAME: '',
    HOT_JAR_URL: 'https://in.hotjar.com/s?siteId=859152&surveyId=95785',
    // Legal
    TERMS_AND_CONDITIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/',
    PRIVACY_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/',
    COOKIES_POLICY_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/',
    OPEN_SOURCE_LICENSES_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/',
    HELP_AND_SUPPORT_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/',
    YOUR_NHS_DATA_MATTERS_URL: 'https://www.nhs.uk/your-nhs-data-matters/',
    // Appointments
    CLINICAL_ABBREVIATIONS_URL: 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/medical-abbreviations/',
    // Terms and conditions
    STUB_TERMS_AND_CONDITIONS: true,
    SHOW_SURVEY: false,
  },
};
