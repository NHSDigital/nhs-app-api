module.exports = {
  NODE_ENV: process.env.NODE_ENV || 'production',
  API_HOST: process.env.API_HOST || 'http://localhost:8080',
  ORGAN_DONATION_URL: process.env.ORGAN_DONATION_URL || 'https://www.organdonation.nhs.uk',
  PORT: process.env.PORT || 4000,
  SYMPTOM_CHECKER_URL:
    process.env.SYMPTOM_CHECKER_URL || 'https://111-int2.staging.111.service.nhs.uk​',
  CID_CLIENT_ID: process.env.CID_CLIENT_ID || 'nhs-online-poc',
  CID_REDIRECT_URI: process.env.CID_REDIRECT_URI || 'http://localhost:3000/auth-return',
  CID_AUTH_ENDPOINT: process.env.CID_AUTH_ENDPOINT
    || 'https://keycloak.dev1.signin.nhs.uk/cicauth/realms/NHS/protocol/openid-connect/auth',
};
