import UrgencyPage from '@/pages/patient-practice-messaging/urgency/index';
import { mount, createStore } from '../../helpers';
import { isFalsy, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient practice messaging urgency page', () => {
  let wrapper;
  let store;
  let redirect;

  const mountPage = ({
    toggle = true,
    isNativeApp = true,
    urgencyChoice = undefined,
  } = {}) => {
    store = createStore({
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
      state: {
        patientPracticeMessaging: { urgencyChoice },
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
    });

    describe('patient practice messaging toggle enabled', () => {
      it('will not redirect', async () => {
        toggle = true;
        isFalsy.mockImplementation(toggle).mockReturnValue(false);

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect });

        expect(redirect).not.toHaveBeenCalled();
      });
    });

    describe('patient practice messaging toggle disabled', () => {
      it('will redirect to home when patient practice messaging toggle disabled', async () => {
        toggle = 'false';
        isFalsy.mockImplementation(toggle).mockReturnValue(true);

        mountPage({ toggle });

        await wrapper.vm.$options.fetch({ store, redirect });
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
    it('will show a validation error when making no choice', () => {
      mountPage();

      wrapper.find('#continueButton').trigger('click');

      const errorContent = wrapper.find('[data-purpose="error-container"] [data-purpose="error"]');
      const validationError = errorContent.find('ul li');
      expect(errorContent.exists()).toBe(true);
      expect(errorContent.text()).toContain('translate_im02.noOptionSelectedErrorHeader');
      expect(validationError.text()).toEqual('translate_im02.noOptionSelectedErrorText');
    });

    it('will redirect to /patient-practice-messaging/contact-your-gp when the answer is yes', () => {
      mountPage({ urgencyChoice: 'yes' });

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/urgency/contact-your-gp');
    });

    it('will redirect to /patient-practice-messaging/recipients when the answer is no', () => {
      mountPage({ urgencyChoice: 'no' });

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/recipients');
    });
  });
});
