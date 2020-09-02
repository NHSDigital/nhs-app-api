import i18n from '@/plugins/i18n';
import last from 'lodash/fp/last';
import Page from '@/pages/messages/gp-messages/index';
import SummaryMessage from '@/components/messaging/SummaryMessage';
import { formatDate } from '@/plugins/filters';
import { isEmptyArray, redirectTo } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');
jest.mock('@/plugins/filters');
jest.mock('lodash/fp/last');

isEmptyArray.mockReturnValue(true);

describe('practice patient messaging inbox', () => {
  const messageItemClass = 'nhs-app-message__item';
  let wrapper;
  let store;

  const summaries = [{
    messageId: 'message-1',
    recipient: 'Dr NHS Online',
    subject: 'This is the message subject',
    lastMessageDateTime: '2020-01-01T13:37:00.137Z',
    unreadReplyInfo: {
      present: true,
      count: 1,
    },
    content: 'test',
    sentDateTime: '2020-01-01T13:37:00.137Z',
    sender: 'Dr Nhs Online',
    replies: [],
    outboundMessage: true,
    requiresDetailsRequest: false,
  }, {
    messageId: 'message-2',
    recipient: 'Dr NHS Online2',
    subject: 'This is the message subject 2',
    lastMessageDateTime: '2020-01-01T13:37:00.137Z',
    unreadReplyInfo: {
      present: false,
      count: 0,
    },
    content: 'test',
    sentDateTime: '2020-01-01T13:37:00.137Z',
    sender: 'Dr Nhs Online2',
    replies: [],
    outboundMessage: true,
    requiresDetailsRequest: false,
  }];

  const mountPage = ({
    messageSummaries = summaries,
    loadedMessages = true,
    im1MessagingEnabled = false,
    hasSubject = true,
  } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
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
      getters: {
        'serviceJourneyRules/sendMessageSubjectEnabled': hasSubject,
      },
    });

    wrapper = mount(Page, {
      $store: store,
      $style: {
        [messageItemClass]: messageItemClass,
      },
      mountOpts: {
        i18n,
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  beforeAll(() => {
    formatDate.mockImplementation('2020-01-01T13:37:00.137Z', 'D MMMM YYYY').mockReturnValue('mock formatted date');
  });

  describe('created', () => {
    describe('practice settings im1 messaging enabled', () => {
      beforeAll(() => {
        mountPage({ im1MessagingEnabled: true });
      });

      it('will clear selected message and recipient if im1MessagingEnabled is enabled for the practice', async () => {
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/clearSelectedRetainingId');
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/clearSelectedRecipient');
      });

      it('will dispatch load if im1MessagingEnabled is enabled for the practice', async () => {
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/loadMessages');
      });

      it('will dispatch action to clear urgency choice', () => {
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/setUrgencyChoice', undefined);
      });
    });

    describe('practice settings im1 messaging disabled', () => {
      beforeEach(() => {
        mountPage();
      });

      it('will redirect if im1MessagingEnabled is not enabled for the practice', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
      });
    });
  });

  describe('get message label', () => {
    it('will return a descriptive sentence about that message', () => {
      // Act
      mountPage();
      const messageLabel = wrapper.vm.getMessageLabel(summaries[0]);

      // Assert
      expect(messageLabel).toEqual('Conversation with Dr NHS Online. Subject: This is the message subject. The last message in this conversation was sent on mock formatted date.');
    });

    it('will return a descriptive sentence without the subject about that message', () => {
      // Act
      const summariesNoSubject = [{
        id: 'message-1',
        recipient: 'Dr NHS Online',
        lastMessageDateTime: '2020-01-01T13:37:00.137Z',
        hasUnreadReplies: true,
      }];
      mountPage({ hasSubject: false });
      const messageLabel = wrapper.vm.getMessageLabel(summariesNoSubject[0]);

      // Assert
      expect(messageLabel).toEqual('Conversation with Dr NHS Online. The last message in this conversation was sent on mock formatted date. View full message.');
    });
  });

  describe('back link', () => {
    it('will go back to the messages hub', () => {
      mountPage();
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages');
    });
  });

  describe('has no messages', () => {
    it('will show no messages message', () => {
      // Act
      mountPage({ messageSummaries: [] });

      // Assert
      expect(wrapper.find('p').text()).toEqual('You have no messages.');
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
      expect(messageItems.length).toBe(2);
      expect(summaryMessages.length).toBe(2);
      expect(summaryMessage.element.id).toEqual('message-1');
      expect(summaryMessage.vm.$props.title).toEqual('Dr NHS Online');
      expect(summaryMessage.vm.$props.subTitle).toEqual('This is the message subject');
      expect(summaryMessage.vm.$props.dateTime).toEqual('2020-01-01T13:37:00.137Z');
      expect(summaryMessage.vm.$props.dateFormat).toBeUndefined();
      expect(summaryMessage.vm.$props.ariaLabel).toEqual('Conversation with Dr NHS Online. Subject: This is the message subject. The last message in this conversation was sent on mock formatted date.');
      expect(summaryMessage.vm.$props.hasUnreadMessages).toBe(true);
    });
  });

  describe('methods', () => {
    describe('goToMessageDetails', () => {
      describe('message has no unread replies', () => {
        beforeEach(() => {
          mountPage();
          wrapper.vm.goToMessageDetails(summaries[1]);
        });

        it('will redirect to view gp message path without unread messages url hash', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/view-details');
        });
      });

      describe('message has unread replies', () => {
        beforeEach(() => {
          mountPage();
          wrapper.vm.goToMessageDetails(summaries[0]);
        });

        it('will redirect to view gp message path with unread messages url hash', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/view-details#unreadMessages');
        });

        it('will set message details if required', () => {
          expect(store.dispatch).toHaveBeenCalledWith('gpMessages/setSelectedMessageID', summaries[0].messageId);
          expect(store.dispatch).toHaveBeenCalledWith('gpMessages/setSelectedRecipient', { name: summaries[0].recipient });
          expect(store.dispatch).toHaveBeenCalledWith('gpMessages/setMessageDetails', {
            messageDetails: summaries[0],
          });
        });
      });
    });

    describe('getSubtitle', () => {
      it('will not trim the subtitle if it is 64 characters and there is no subject', () => {
        const messageSummaries = [{
          messageId: 'message-1',
          recipient: 'Dr NHS Online',
          subject: 'This is the message subject',
          lastMessageDateTime: '2020-01-01T13:37:00.137Z',
          unreadReplyInfo: {
            present: true,
            count: 1,
          },
          content: 'This is sixty-four characters long to check this class method!!!',
          sentDateTime: '2020-01-01T13:37:00.137Z',
          sender: 'Dr Nhs Online',
          replies: [],
          outboundMessage: true,
          requiresDetailsRequest: false,
        }];
        mountPage({ hasSubject: false });
        const subtitle = wrapper.vm.getSubtitle(messageSummaries[0]);
        expect(subtitle).toBe('This is sixty-four characters long to check this class method!!!');
      });

      it('will not trim the subtitle if it is less than 64 characters and there is no subject', () => {
        const messageSummaries = [{
          messageId: 'message-1',
          recipient: 'Dr NHS Online',
          lastMessageDateTime: '2020-01-01T13:37:00.137Z',
          unreadReplyInfo: {
            present: true,
            count: 1,
          },
          content: 'This is less than 64 chars',
          sentDateTime: '2020-01-01T13:37:00.137Z',
          sender: 'Dr Nhs Online',
          replies: [],
          outboundMessage: true,
          requiresDetailsRequest: false,
        }];
        mountPage({ hasSubject: false });
        const subtitle = wrapper.vm.getSubtitle(messageSummaries[0]);
        expect(subtitle).toBe('This is less than 64 chars');
      });

      it('will trim the subtitle is more than 64 characters and there is no subject', () => {
        const messageSummaries = [{
          messageId: 'message-1',
          recipient: 'Dr NHS Online',
          lastMessageDateTime: '2020-01-01T13:37:00.137Z',
          unreadReplyInfo: {
            present: true,
            count: 1,
          },
          content: 'This is sixty-five characters long to check this class method!!!!',
          sentDateTime: '2020-01-01T13:37:00.137Z',
          sender: 'Dr Nhs Online',
          replies: [],
          outboundMessage: true,
          requiresDetailsRequest: false,
        }];
        mountPage({ hasSubject: false });

        const subtitle = wrapper.vm.getSubtitle(messageSummaries[0]);
        expect(subtitle).toBe('This is sixty-five characters long to check this class method!!!...');
      });

      it('will set the subtitle to the most recent reply', () => {
        const reply = {
          sender: 'Patient',
          sentDateTime: '2020-01-01T13:37:00.137Z',
          isUnread: false,
          replyContent: 'This is the reply content',
          outboundMessage: true,
        };
        const messageSummaries = [{
          messageId: 'message-1',
          recipient: 'Dr NHS Online',
          lastMessageDateTime: '2020-01-01T13:37:00.137Z',
          unreadReplyInfo: {
            present: true,
            count: 1,
          },
          content: 'This is sixty-five characters long to check this class method!!!!',
          sentDateTime: '2020-01-01T13:37:00.137Z',
          sender: 'Dr Nhs Online',
          replies: [reply],
          outboundMessage: true,
          requiresDetailsRequest: false,
        }];
        mountPage({ hasSubject: false });
        isEmptyArray.mockReturnValue(false);
        last.mockImplementation(() => reply);

        const subtitle = wrapper.vm.getSubtitle(messageSummaries[0]);
        expect(subtitle).toBe('This is the reply content');
      });
    });
  });
});
