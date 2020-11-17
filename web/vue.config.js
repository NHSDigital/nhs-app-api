const env = require('./src/config/env.json');

module.exports = {
  devServer: {
    before: (app) => {
      app.get(/CONFIG_PATH\/config.json$/, (req, res) => res.send(env));
    },
    disableHostCheck: process.env.NODE_ENV !== 'production',
    port: env.PORT,
  },
  // assetsDir: VERSION_TAG, this is used locally but ignored in ADO - not sure why moved to package.json
  productionSourceMap: process.env.NODE_ENV !== 'production',
  chainWebpack: (config) => {
    // GraphQL Loader
    config.module
      .rule('images')
      .use('url-loader')
      .loader('url-loader')
      .end();
  },
};
