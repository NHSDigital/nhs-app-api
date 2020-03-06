import Delete from '@/pages/patient-practice-messaging/delete';
import { createStore, mount } from '../../helpers';
import { redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient messaging delete', () => {
  let wrapper;
  let store;
  let redirect;

  const mountPage = ({
    selectedId = undefined,
    messageDeleted = false,
    currentRoutePath = '/patient-practice-messaging/view-details',
  } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageId: selectedId,
          messageDeleted,
        },
        device: { isNativeApp: false },
      },
      router: {
        currentRoute: { path: currentRoutePath },
      },
    });
    wrapper = mount(Delete, {
      $store: store,
    });
  };

  describe('fetch', () => {
    beforeEach(() => {
      redirect = jest.fn();
    });

    describe('selected message id is undefined', () => {
      it('will redirect to patient practice messaging', async () => {
        mountPage();
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).toHaveBeenCalledWith('/patient-practice-messaging');
      });
    });

    describe('current route is not patient practice messaging view details', () => {
      it('will redirect to patient practice messaging', async () => {
        mountPage({ currentRoutePath: '/more' });
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).toHaveBeenCalledWith('/patient-practice-messaging');
      });
    });

    describe('current route is view details and selected message id is defined', () => {
      it('will not redirect', async () => {
        mountPage({ selectedId: 1 });
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).not.toHaveBeenCalled();
      });
    });
  });

  describe('computed and data', () => {
    beforeEach(() => {
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
    it('will redirect to message details page', () => {
      mountPage({ isNativeApp: false, selectedId: 1 });
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/view-details');
    });
  });

  describe('delete clicked', () => {
    beforeEach(async () => {
      mountPage({ isNativeApp: false, selectedId: 1, messageDeleted: true });
      await wrapper.vm.deleteButtonClicked();
    });

    it('will call delete', () => {
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/deleteMessage', 1);
    });

    it('will call redirect', () => {
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/delete-success');
    });
  });
});
