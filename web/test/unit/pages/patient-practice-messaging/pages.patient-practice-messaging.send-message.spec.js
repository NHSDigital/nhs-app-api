import SendMessage from '@/pages/patient-practice-messaging/send-message';
import { create$T, createStore, mount } from '../../helpers';
import * as dependency from '@/lib/utils';

describe('patient messaging messages', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

  const mountPage = ({
    toggle = true,
    selectedMessageRecipient = undefined,
    messageSent = false,
    selectedId = undefined,
    sendMessageSubjectEnabled = true } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          selectedMessageId: selectedId,
          messageSent,
          selectedMessageRecipient,
        },
        device: { isNativeApp: false },
      },
      getters: {
        'serviceJourneyRules/sendMessageSubjectEnabled': sendMessageSubjectEnabled,
      },
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
    });
    $t = create$T();

    wrapper = mount(SendMessage, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('template', () => {
    it('will contain a subject field, a message field and a button', () => {
      mountPage({ selectedMessageRecipient: 'Recipient' });
      const subjectField = wrapper.find('#subjectText');
      const messageField = wrapper.find('#messageText');

      expect(subjectField.exists()).toBe(true);
      expect(messageField.exists()).toBe(true);
    });

    it('will show information with links', () => {
      mountPage({ selectedMessageRecipient: 'Recipient' });

      const subHeader = wrapper.find('#subHeader');
      expect(subHeader.exists()).toBe(true);

      const links = subHeader.findAll('a');
      expect(links.at(0).text()).toBe('translate_patient_practice_messaging.createMessage.nhs111Link');
      expect(links.at(1).text()).toBe('translate_patient_practice_messaging.createMessage.call111Link.');
    });

    it('will show validation errors if the input is invalid', () => {
      mountPage({ selectedMessageRecipient: 'Recipient' });
      wrapper.vm.subjectError = true;
      wrapper.vm.messageTextError = true;

      const errorDialog = wrapper.find('#errorDialog');
      const subjectError = wrapper.find('#subjectText-error-message');
      const messageError = wrapper.find('#messageText-error-message');

      expect(errorDialog.exists()).toBe(true);
      expect(subjectError.exists()).toBe(true);
      expect(messageError.exists()).toBe(true);
    });

    it('will not show the subject field if sendMessageSubjectEnabled is false in SJR', () => {
      mountPage({ selectedMessageRecipient: 'Recipient', sendMessageSubjectEnabled: false });
      const subjectField = wrapper.find('#subjectText');

      expect(subjectField.exists()).toBe(false);
    });

    it('will not validate the subject field if sendMessageSubjectEnabled is false in SJR', () => {
      mountPage({ sendMessageSubjectEnabled: false });
      wrapper.vm.subjectError = true;

      const errorDialog = wrapper.find('#errorDialog');
      const subjectError = wrapper.find('#subjectText-error-message');

      expect(errorDialog.exists()).toBe(false);
      expect(subjectError.exists()).toBe(false);
    });
  });

  describe('computed', () => {
    beforeEach(() => {
      mountPage({ selectedMessageRecipient: 'Recipient' });
    });
    it('will return true for showError if there is a message error or a subject error', () => {
      wrapper.vm.subjectError = true;
      expect(wrapper.vm.showError).toBe(true);
    });

    it('will return the correct back path', () => {
      expect(wrapper.vm.backPath).toBe('/patient-practice-messaging/recipients');
    });

    it('will set the error class if there is an error', () => {
      wrapper.vm.subjectError = true;
      expect(wrapper.vm.getErrorClass).toBe('nhsuk-form-group--error');
    });
  });

  describe('fetch', () => {
    it('will redirect to the messaging inbox page if the toggle is false', async () => {
      mountPage({ toggle: false });
      await wrapper.vm.$options.fetch({ store, redirect });
      expect(redirect).toHaveBeenCalledWith('/patient-practice-messaging');
    });
  });

  describe('methods', () => {
    it('will send the message if the inputs are valid', async () => {
      mountPage({ selectedMessageRecipient: 'Recipient', messageSent: true });
      dependency.redirectTo = jest.fn();

      wrapper.vm.messageText = 'Test message';
      wrapper.vm.subjectText = 'Test subject';

      await wrapper.vm.onSendMessageButtonClicked();

      expect(wrapper.vm.messageTextError).toBe(false);
      expect(wrapper.vm.subjectError).toBe(false);

      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/setSelectedMessageID', 0);
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/sendMessage', {
        messageText: 'Test message',
        subjectText: 'Test subject',
      });
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/view-details');
    });

    it('will not send the message if the inputs are invalid', async () => {
      mountPage({ selectedMessageRecipient: 'Recipient' });
      dependency.redirectTo = jest.fn();

      wrapper.vm.messageText = '';
      wrapper.vm.subjectText = '';

      await wrapper.vm.onSendMessageButtonClicked();

      expect(wrapper.vm.messageTextError).toBe(true);
      expect(wrapper.vm.subjectError).toBe(true);

      expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/setSelectedMessageID', 0);
      expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/sendMessage', {
        messageText: 'Test message',
        subjectText: 'Test subject',
      });
      expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, '/patient-practice-messaging/view-details');
    });
  });
});

