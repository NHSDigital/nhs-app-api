import { Given, When, Then } from 'cucumber';
import HomePage from '../page-objects/HomePage';

Given('I am on the home page', function () {
  this.homePage = new HomePage({ world: this });
  return this.homePage.goto();
});

Then('I should see the NHS Online banner', function () {
  return this.homePage.expect.banner.to.be.visible;
});

Then('I should a see mechanism for initiating login', function () {
  return this.homePage.expect.loginButton.to.be.visible;
});

Then('I should a see mechanism for initiating the creation of a new account', function () {
  return this.homePage.expect.createAccountButton.to.be.visible;
});

When('I invoke symptom checker', function () {
  return this.homePage.invoke.symptomCheckerButton;
});
