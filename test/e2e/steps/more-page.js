import { Given, Then, When } from 'cucumber';
import MorePage from '../page-objects/MorePage';

Given('I am on the more page', function () {
  this.morePage = new MorePage({ world: this });
  return this.morePage.goto();
});

When('I invoke organ donation', function() {
  return this.morePage.invoke.organDonationButton;
});

Then('I should be on the organ donation register', function() {
  return this.morePage.expect.url.toEqual('https://www.organdonation.nhs.uk/');
});
