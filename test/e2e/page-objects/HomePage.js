import BasePage from './BasePage';
import { css, textContains } from '../support/selectors';

class HomePage extends BasePage {
  constructor({ world }) {
    super({ path: '/', world });
    this.selectors.banner = css('h1');
    this.selectors.createAccountButton = textContains({
      element: 'button',
      value: 'Create an NHS account',
    });

    this.selectors.loginButton = textContains({
      element: 'button',
      value: 'Sign in with your NHS account',
    });
  }
}

export default HomePage;
