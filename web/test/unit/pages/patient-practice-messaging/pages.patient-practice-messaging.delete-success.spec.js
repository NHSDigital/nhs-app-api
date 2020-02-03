import DeleteSuccess from '@/pages/patient-practice-messaging/delete-success';
import { create$T, createStore, mount } from '../../helpers';
import { isFalsy, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient messaging delete success', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const mountPage = ({
    toggle = true,
    messageDeleted = true } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          messageDeleted,
        },
        device: { isNativeApp: false } },
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
    });
    $t = create$T();

    wrapper = mount(DeleteSuccess, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('fetch toggle check', () => {
    let toggle;
    describe('patient practice messaging toggle disabled', () => {
      beforeEach(async () => {
        toggle = 'false';
        isFalsy.mockImplementation(toggle).mockReturnValue(true);

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });
  describe('fetch deleted', () => {
    let messageDeleted;
    describe('patient practice messaging messaging deleted false', () => {
      beforeEach(async () => {
        messageDeleted = false;
        isFalsy.mockImplementation(messageDeleted).mockReturnValue(true);

        mountPage({ toggle: 'true', messageDeleted });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });
  describe('data', () => {
    it('will return the correct messages path', () => {
      expect(wrapper.vm.messagesPath).toBe('/patient-practice-messaging');
    });
  });
  describe('success page messages link clicked', () => {
    beforeEach(() => {
      mountPage();
    });

    it('will redirect to message list page', () => {
      wrapper.vm.goToMessagesClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging');
    });
  });
});
