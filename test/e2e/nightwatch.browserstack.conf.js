const { assign } = require('lodash/fp');
const config = require('./nightwatch.base.conf');

require('browserstack-automate').Nightwatch();

module.exports = assign(config, {
  selenium: {
    start_process: false,
    host: 'hub-cloud.browserstack.com',
    port: 80,
  },
  test_settings: {
    default: {
      selenium_port: 80,
      selenium_host: 'hub-cloud.browserstack.com',
    },
    bswin8chrome60: {
      desiredCapabilities: {
        'browserstack.user': 'leegathercole1',
        'browserstack.key': process.env.BROWSERSTACK_ACCESS_KEY,
        os: 'Windows',
        os_version: '8',
        browser: 'Chrome',
        browser_version: '60.0',
        resolution: '1024x768',
        'browserstack.local': true,
        'browserstack.debug': true,
      },
    },
  },
});
