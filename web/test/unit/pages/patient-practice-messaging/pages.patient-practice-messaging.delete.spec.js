import Delete from '@/pages/patient-practice-messaging/delete';
import { create$T, createStore, mount } from '../../helpers';
import { isFalsy, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient messaging delete', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const mountPage = ({
    toggle = true,
    selectedId = undefined,
    messageDeleted = false } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageId: selectedId,
          messageDeleted,
        },
        device: { isNativeApp: false } },
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
      router: {
        currentRoute: {
          path: '/patient-practice-messaging/view-details',
        },
      },
    });
    $t = create$T();

    wrapper = mount(Delete, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('fetch', () => {
    let toggle;
    describe('patient practice messaging toggle disabled', () => {
      beforeEach(async () => {
        toggle = 'false';
        isFalsy.mockImplementation(toggle).mockReturnValue(true);

        mountPage({ toggle, selectedId: 1 });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });
  describe('computed and data', () => {
    beforeEach(async () => {
      mountPage({ selectedId: 1 });
    });
    it('will return the correct messages path', () => {
      expect(wrapper.vm.messagesPath).toBe('/patient-practice-messaging');
    });

    it('will return the correct message details path', () => {
      expect(wrapper.vm.messageDetailsPath).toBe('/patient-practice-messaging/view-details');
    });

    it('will return the correct message id', () => {
      expect(wrapper.vm.messageID).toBe(1);
    });

    it('will return the correct delete button text reference', () => {
      expect(wrapper.vm.buttonText).toBe('patient_practice_messaging.delete.backButtonText.text');
    });
  });
  describe('back link clicked', () => {
    beforeEach(() => {
      mountPage({ isNativeApp: false, selectedId: 1 });
    });

    it('will redirect to message details page', () => {
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/view-details');
    });
  });
  describe('delete clicked', () => {
    beforeEach(() => {
      mountPage({ isNativeApp: false, selectedId: 1, messageDeleted: true });
    });

    it('will call delete', () => {
      wrapper.vm.deleteButtonClicked();
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/deleteMessage', 1);
    });

    it('will call redirect', async () => {
      await wrapper.vm.deleteButtonClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/delete-success');
    });
  });
});
