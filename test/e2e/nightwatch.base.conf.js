require('babel-register');
require('nightwatch-cucumber')({
  cucumberArgs: [
    '--require', './test/e2e',
    '--format', 'json:test/e2e/reports/cucumber_report.json',
    './test/e2e/features'],
});

// http://nightwatchjs.org/gettingstarted#settings-file
const config = {
  output_folder: 'test/e2e/reports',
  test_settings: {},
};

module.exports = config;
