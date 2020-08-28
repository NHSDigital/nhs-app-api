const path = require('path');

const config = {
  mode: 'production',
  module: {
    rules: [
      {
        test: /\.js$/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: ['@babel/preset-env'],
          },
        },
      },
    ],
  },
};

const configurations = [];

['v1'].forEach((version) => {
  configurations.push(Object.assign({}, config, {
    entry: {
      nhsapp: `./src/static/js/${version}/src/nhsapp.js`,
    },
    output: {
      path: path.join(__dirname, `public/js/${version}`),
      filename: '[name].js',
    },
  }));
});

module.exports = configurations;
