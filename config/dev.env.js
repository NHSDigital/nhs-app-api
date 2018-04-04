const merge = require('webpack-merge');
const fromEnv = require('./from-env');
const prodEnv = require('./prod.env');

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  CID_REDIRECT_URI: fromEnv('CID_REDIRECT_URI'),
});
