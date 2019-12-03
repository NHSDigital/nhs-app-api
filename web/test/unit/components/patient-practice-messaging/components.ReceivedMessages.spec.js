import ReceivedMessages from '@/components/patient-practice-messaging/RecievedMessages';
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

    it('will display sender', () => {
      const sender = wrapper.find('#messageReplySender0').element.firstChild.data;
      expect(sender).toBe('Test');
    });

    it('will display time', () => {
      const dateTime = wrapper.find('#messageReplyDateTime0').element.firstChild.data;
      expect(dateTime).toBe('\n' +
        '        Sent 09 December 2019\n' +
        '        at 1:56pm\n' +
        '      ');
    });

    it('will display message content', () => {
      const content = wrapper.find('#messageReplyPanel0>p').element.firstChild.data;
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
        }];

        expect(wrapper.vm.getReplies).toEqual(replies);
      });
    });
  });
});
