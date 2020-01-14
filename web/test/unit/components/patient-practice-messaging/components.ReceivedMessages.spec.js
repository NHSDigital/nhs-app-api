import ReceivedMessages from '@/components/patient-practice-messaging/ReceivedMessages';
import { mount, createStore } from '../../helpers';

const $store = () => (
  createStore({
    state: {
      device: { isNativeApp: true },
      patientPracticeMessaging: {
        selectedMessageDetails: {
          messageDetails: {
            messageReplies: [{
              sender: 'Test',
              replyContent: 'This is a test',
              sentDateTime: '2019-12-09T13:56:50.377',
              isUnread: false,
            },
            {
              sender: 'Test',
              replyContent: 'This is a test',
              sentDateTime: '2019-12-09T13:56:50.377',
              isUnread: true,
            },
            {
              sender: 'Test',
              replyContent: 'This is a test',
              sentDateTime: '2019-12-09T13:56:50.377',
              isUnread: true,
            }],
          },
        },
        loadedDetails: true,
      },
    },
  }));

describe('Received Messages', () => {
  let wrapper;

  describe('Message with replies', () => {
    beforeEach(() => {
      wrapper = mount(ReceivedMessages, {
        $store: $store(),
      });
    });

    it('will display read sender', () => {
      const sender = wrapper.find('#readMessageReplySender0').element.firstChild.data;
      expect(sender).toBe('Test');
    });

    it('will display unread sender', () => {
      const sender = wrapper.find('#unreadMessageReplySender0').element.firstChild.data;
      expect(sender).toBe('Test');
    });

    it('will display read message time', () => {
      const dateTime = wrapper.find('#readMessageReplyDateTime0').element.firstChild.data;
      const dateTimeSingleLine = dateTime.replace(/\n|\r/g, '').replace(/  +/g, ' ');
      expect(dateTimeSingleLine.trim()).toBe('Sent 09 December 2019 at 1:56pm');
    });

    it('will display unread message time', () => {
      const dateTime = wrapper.find('#unreadMessageReplyDateTime0').element.firstChild.data;
      const dateTimeSingleLine = dateTime.replace(/\n|\r/g, '').replace(/  +/g, ' ');
      expect(dateTimeSingleLine.trim()).toBe('Sent 09 December 2019 at 1:56pm');
    });

    it('will display read message content', () => {
      const content = wrapper.find('#readMessageReplyPanel0>p').element.firstChild.data;
      expect(content).toBe('This is a test');
    });

    it('will display unread message content', () => {
      const content = wrapper.find('#unreadMessageReplyPanel0>p').element.firstChild.data;
      expect(content).toBe('This is a test');
    });
  });

  describe('Computed properties', () => {
    beforeEach(() => {
      wrapper = mount(ReceivedMessages, {
        $store: $store(),
      });
    });

    describe('getReplies', () => {
      it('should return the list of replies', () => {
        const replies = [{
          sender: 'Test',
          replyContent: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: false,
        },
        {
          sender: 'Test',
          replyContent: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: true,
        },
        {
          sender: 'Test',
          replyContent: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: true,
        }];

        expect(wrapper.vm.getReplies).toEqual(replies);
      });
    });

    describe('unReadReplies', () => {
      it('should return the list of unread replies', () => {
        const unReadReplies = [{
          sender: 'Test',
          replyContent: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: true,
        },
        {
          sender: 'Test',
          replyContent: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: true,
        }];

        expect(wrapper.vm.unreadMessages).toEqual(unReadReplies);
      });
    });

    describe('readReplies', () => {
      it('should return the list of read replies', () => {
        const readReplies = [{
          sender: 'Test',
          replyContent: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: false,
        }];

        expect(wrapper.vm.readMessages).toEqual(readReplies);
      });
    });
  });
});
