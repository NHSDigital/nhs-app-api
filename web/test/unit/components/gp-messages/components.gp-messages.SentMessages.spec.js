import SentMessage from '@/components/gp-messages/SentMessage';
import { mount, createStore } from '../../helpers';

const createPropsData = () => ({
  message: {
    content: 'Test message',
    subject: 'Test',
    sentDateTime: '2019-12-09T13:56:50.377',
    outboundMessage: true,
    replies: [{
      sender: 'Test',
      replyContent: 'This is a test',
      sentDateTime: '2019-12-09T13:56:50.377',
    }],
  },
  sentIndex: 0,
  sentPrefixIdentifier: 'initial',
  messageContent: 'Test message',
});

const $store = () => (
  createStore({
    state: {
      device: { isNativeApp: true },
      gpMessages: {
        selectedMessageDetails: {
          messageDetails: {
            content: 'Test message',
            subject: 'Test',
            sentDateTime: '2019-12-09T13:56:50.377',
            replies: [{
              sender: 'Test',
              replyContent: 'This is a test',
              sentDateTime: '2019-12-09T13:56:50.377',
            }],
          },
        },
        loadedDetails: true,
      },
    },
  }));

describe('Sent Message', () => {
  let wrapper;

  describe('Message with replies', () => {
    beforeEach(() => {
      wrapper = mount(SentMessage, {
        $store: $store(),
        propsData: createPropsData(),
      });
    });

    it('will display subject', () => {
      const subject = wrapper.find('#initialMessageSubject0').text();
      expect(subject.trim()).toBe('Test');
    });

    it('will display time', () => {
      const dateTime = wrapper.find('time').text();
      const dateTimeSingleLine = dateTime.replace(/\n|\r/g, '').replace(/  +/g, ' ');
      expect(dateTimeSingleLine.trim()).toBe('Sent 9 December 2019 at 1:56pm');
    });

    it('will display message content', () => {
      const content = wrapper.find('#initialMessageSentPanel0>p').text();
      expect(content).toBe('Test message');
    });
  });
});
