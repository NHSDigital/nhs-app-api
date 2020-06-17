const env = require('./src/config/env.json');

// TODO: productionSourceMap do we want this true in production???
let VERSION_TAG = process.env.APP_VERSION_TAG;
if (VERSION_TAG) {
  VERSION_TAG = process.env.APP_VERSION_TAG.replace(/\./g, '-');
}
module.exports = {
  devServer: {
    before: (app) => {
      app.get('/config.json', (req, res) => res.send(env));
    },
    disableHostCheck: process.env.NODE_ENV !== 'production',
    port: env.PORT,
  },
  assetsDir: VERSION_TAG,
  productionSourceMap: true,
  chainWebpack: (config) => {
    // GraphQL Loader
    config.module
      .rule('images')
      .use('url-loader')
      .loader('url-loader')
      .end();
  },
};
