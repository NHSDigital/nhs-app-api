const fromEnv = require('./from-env');

module.exports = {
  NODE_ENV: '"production"',
  CID_AUTH_ENDPOINT: fromEnv('CID_AUTH_ENDPOINT'),
  CID_CLIENT_ID: fromEnv('CID_CLIENT_ID', 'nhs-online-poc'),
  CID_REDIRECT_URI: fromEnv('CID_REDIRECT_URI'),
};
