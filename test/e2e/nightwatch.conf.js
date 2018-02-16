const minimist = require('minimist');
const { path: seleniumPath } = require('selenium-server');
const { path: chromePath } = require('chromedriver');

const commandLineOptions = minimist(process.argv.slice(2));

require('babel-register');
require('nightwatch-cucumber')({
  cucumberArgs: [
    '--require', './test/e2e/page-models',
    '--require', './test/e2e/steps',
    '--require', './test/e2e/support',
    '--format', 'json:test/e2e/reports/cucumber_report.json',
    './test/e2e/features'],
});

// http://nightwatchjs.org/gettingstarted#settings-file
const config = {
  output_folder: 'test/e2e/reports',
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
  },
};

if (commandLineOptions.headless) {
  config.test_settings.chrome.desiredCapabilities.chromeOptions = {
    args: ['--headless'],
  };
}

module.exports = config;
