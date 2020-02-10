import Page from '@/pages/patient-practice-messaging/index';
import SummaryMessage from '@/components/messaging/SummaryMessage';
import { createStore, create$T, mount } from '../../helpers';
import { formatDate } from '@/plugins/filters';

jest.mock('@/plugins/filters');

describe('practice patient messaging inbox', () => {
  const messageItemClass = 'nhs-app-message__item';
  let wrapper;
  let store;
  let redirect;
  let $t;

  const summaries = [{
    id: 'message-1',
    recipient: 'Dr NHS Online',
    subject: 'This is the message subject',
    lastMessageDateTime: '2020-01-01T13:37:00.137Z',
    hasUnreadReplies: true,
  }];

  const mountPage = ({
    messageSummaries = summaries,
    loadedMessages = true,
    toggle = true,
  } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          messageSummaries,
          loadedMessages,
          urgencyChoice: 'yes',
        },
      },
      $env: { PATIENT_PRACTICE_MESSAGING_ENABLED: toggle },
    });
    $t = create$T();

    wrapper = mount(Page, {
      $store: store,
      $t,
      $style: {
        [messageItemClass]: messageItemClass,
      },
    });
  };

  beforeAll(() => {
    formatDate.mockImplementation('2020-01-01T13:37:00.137Z', 'D MMMM YYYY').mockReturnValue('mock formatted date');
  });

  describe('fetch', () => {
    let toggle;

    beforeEach(() => {
      redirect = jest.fn();
    });

    describe('patient practice messaging toggle enabled', () => {
      beforeEach(async () => {
        toggle = true;

        mountPage({ toggle });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will dispatch load', () => {
        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/loadMessages');
      });

      it('will not redirect', () => {
        expect(redirect).not.toHaveBeenCalled();
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
        expect(store.dispatch).not.toHaveBeenCalledWith('patientPracticeMessaging/loadMessages');
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });

  describe('mounted', () => {
    it('will dispatch action to clear urgency choice', () => {
      mountPage();
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/setUrgencyChoice', undefined);
    });
  });

  describe('get message label', () => {
    it('will return a descriptive sentence about that message', () => {
      // Act
      mountPage();
      const messageLabel = wrapper.vm.getMessageLabel(summaries[0]);

      // Assert
      expect($t).toHaveBeenCalledWith('im01.summary.hidden', { recipient: 'Dr NHS Online', subject: 'This is the message subject', date: 'mock formatted date' });
      expect(messageLabel).toEqual('translate_im01.summary.hidden');
    });
  });

  describe('has no messages', () => {
    it('will show no messages message', () => {
      // Act
      mountPage({ messageSummaries: [] });

      // Assert
      expect(wrapper.find('p').text()).toEqual('translate_im01.noMessages');
    });
  });

  describe('has messages', () => {
    it('will display a summary item per message summary', () => {
      // Act
      mountPage();
      const messageItems = wrapper.findAll(`.${messageItemClass}`);
      const summaryMessages = wrapper.findAll(SummaryMessage);
      const summaryMessage = summaryMessages.wrappers[0];

      // Assert
      expect(messageItems.length).toBe(1);
      expect(summaryMessages.length).toBe(1);
      expect(summaryMessage.element.id).toEqual('message-1');
      expect(summaryMessage.vm.$props.title).toEqual('Dr NHS Online');
      expect(summaryMessage.vm.$props.subTitle).toEqual('This is the message subject');
      expect(summaryMessage.vm.$props.dateTime).toEqual('2020-01-01T13:37:00.137Z');
      expect(summaryMessage.vm.$props.dateFormat).toBeUndefined();
      expect(summaryMessage.vm.$props.ariaLabel).toEqual('translate_im01.summary.hidden');
      expect(summaryMessage.vm.$props.hasUnreadMessages).toBe(true);
    });
  });
});
