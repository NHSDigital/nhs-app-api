import { resolve as resolveUrl } from 'url';

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
        const selector = self.selectors[name];
        if (selector) {
          const { value, using = 'css selector' } = selector;
          return self.browser.expect.element(value, using);
        }

        throw new Error(`Cannot find selector: ${name}`);
      },
    });
  }

  goto() {
    return this.browser.url(resolveUrl(this.baseUrl, this.path));
  }
}

export default BasePage;

