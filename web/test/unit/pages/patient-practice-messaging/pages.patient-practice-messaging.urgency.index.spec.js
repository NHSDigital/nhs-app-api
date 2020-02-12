import UrgencyPage from '@/pages/patient-practice-messaging/urgency/index';
import { mount, createStore } from '../../helpers';
import { isFalsy, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient practice messaging urgency page', () => {
  let wrapper;
  let store;
  let redirect;
  let app;

  const mountPage = ({
    toggle = true,
    isNativeApp = true,
    messageRecipients = [],
    urgencyChoice = undefined,
  } = {}) => {
    store = createStore({
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
      state: {
        patientPracticeMessaging: {
          urgencyChoice,
          messageRecipients,
        },
        device: { isNativeApp },
      },
    });
    wrapper = mount(UrgencyPage, {
      $store: store,
    });
  };

  describe('fetch', () => {
    let toggle;

    beforeEach(() => {
      redirect = jest.fn();

      app = {
        i18n: {
          t: jest.fn(),
        },
      };
    });

    describe('patient practice messaging toggle enabled', () => {
      it('will not redirect', async () => {
        toggle = true;
        isFalsy.mockImplementation(toggle).mockReturnValue(false);

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect, app });

        expect(redirect).not.toHaveBeenCalled();
        expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/loadRecipients');
      });

      it('will inform me if there is no recipients', async () => {
        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect, app });

        expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/loadRecipients');
        expect(wrapper.find('#noRecipients').exists()).toBe(true);
      });
    });

    describe('patient practice messaging toggle disabled', () => {
      it('will redirect to home when patient practice messaging toggle disabled', async () => {
        toggle = 'false';
        isFalsy.mockImplementation(toggle).mockReturnValue(true);

        mountPage({ toggle });

        await wrapper.vm.$options.fetch({ store, redirect, app });
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });

  describe('back link clicked', () => {
    beforeEach(() => {
      mountPage({ isNativeApp: false });
    });

    it('will redirect to patient practice messaging inbox', () => {
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging');
    });
  });

  describe('continue clicked', () => {
    it('will show a validation error when making no choice', async () => {
      const messageRecipients = [{ recipientGuid: '1', name: 'Dr. Test' }];
      mountPage({ messageRecipients });

      await wrapper.vm.$options.fetch({ store, redirect, app });

      wrapper.find('#continueButton').trigger('click');

      const errorContent = wrapper.find('[data-purpose="error-container"] [data-purpose="error"]');
      const validationError = errorContent.find('ul li');
      expect(errorContent.exists()).toBe(true);
      expect(errorContent.text()).toContain('translate_im02.noOptionSelectedErrorHeader');
      expect(validationError.text()).toEqual('translate_im02.noOptionSelectedErrorText');
    });

    it('will redirect to /patient-practice-messaging/contact-your-gp when the answer is yes', async () => {
      const messageRecipients = [{ recipientGuid: '1', name: 'Dr. Test' }];
      mountPage({ urgencyChoice: 'yes', messageRecipients });

      await wrapper.vm.$options.fetch({ store, redirect, app });

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/urgency/contact-your-gp');
    });

    it('will redirect to /patient-practice-messaging/recipients when the answer is no', async () => {
      const messageRecipients = [{ recipientGuid: '1', name: 'Dr. Test' }];
      mountPage({ urgencyChoice: 'no', messageRecipients });

      await wrapper.vm.$options.fetch({ store, redirect, app });

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/recipients');
    });
  });
});
