const { assign } = require('lodash/fp');
const { default: urlExpectations } = require('../support/expectations/url-expectations');

const MODE_CSS = 'css selector';
const MODE_XPATH = 'xpath';

class BasePage {
  constructor({ path, world }) {
    const self = this;
    const { browser, baseUrl } = world;
    self.selectors = {};

    self.path = path;
    self.browser = browser;
    self.baseUrl = baseUrl;

    // Creates a proxy to route all calls to the `expect` property through this `get` function.
    // An attempt is made to find an appropriate selector for the specified property name and
    // call it if it is found.  Otherwise and error is thrown.
    // For example.  If a child page defines a selector called 'header' as 'h1',
    // `page.expect.header` will attempt to resolve using `browser.expect.element('h1').
    self.expect = new Proxy(self.browser.expect, {
      get(target, name) {
        if (name === 'url') {
          return urlExpectations(self);
        }

        const { value, using } = self.getSelector(name);
        return self.browser.expect.element(value, using);
      },
    });

    self.invoke = new Proxy(self.browser, {
      get(target, name) {
        const { value, using } = self.getSelector(name);
        self.changeMode(using);
        return self.browser.click(value);
      }
    })
  }

  changeMode(mode) {
    switch(mode) {
      case MODE_XPATH:
        return this.browser.useXpath();
      default:
        return this.browser.useCss();
    }
  }

  getSelector(name) {
    const selector = this.selectors[name];
    if (selector) {
      const result = assign({ using: MODE_CSS }, selector);
      return result;
    }

    throw new Error(`Cannot find selector: ${name}`);
  }

  goto() {
    const baseUrl = this.baseUrl.replace(/\/$/, '');
    const path = this.path.replace(/^\//, '');
    return this.browser.url(`${baseUrl}/${path}`);
    // return this.browser.url('data:,')
  }

  invoke(selectorName) {
    const selector = this.getSelector(selectorName);
    return this.browser.click(selector);
  }
}

export default BasePage;

