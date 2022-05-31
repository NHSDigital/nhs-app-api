import ConfirmedAppointmentsHelpPage from '@/pages/wayfinder/help/confirmed-appointments-help';
import { createStore, mount } from '../../../helpers';

jest.mock('@/lib/utils');

describe('Missing or incorrect confirmed appointments link clicked from index page', () => {
  let $store;
  let wrapper;
  let title1;
  let title2;
  let text1;
  let text2;
  let text3;
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
    it('title 1 is visible and contains correct text', () => {
      wrapper = createIndexPage();
      title1 = wrapper.find('#title1');

      expect(title1.exists()).toBe(true);
      expect(title1.text()).toEqual('If something is missing');
    });

    it('text 1 is visible and contains correct text', () => {
      wrapper = createIndexPage();
      text1 = wrapper.find('#text1');

      expect(text1.exists()).toBe(true);
      expect(text1.text()).toEqual('You may have appointments not shown that are in other services. Contact the healthcare provider the appointment is booked with.');
    });

    it('title 2 is visible and contains correct text', () => {
      wrapper = createIndexPage();
      title2 = wrapper.find('#title2');

      expect(title2.exists()).toBe(true);
      expect(title2.text()).toEqual('If a change or cancellation is not showing');
    });

    it('text 2 is visible and contains correct text', () => {
      wrapper = createIndexPage();
      text2 = wrapper.find('#text2');

      expect(text2.exists()).toBe(true);
      expect(text2.text()).toEqual('You may have requested to change or cancel a booked appointment.');
    });

    it('text 3 is visible and contains correct text', () => {
      wrapper = createIndexPage();
      text3 = wrapper.find('#text3');

      expect(text3.exists()).toBe(true);
      expect(text3.text()).toEqual('Any updates you have made may not be shown until the request is approved by the healthcare provider the appointment is booked with.');
    });

    it('back button exists', () => {
      wrapper = createIndexPage();
      backButton = wrapper.find('#desktopBackLink');

      expect(backButton.exists()).toBe(true);
      expect(backButton.text()).toEqual('Back');
    });
  });
});
