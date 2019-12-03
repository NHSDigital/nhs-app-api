import Message from '@/pages/patient-practice-messaging/view-details';
import { create$T, createStore, mount } from '../../helpers';

describe('patient messaging messages', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const messageDetails = {
    recipient: 'test',
    content: 'Test content',
    subject: 'Test subject',
    sentDateTime: '2019-12-09T13:56:50.377',
  };

  const mountPage = ({
    messageDetaiils = messageDetails,
    toggle = true,
    selectedId = undefined } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageDetails: messageDetaiils,
          selectedMessageId: selectedId,
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
        expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/load');
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });
});
