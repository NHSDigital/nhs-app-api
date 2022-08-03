import ConfirmedAppointmentsHelpPage from '@/pages/wayfinder/help/confirmed-appointments-help';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils');

describe('Missing, incorrect or cancelled confirmed appointments link clicked from index page', () => {
  let $store;
  let wrapper;
  let missingAppointmentsHelpTitle;
  let changeNotShowingHelpTitle;
  let missingAppointmentsHelpText;
  let under18HelpText;
  let changeNotShowingHelpTextOne;
  let changeNotShowingHelpTextTwo;
  let changeNotShowingHelpTextThree;
  let backButton;

  const createIndexPage = () => mount(ConfirmedAppointmentsHelpPage, {
    $store,
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
  });

  describe('if page loaded', () => {
    it('Missing confirmed appointments help title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      missingAppointmentsHelpTitle = wrapper.find('#missingAppointmentsHelpTitle');

      expect(missingAppointmentsHelpTitle.exists()).toBe(true);
      expect(missingAppointmentsHelpTitle.text()).toEqual('If something is missing');
    });

    it('Missing confirmed appointments help text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      missingAppointmentsHelpText = wrapper.find('#missingAppointmentsHelpText');

      expect(missingAppointmentsHelpText.exists()).toBe(true);
      expect(missingAppointmentsHelpText.text()).toEqual('You may have appointments not shown that are in other services. Contact the healthcare provider the appointment is booked with.');
    });

    it('Missing confirmed appointments under 18 help text is visible and contain correct text', () => {
      wrapper = createIndexPage();
      under18HelpText = wrapper.find('#under18HelpText');

      expect(under18HelpText.exists()).toBe(true);
      expect(under18HelpText.text()).toEqual('If you\'re aged 16 to 17, you may not be able to view or manage some of your hospital appointments. This is because some NHS Trusts require you to be aged 18 or over to access these appointments.');
    });

    it('Missing confirmed appointments change not showing help title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTitle = wrapper.find('#changeNotShowingHelpTitle');

      expect(changeNotShowingHelpTitle.exists()).toBe(true);
      expect(changeNotShowingHelpTitle.text()).toEqual('If a change or cancellation is not showing');
    });

    it('Missing confirmed appointments change not showing help text one is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTextOne = wrapper.find('#changeNotShowingHelpTextOne');

      expect(changeNotShowingHelpTextOne.exists()).toBe(true);
      expect(changeNotShowingHelpTextOne.text()).toEqual('You may have requested to change or permanently cancel a booked appointment. This request may not automatically be accepted.');
    });

    it('Missing confirmed appointments change not showing help text one is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTextOne = wrapper.find('#changeNotShowingHelpTextOne');

      expect(changeNotShowingHelpTextOne.exists()).toBe(true);
      expect(changeNotShowingHelpTextOne.text()).toEqual('You may have requested to change or permanently cancel a booked appointment. This request may not automatically be accepted.');
    });

    it('Missing confirmed appointments change not showing help text two is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTextTwo = wrapper.find('#changeNotShowingHelpTextTwo');

      expect(changeNotShowingHelpTextTwo.exists()).toBe(true);
      expect(changeNotShowingHelpTextTwo.text()).toEqual('The appointment will show as pending while the request is reviewed by the relevant organisation or healthcare provider it’s booked with.');
    });

    it('Missing confirmed appointments change not showing help text three is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTextThree = wrapper.find('#changeNotShowingHelpTextThree');

      expect(changeNotShowingHelpTextThree.exists()).toBe(true);
      expect(changeNotShowingHelpTextThree.text()).toEqual('If the request to change or cancel the appointment is not accepted it will still show as booked.');
    });

    it('back button exists', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
      expect(backButton.text()).toEqual('Back');
    });
  });
});
