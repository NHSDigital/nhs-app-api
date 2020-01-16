import Message from '@/pages/patient-practice-messaging/view-details';
import { create$T, createStore, mount } from '../../helpers';

describe('patient messaging messages', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const messageDetails = {
    messageDetails: {
      recipient: 'test',
      content: 'Test content',
      subject: 'Test subject',
      sentDateTime: '2019-12-09T13:56:50.377',
    },
  };

  const mountPage = ({
    messageDetaiils = messageDetails,
    toggle = true,
    selectedId = undefined,
    loaded = false } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageDetails: messageDetaiils,
          selectedMessageId: selectedId,
          loadedDetails: loaded,
          selectedMessageRecipient: 'test',
        },
        device: { isNativeApp: false } },
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
    });
    $t = create$T();

    wrapper = mount(Message, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('fetch', () => {
    let toggle;
    describe('has selected id', () => {
      const id = '1';
      beforeEach(async () => {
        toggle = true;
        mountPage({ toggle, selectedId: '1' });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will dispatch `patientPracticeMessaging/loadMessage` with id', () => {
        expect(store.dispatch).toBeCalledWith('patientPracticeMessaging/loadMessage', { id, clearApiError: true });
      });
    });
    describe('patient practice messaging toggle disabled', () => {
      beforeEach(async () => {
        toggle = 'false';

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will not dispatch load', () => {
        // Assert
        expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/loadMessage');
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });
  describe('mounted', () => {
    let toggle;
    beforeEach(async () => {
      toggle = true;
      mountPage({ toggle, selectedId: '1', loaded: true });
      await wrapper.vm.$options.fetch({ store, redirect });
    });

    it('will dispatch update read status', () => {
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/updateReadStatusAsRead');
    });

    it('will dispatch updateHeaderText', () => {
      expect(store.dispatch).toHaveBeenNthCalledWith(1, 'header/updateHeaderText', jasmine.anything());
    });

    it('will dispatch updatePageTitle', () => {
      expect(store.dispatch).toHaveBeenNthCalledWith(2, 'pageTitle/updatePageTitle', jasmine.anything());
    });
  });
});
