import ReferralsOrAppointmentsHelpPage from '@/pages/wayfinder/help/referrals-or-appointments-help';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils');

describe('Missing or incorrect referrals or appointments link clicked from index page', () => {
  let $store;
  let wrapper;
  let missingAppointmentsHelpTitle;
  let changeNotShowingHelpTitle;
  let missingAppointmentsHelpText;
  let under18HelpText;
  let changeNotShowingHelpText;
  let appointmentChangeOrCancellationNotShowingTitle;
  let changeOrCancellationTextOne;
  let changeOrCancellationTextTwo;
  let changeOrCancellationTextThree;
  let backButton;

  const createIndexPage = () => mount(ReferralsOrAppointmentsHelpPage, {
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
    it('Missing referrals or appointments help title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      missingAppointmentsHelpTitle = wrapper.find('#missingAppointmentsHelpTitle');

      expect(missingAppointmentsHelpTitle.exists()).toBe(true);
      expect(missingAppointmentsHelpTitle.text()).toEqual('If something is missing');
    });

    it('Missing referrals or appointments help text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      missingAppointmentsHelpText = wrapper.find('#missingAppointmentsHelpText');

      expect(missingAppointmentsHelpText.exists()).toBe(true);
      expect(missingAppointmentsHelpText.text()).toEqual('You may have referrals or appointments not shown that are in other services. Contact the relevant organisation or healthcare provider.');
    });

    it('Missing referrals or appointments under 18 help text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      under18HelpText = wrapper.find('#under18HelpText');

      expect(under18HelpText.exists()).toBe(true);
      expect(under18HelpText.text()).toEqual('If you\'re aged 16 to 17, you may not be able to view or manage some of your hospital appointments. This is because some NHS Trusts require you to be aged 18 or over to access these appointments.');
    });

    it('Missing referrals or appointments change not showing help title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTitle = wrapper.find('#changeNotShowingHelpTitle');

      expect(changeNotShowingHelpTitle.exists()).toBe(true);
      expect(changeNotShowingHelpTitle.text()).toEqual('Cancelled referrals');
    });

    it('Missing referrals or appointments change not showing help text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpText = wrapper.find('#changeNotShowingHelpText');

      expect(changeNotShowingHelpText.exists()).toBe(true);
      expect(changeNotShowingHelpText.text()).toEqual('If you have cancelled a referral and it’s still showing, you need to contact the healthcare provider that referred you.');
    });

    it('Appointment change or cancellation not showing help title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      appointmentChangeOrCancellationNotShowingTitle = wrapper.find('#appointmentChangeOrCancellationNotShowingTitle');

      expect(appointmentChangeOrCancellationNotShowingTitle.exists()).toBe(true);
      expect(appointmentChangeOrCancellationNotShowingTitle.text()).toEqual('If an appointment change or cancellation is not showing');
    });

    it('Appointment change or cancellation not showing help text one is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeOrCancellationTextOne = wrapper.find('#changeOrCancellationTextOne');

      expect(changeOrCancellationTextOne.exists()).toBe(true);
      expect(changeOrCancellationTextOne.text()).toEqual('If you have requested to change or permanently cancel an appointment this request may not automatically be accepted.');
    });

    it('Appointment change or cancellation not showing help text two is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeOrCancellationTextTwo = wrapper.find('#changeOrCancellationTextTwo');

      expect(changeOrCancellationTextTwo.exists()).toBe(true);
      expect(changeOrCancellationTextTwo.text()).toEqual('The appointment will show as pending while the request is reviewed by the relevant organisation or healthcare provider it’s booked with.');
    });

    it('Appointment change or cancellation not showing help text three is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeOrCancellationTextThree = wrapper.find('#changeOrCancellationTextThree');

      expect(changeOrCancellationTextThree.exists()).toBe(true);
      expect(changeOrCancellationTextThree.text()).toEqual('If the request to change or cancel the appointment is not accepted it will still show as booked.');
    });

    it('back button exists', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
      expect(backButton.text()).toEqual('Back');
    });
  });
});
