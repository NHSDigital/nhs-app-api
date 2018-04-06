const DEFAULT_PROFILE_NAME = 'chrome';
const CONFIG_PATH_BROWSERSTACK = './nightwatch.browserstack.conf';
const CONFIG_PATH_LOCAL = './nightwatch.local.conf';

const browserstack = require('browserstack-local');
const minimist = require('minimist');
const nightwatch = require('nightwatch');
const path = require('path');

const browserStackConfig = require(CONFIG_PATH_BROWSERSTACK);

class Runner {
  constructor(argv) {
    this.arguments = minimist(argv.slice(2));
    if (!this.arguments.env) {
      this.arguments.env = DEFAULT_PROFILE_NAME;
      argv.push('--env', this.arguments.env);
    }

    argv.push('--config', this.configPath);
  }

  get configPath() {
    const relativePath = this.isBrowserstack ? CONFIG_PATH_BROWSERSTACK : CONFIG_PATH_LOCAL;
    return path.join(__dirname, relativePath);
  }

  get isBrowserstack() {
    return !!browserStackConfig.test_settings[this.profileName]
  }

  get profileName() {
    return this.arguments.env;
  }

  initialiseBrowserstack() {
    console.log('Initialising browserstack');
    return new Promise((resolve, reject) => {
      this.browserstackLocal = new browserstack.Local();
      nightwatch.bs_local = this.browserstackLocal;
      nightwatch.bs_local.start({ key: process.env.BROWSERSTACK_ACCESS_KEY }, (error) => {
        if (error) {
          reject(error);
        } else {
          resolve();
        }
      });
    });
  };

  start() {
    const promise = this.isBrowserstack ? this.initialiseBrowserstack() : Promise.resolve();
    return promise
      .then(() => {
        return new Promise((resolve, reject) => {
          try {
            console.log('Running nightwatch');

            nightwatch.cli((argv) => {
              nightwatch
                .CliRunner(argv)
                .setup(null, () => this.stopBrowserstack())
                .runTests(() => this.stopBrowserstack());
              resolve();
            });
          } catch (ex) {
            reject(ex);
          }
        });
      });
  }

  stop(err) {
    let exitCode = 0;
    if (err) {
      console.log('There was an error while starting the test runner:\n\n');
      process.stderr.write(err.stack + '\n');
      exitCode = 2;
    }

    console.log('Stopping');

    this.stopBrowserstack();
    process.exit(exitCode);
  }

  stopBrowserstack() {
    if(this.browserstackLocal) {
      this.browserstackLocal.stop(() => {});
    }
  }
}

const runner = new Runner(process.argv);

process.on('exit', () => runner.stop());

return runner
  .start()
  .catch(err => runner.stop(err));
