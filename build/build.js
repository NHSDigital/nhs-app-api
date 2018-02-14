// The 'no-console' rule is disable as, for a console app, logging to the console is appropriate.
/* eslint-disable no-console */
require('./check-versions')();

process.env.NODE_ENV = 'production';

const ora = require('ora');
const rm = require('rimraf');
const path = require('path');
const chalk = require('chalk');
const webpack = require('webpack');
const config = require('../config');
const webpackConfig = require('./webpack.prod.conf');

const spinner = ora('building for production...');
spinner.start();

rm(path.join(config.build.assetsRoot, config.build.assetsSubDirectory), (rmErr) => {
  if (rmErr) throw rmErr;
  webpack(webpackConfig, (webpackErr, stats) => {
    spinner.stop();
    if (webpackErr) throw webpackErr;
    process.stdout.write(`${stats.toString({
      colors: true,
      modules: false,
      // If you are using ts-loader, setting this to true will make TypeScript errors show up
      // during build.
      children: false,
      chunks: false,
      chunkModules: false,
    })}\n\n`);

    if (stats.hasErrors()) {
      console.log(chalk.red('  Build failed with errors.\n'));
      process.exit(1);
    }

    console.log(chalk.cyan('  Build complete.\n'));
    console.log(chalk.yellow(
      '  Tip: built files are meant to be served over an HTTP server.\n' +
      '  Opening index.html over file:// won\'t work.\n',
    ));
  });
});
