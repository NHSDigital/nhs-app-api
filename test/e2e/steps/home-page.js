/* eslint-disable func-names */
// The Given, When, Then functions require the use of `this`.  In turn this required an actual
// function as opposed to an anonymous function.  Names, however, are rather pointless in this
// case.
import { Given, Then } from 'cucumber';
import HomePage from '../page-objects/HomePage';

Given('I am on the home page', function () {
  this.homePage = new HomePage({ world: this });
  return this.homePage.goto();
});

Then('I should see the NHS Online banner', function () {
  return this.homePage.expect.banner.to.be.visible;
});
