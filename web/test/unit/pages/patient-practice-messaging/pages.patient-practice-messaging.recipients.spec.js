import RecipientsPage from '@/pages/patient-practice-messaging/recipients';
import { mount, createStore } from '../../helpers';
import { isFalsy, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient practice messaging recipients page', () => {
  let wrapper;
  let store;
  let redirect;

  const mountPage = ({
    toggle = true,
    isNativeApp = true,
  } = {}) => {
    store = createStore({
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
      state: {
        patientPracticeMessaging: {
          messageRecipients: [],
          loadedRecipients: true,
        },
        device: { isNativeApp },
      },
    });
    wrapper = mount(RecipientsPage, {
      $store: store,
    });
  };

  describe('fetch', () => {
    let toggle;

    beforeEach(() => {
      redirect = jest.fn();
    });

    describe('patient practice messaging toggle enabled', () => {
      beforeEach(async () => {
        toggle = true;
        isFalsy.mockImplementation(toggle).mockReturnValue(false);

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will dispatch load', () => {
        expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/loadRecipients');
      });

      it('will not redirect', () => {
        expect(redirect).not.toHaveBeenCalled();
      });
    });

    describe('patient practice messaging toggle disabled', () => {
      beforeEach(async () => {
        toggle = 'false';
        isFalsy.mockImplementation(toggle).mockReturnValue(true);

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will not dispatch load', () => {
        expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/loadRecipients');
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });

    describe('back link clicked', () => {
      beforeEach(() => {
        mountPage({ isNativeApp: false });
      });

      it('will redirect to urgency question', () => {
        wrapper.vm.backLinkClicked();
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/urgency');
      });
    });
  });
});
