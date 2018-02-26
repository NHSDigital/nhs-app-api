import BasePage from './BasePage';
import { css } from '../support/selectors';

class MorePage extends BasePage {
  constructor({ world }) {
    super({ path: '/more', world });
    this.selectors.organDonationButton = css('#btn_organdonation');
  }
}

export default MorePage;
