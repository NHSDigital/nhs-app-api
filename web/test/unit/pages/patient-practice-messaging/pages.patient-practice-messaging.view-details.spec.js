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
        device: { isNativeApp: false },
      },
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
    describe('selected message id is defined', () => {
      beforeEach(async () => {
        mountPage({ selectedId: '1' });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will dispatch `patientPracticeMessaging/loadMessage` with id', () => {
        expect(store.dispatch).toBeCalledWith('patientPracticeMessaging/loadMessage', { id: '1', clearApiError: true });
      });
    });

    describe('selected message id is undefined', () => {
      beforeEach(async () => {
        mountPage();
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will not dispatch load', () => {
        expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/loadMessage');
      });

      it('will redirect to /patient-practice-messaging', () => {
        expect(redirect).toHaveBeenCalledWith('/patient-practice-messaging');
      });
    });
  });

  describe('mounted', () => {
    it('will dispatch update read status', () => {
      mountPage({ selectedId: '1', loaded: true });
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/updateReadStatusAsRead');
    });
  });
});
