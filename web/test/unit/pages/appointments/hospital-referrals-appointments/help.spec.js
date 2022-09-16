import HelpPage from '@/pages/appointments/hospital-referrals-appointments/help';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils');

describe('One of three help links clicked from index page', () => {
  let $store;
  let wrapper;
  let referralsHelpTitle;
  let missingReferralsExpanderTitle;
  let missingReferralsText;
  let incorrectOrCancelledReferralsExpanderTitle;
  let cancelledReferralsContactText;
  let appointmentsHelpTitle;
  let missingAppointmentsExpanderTitle;
  let missingAppointmentsTextOne;
  let missingAppointmentsTextTwo;
  let incorrectChangedCancelledAppointmentsExpanderTitle;
  let incorrectChangedCancelledAppointmentsTextOne;
  let incorrectChangedCancelledAppointmentsTextTwo;
  let incorrectChangedCancelledAppointmentsTextThree;
  let incorrectChangedCancelledAppointmentsTextFour;
  let backButton;

  const createPage = () => mount(HelpPage, {
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
    it('show referrals help title', () => {
      wrapper = createPage();
      referralsHelpTitle = wrapper.find('#referralsHelpTitle');

      expect(referralsHelpTitle.exists()).toBe(true);
      expect(referralsHelpTitle.text()).toEqual('Referrals');
    });

    it('show missing referrals expander title', () => {
      wrapper = createPage();
      missingReferralsExpanderTitle = wrapper.find('#missingReferralsExpanderTitle');

      expect(missingReferralsExpanderTitle.exists()).toBe(true);
      expect(missingReferralsExpanderTitle.text()).toEqual('Missing referrals');
    });

    it('show missing referrals text', () => {
      wrapper = createPage();
      missingReferralsText = wrapper.find('#missingReferralsText');

      expect(missingReferralsText.exists()).toBe(true);
      expect(missingReferralsText.text()).toEqual('You may have referrals not shown that are in other services. Contact the healthcare provider that referred you for more information.');
    });

    it('show incorrect or cancelled referrals expander title', () => {
      wrapper = createPage();
      incorrectOrCancelledReferralsExpanderTitle = wrapper.find('#incorrectOrCancelledReferralsExpanderTitle');

      expect(incorrectOrCancelledReferralsExpanderTitle.exists()).toBe(true);
      expect(incorrectOrCancelledReferralsExpanderTitle.text()).toEqual('Incorrect or cancelled referrals');
    });

    it('show cancelled referrals contact text', () => {
      wrapper = createPage();
      cancelledReferralsContactText = wrapper.find('#cancelledReferralsContactText');

      expect(cancelledReferralsContactText.exists()).toBe(true);
      expect(cancelledReferralsContactText.text()).toEqual('If you have cancelled a referral and it\'s still showing, you need to contact the healthcare provider that referred you.');
    });

    it('show appointments help title', () => {
      wrapper = createPage();
      appointmentsHelpTitle = wrapper.find('#appointmentsHelpTitle');

      expect(appointmentsHelpTitle.exists()).toBe(true);
      expect(appointmentsHelpTitle.text()).toEqual('Appointments');
    });

    it('show missing appointments expander title', () => {
      wrapper = createPage();
      missingAppointmentsExpanderTitle = wrapper.find('#missingAppointmentsExpanderTitle');

      expect(missingAppointmentsExpanderTitle.exists()).toBe(true);
      expect(missingAppointmentsExpanderTitle.text()).toEqual('Missing appointments');
    });

    it('show missing appointments text one', () => {
      wrapper = createPage();
      missingAppointmentsTextOne = wrapper.find('#missingAppointmentsTextOne');

      expect(missingAppointmentsTextOne.exists()).toBe(true);
      expect(missingAppointmentsTextOne.text()).toEqual('You may have appointments not shown that are in other services. Contact the relevant organisation or healthcare provider for more information.');
    });

    it('show missing appointments text two', () => {
      wrapper = createPage();
      missingAppointmentsTextTwo = wrapper.find('#missingAppointmentsTextTwo');

      expect(missingAppointmentsTextTwo.exists()).toBe(true);
      expect(missingAppointmentsTextTwo.text()).toEqual('If you\'re aged 16 to 17, you may not be able to view or manage some of your hospital appointments. This is because some NHS Trusts require you to be aged 18 or over to access these appointments.');
    });

    it('show incorrect, changed or cancelled appointment expander title', () => {
      wrapper = createPage();
      incorrectChangedCancelledAppointmentsExpanderTitle = wrapper.find('#incorrectChangedCancelledAppointmentsExpanderTitle');

      expect(incorrectChangedCancelledAppointmentsExpanderTitle.exists()).toBe(true);
      expect(incorrectChangedCancelledAppointmentsExpanderTitle.text()).toEqual('Incorrect, changed or cancelled appointments');
    });

    it('show incorrect changed or cancelled appointments text one', () => {
      wrapper = createPage();
      incorrectChangedCancelledAppointmentsTextOne = wrapper.find('#incorrectChangedCancelledAppointmentsTextOne');

      expect(incorrectChangedCancelledAppointmentsTextOne.exists()).toBe(true);
      expect(incorrectChangedCancelledAppointmentsTextOne.text()).toEqual('You may have requested to change or permanently cancel a confirmed appointment. This request may not automatically be accepted.');
    });

    it('show incorrect changed or cancelled appointments text two', () => {
      wrapper = createPage();
      incorrectChangedCancelledAppointmentsTextTwo = wrapper.find('#incorrectChangedCancelledAppointmentsTextTwo');

      expect(incorrectChangedCancelledAppointmentsTextTwo.exists()).toBe(true);
      expect(incorrectChangedCancelledAppointmentsTextTwo.text()).toEqual('The appointment will show as pending while the request is reviewed by the relevant organisation or healthcare provider it\'s booked with.');
    });

    it('show incorrect changed or cancelled appointments text three', () => {
      wrapper = createPage();
      incorrectChangedCancelledAppointmentsTextThree = wrapper.find('#incorrectChangedCancelledAppointmentsTextThree');

      expect(incorrectChangedCancelledAppointmentsTextThree.exists()).toBe(true);
      expect(incorrectChangedCancelledAppointmentsTextThree.text()).toEqual('If the request to change or cancel the appointment is not accepted it will still show as booked in your confirmed appointments.');
    });

    it('show incorrect changed or cancelled appointments text four', () => {
      wrapper = createPage();
      incorrectChangedCancelledAppointmentsTextFour = wrapper.find('#incorrectChangedCancelledAppointmentsTextFour');

      expect(incorrectChangedCancelledAppointmentsTextFour.exists()).toBe(true);
      expect(incorrectChangedCancelledAppointmentsTextFour.text()).toEqual('If the cancellation is accepted the appointment will show as cancelled in your confirmed appointments.');
    });

    it('back button exists', () => {
      wrapper = createPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
      expect(backButton.text()).toEqual('Back');
    });
  });
});
