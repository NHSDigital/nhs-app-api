import Page from '@/pages/patient-practice-messaging/index';
import SummaryMessage from '@/components/messaging/SummaryMessage';
import { createStore, create$T, mount } from '../../helpers';
import { formatDate } from '@/plugins/filters';
import { INDEX } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');
jest.mock('@/plugins/filters');

describe('practice patient messaging inbox', () => {
  const messageItemClass = 'nhs-app-message__item';
  let wrapper;
  let store;
  let $t;

  const redirect = jest.fn();

  const summaries = [{
    messageId: 'message-1',
    recipient: 'Dr NHS Online',
    subject: 'This is the message subject',
    lastMessageDateTime: '2020-01-01T13:37:00.137Z',
    hasUnreadReplies: true,
    content: 'test',
    sentDateTime: '2020-01-01T13:37:00.137Z',
    sender: 'Dr Nhs Online',
    replies: [],
    outboundMessage: true,
    requiresDetailsRequest: false,
  }];

  const mountPage = ({
    messageSummaries = summaries,
    loadedMessages = true,
    im1MessagingEnabled = false,
  } = {}) => {
    store = createStore({
      state: {
        patientPracticeMessaging: {
          messageSummaries,
          loadedMessages,
          urgencyChoice: 'yes',
        },
        practiceSettings: {
          im1MessagingEnabled,
        },
        device: {
          isNativeApp: false,
        },
      },
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

  describe('asyncData', () => {
    it('will dispatch load if im1MessagingEnabled is enabled for the practice', async () => {
      mountPage({ im1MessagingEnabled: true });
      await wrapper.vm.$options.asyncData({ store, redirect });
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/loadMessages');
    });

    it('will redirect if im1MessagingEnabled is not enabled for the practice', async () => {
      mountPage();
      await wrapper.vm.$options.asyncData({ store, redirect });
      expect(redirect).toHaveBeenCalledWith(INDEX.path);
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
      expect($t).toHaveBeenCalledWith('im01.summary.hiddenWithSubject', { recipient: 'Dr NHS Online', subject: 'This is the message subject', date: 'mock formatted date' });
      expect(messageLabel).toEqual('translate_im01.summary.hiddenWithSubject');
    });

    it('will return a descriptive sentence without the subject about that message', () => {
      // Act
      const summariesNoSubject = [{
        id: 'message-1',
        recipient: 'Dr NHS Online',
        lastMessageDateTime: '2020-01-01T13:37:00.137Z',
        hasUnreadReplies: true,
      }];
      mountPage({ messagesSummaries: summariesNoSubject });
      const messageLabel = wrapper.vm.getMessageLabel(summariesNoSubject[0]);

      // Assert
      expect($t).toHaveBeenCalledWith('im01.summary.hiddenWithoutSubject', { recipient: 'Dr NHS Online', date: 'mock formatted date' });
      expect(messageLabel).toEqual('translate_im01.summary.hiddenWithoutSubject');
    });
  });

  describe('back link', () => {
    it('will go back to the more screen', () => {
      mountPage();
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/more');
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
      expect(summaryMessage.vm.$props.ariaLabel).toEqual('translate_im01.summary.hiddenWithSubject');
      expect(summaryMessage.vm.$props.hasUnreadMessages).toBe(true);
    });
  });

  describe('methods', () => {
    it('will set message details if required', async () => {
      mountPage();
      await wrapper.vm.goToMessageDetails(summaries[0]);
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/setSelectedMessageID', summaries[0].messageId);
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/setSelectedRecipient', { name: summaries[0].recipient });
      expect(store.dispatch).toHaveBeenCalledWith('patientPracticeMessaging/setMessageDetails', {
        messageDetails: {
          content: 'test',
          sentDateTime: '2020-01-01T13:37:00.137Z',
          sender: 'Dr Nhs Online',
          messageReplies: [],
          outboundMessage: true,
        },
      });
    });
  });
});
