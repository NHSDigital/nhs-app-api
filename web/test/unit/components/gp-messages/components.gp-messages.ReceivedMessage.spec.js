import ReceivedMessage from '@/components/gp-messages/ReceivedMessage';
import { mount, createStore } from '../../helpers';

const $store = () => (
  createStore({
    state: {
      device: { isNativeApp: true },
      gpMessages: {
        selectedMessageDetails: {
          messageDetails: {
            replies: [{
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

describe('Received Message', () => {
  let wrapper;

  describe('Message with replies', () => {
    beforeEach(() => {
      const propsData = {
        message: {
          sender: 'Test',
          content: 'This is a test',
          sentDateTime: '2019-12-09T13:56:50.377',
          isUnread: false,
        },
        replyIndex: 0,
        replyPrefixIdentifier: 'initial',
        messageContent: 'This is a test',
      };
      wrapper = mount(ReceivedMessage, {
        propsData,
        $store: $store(),
      });
    });

    it('will display read sender', () => {
      const sender = wrapper.find('#initialMessageReplySender0').text();
      expect(sender).toBe('Test');
    });

    it('will display read message time', () => {
      const dateTime = wrapper.find('time').text();
      expect(dateTime.trim()).toBe('Sent 9 December 2019 at 1:56pm');
    });

    it('will display read message content', () => {
      const content = wrapper.find('#initialMessageReplyPanel0 > p').text();
      expect(content).toBe('This is a test');
    });
  });
});
