import ReferralsOrAppointmentsHelpPage from '@/pages/wayfinder/help/referrals-or-appointments-help';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils');

describe('Missing or incorrect referrals or appointments link clicked from index page', () => {
  let $store;
  let wrapper;
  let missingAppointmentsHelpTitle;
  let changeNotShowingHelpTitle;
  let missingAppointmentsHelpText;
  let Under18HelpText;
  let changeNotShowingHelpText;
  let changeNeedApprovalHelpText;
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
      Under18HelpText = wrapper.find('#Under18HelpText');

      expect(Under18HelpText.exists()).toBe(true);
      expect(Under18HelpText.text()).toEqual('If you\'re aged 16 to 17, you may not be able to view or manage some of your hospital appointments. This is because some NHS Trusts require you to be aged 18 or over to access these appointments.');
    });

    it('Missing referrals or appointments change not showing help title is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpTitle = wrapper.find('#changeNotShowingHelpTitle');

      expect(changeNotShowingHelpTitle.exists()).toBe(true);
      expect(changeNotShowingHelpTitle.text()).toEqual('If a change or cancellation is not showing');
    });

    it('Missing referrals or appointments change not showing help text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNotShowingHelpText = wrapper.find('#changeNotShowingHelpText');

      expect(changeNotShowingHelpText.exists()).toBe(true);
      expect(changeNotShowingHelpText.text()).toEqual('You may have requested to change or cancel a referral or appointment.');
    });

    it('Missing referrals or appointments change need approval help text is visible and contains correct text', () => {
      wrapper = createIndexPage();
      changeNeedApprovalHelpText = wrapper.find('#changeNeedApprovalHelpText');

      expect(changeNeedApprovalHelpText.exists()).toBe(true);
      expect(changeNeedApprovalHelpText.text()).toEqual('Any updates you have made may not be shown until the request is approved by the relevant organisation or healthcare provider.');
    });

    it('back button exists', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
      expect(backButton.text()).toEqual('Back');
    });
  });
});
