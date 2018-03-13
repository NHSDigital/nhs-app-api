const minimist = require('minimist');
const { assign } = require('lodash/fp');
const { path: seleniumPath } = require('selenium-server');
const { path: chromePath } = require('chromedriver');
const baseConfig = require('./nightwatch.base.conf');
const commandLineOptions = minimist(process.argv.slice(2));

const config = assign(baseConfig, {
    selenium: {
    start_process: true,
    server_path: seleniumPath,
    host: '127.0.0.1',
    port: 4444,
    cli_args: {
      'webdriver.chrome.driver': chromePath,
    },
  },
  test_settings: {
    default: {
      selenium_port: 4444,
      selenium_host: 'localhost',
      silent: true,
    },
    chrome: {
      desiredCapabilities: {
        browserName: 'chrome',
        javascriptEnabled: true,
        acceptSslCerts: true,
      },
    },

    firefox: {
      desiredCapabilities: {
        browserName: 'firefox',
        javascriptEnabled: true,
        acceptSslCerts: true,
      },
    },
  }
});

if (commandLineOptions.headless) {
    config.test_settings.chrome.desiredCapabilities.chromeOptions = {
        args: ['--headless'],
    };
}

module.exports = config;
