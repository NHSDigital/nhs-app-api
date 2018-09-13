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
    API_HOST: process.env.API_HOST || 'http://api.local.bitraft.io:8082',
    API_HOST_SERVER: process.env.API_HOST_SERVER || process.env.API_HOST || 'http://api.local.bitraft.io:8082',
    COOKIE_DOMAIN: process.env.COOKIE_DOMAIN || '.bitraft.io',
    PORT: process.env.PORT || '3000',
    ORGAN_DONATION_URL:
      process.env.ORGAN_DONATION_URL || 'https://www.organdonation.nhs.uk',
    DATA_SHARING_URL:
      process.env.DATA_SHARING_URL || 'https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/',
    SYMPTOM_CHECKER_URL:
      process.env.SYMPTOM_CHECKER_URL ||
      'https://111.nhs.uk',
    SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS:
      process.env.SYMPTOM_CHECKER_NATIVE_QUERY_PARAMS || '?referrer=nhsapp&utm_source=%20&utm_medium=NHS%20App&utm_campaign=%20',
    CONDITIONS_CHECKER_URL:
      process.env.CONDITIONS_CHECKER_URL ||
      'https://www.nhs.uk/conditions/',
    TERMS_AND_CONDITIONS_URL:
      process.env.TERMS_AND_CONDITIONS_URL ||
      'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/',
    PRIVACY_POLICY_URL:
      process.env.PRIVACY_POLICY_URL ||
      'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/',
    COOKIES_POLICY_URL:
      process.env.COOKIES_POLICY_URL ||
      'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/',
    OPEN_SOURCE_LICENSES_URL:
      process.env.OPEN_SOURCE_LICENSES_URL ||
      'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/open-source-licences/',
    HELP_AND_SUPPORT_URL:
      process.env.HELP_AND_SUPPORT_URL ||
      'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/',
    CLINICAL_ABBREVIATIONS_URL:
      process.env.CLINICAL_ABBREVIATIONS_URL ||
      'https://www.england.nhs.uk/participation/resources/involvejargon/',
    CID_CLIENT_ID: process.env.CID_CLIENT_ID || 'nhs-online',
    YOUR_NHS_DATA_MATTERS_URL:
      process.env.YOUR_NHS_DATA_MATTERS_URL ||
      'https://www.nhs.uk/your-nhs-data-matters/',
    DATA_PREFERENCES_URL:
      process.env.DATA_PREFERENCES_URL ||
      'https://ndopapp-int1.thunderbird.service.nhs.uk/createsession',
    CID_REDIRECT_URI:
      process.env.CID_REDIRECT_URI || 'http://web.local.bitraft.io:3000/auth-return',
    NATIVE_CID_REDIRECT_URI:
      process.env.NATIVE_CID_REDIRECT_URI || 'nhsapp://android.local.bitraft.io:3000/auth-return',
    CID_AUTH_ENDPOINT:
      process.env.CID_AUTH_ENDPOINT
      || 'https://auth.ext.signin.nhs.uk/authorize',
    CID_REGISTER_ENDPOINT:
      process.env.CID_REGISTER_ENDPOINT
      || 'https://account.uat.signin.nhs.uk/#/account/register',
    ANALYTICS_SCRIPT_URL:
      process.env.ANALYTICS_SCRIPT_URL || '//assets.adobedtm.com/launch-EN2bcb86c8edd64d5aa2abd8aabdcfb129-development.min.js',
    ANALYTICS_ENVIRONMENT:
      process.env.ANALYTICS_ENVIRONMENT || 'development',
    HOTJAR_FILENAME: process.env.HOTJAR_FILENAME,
    HOT_JAR_URL: process.env.HOT_JAR_URL || 'https://in.hotjar.com/s?siteId=859152&surveyId=95785',
  },
};
