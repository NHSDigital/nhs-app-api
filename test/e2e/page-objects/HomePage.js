import BasePage from './BasePage';

class HomePage extends BasePage {
  constructor({ world }) {
    super({ path: '/', world });
    this.selectors.banner = 'h1';
  }
}

export default HomePage;
