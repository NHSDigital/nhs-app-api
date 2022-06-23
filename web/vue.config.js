/* eslint-disable quote-props */
// eslint-disable-next-line import/extensions
const webpack = require('webpack');
const _ = require('lodash');

module.exports = {
  configureWebpack: {
    resolve: {
      // we need to add these in for packages we required that are not part of webpack > 5
      // https://gist.github.com/ef4/d2cf5672a93cf241fd47c020b9b3066a
      fallback: {
        'crypto': require.resolve('crypto-browserify'),
        'querystring': require.resolve('querystring-es3'),
        'buffer': require.resolve('buffer/'),
        'stream': require.resolve('stream-browserify'),
      },
    },
    plugins: [
      // required after going to webpack > 5
      // https://github.com/browserify/commonjs-assert/issues/55
      new webpack.ProvidePlugin({
        process: 'process/browser',
      }),
    ],
  },

  // properties found at https://webpack.js.org/configuration/dev-server/
  devServer: {
    onBeforeSetupMiddleware: (devServer) => {
      devServer.app.get(
        /CONFIG_PATH\/config.json$/,
        (req, res) => res.send(
          _.pickBy(
            process.env,
            (value, key) =>
              key !== 'NGINX_VERSION' &&
              key !== 'NJS_VERSION' &&
              key !== 'PKG_RELEASE' &&
              key !== 'PATH',
          ),
        ),
      );
    },
    allowedHosts: process.env.NODE_ENV !== 'production' ? 'all' : 'auto',
    port: process.env.PORT,
  },

  // assetsDir: VERSION_TAG, this
  // is used locally but ignored in ADO - not sure why moved to package.json
  productionSourceMap: process.env.NODE_ENV !== 'production',
  chainWebpack: (config) => {
    config.module
      .rule('images')
      .test(/\.(png|jpe?g|gif|webp)(\?.*)?$/)
      .type('asset/resource')
      .end();
  },
};
