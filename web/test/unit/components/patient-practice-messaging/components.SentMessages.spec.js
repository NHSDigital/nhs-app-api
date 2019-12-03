import SentMessage from '@/components/patient-practice-messaging/SentMessage';
import { mount, createStore } from '../../helpers';

const createPropsData = () => ({
});

const $store = () => (
  createStore({
    state: {
      device: { isNativeApp: true },
      patientPracticeMessaging: {
        selectedMessageDetails: {
          messageDetails: {
            content: 'Test message',
            subject: 'Test',
            sentDateTime: '2019-12-09T13:56:50.377',
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
      const subject = wrapper.find('#messageSubject').element.firstChild.data;
      expect(subject.trim()).toBe('Test');
    });

    it('will display time', () => {
      const dateTime = wrapper.find('#messageSentDateTime').element.firstChild.data;
      const dateTimeSingleLine = dateTime.replace(/\n|\r/g, '').replace(/  +/g, ' ');
      expect(dateTimeSingleLine.trim()).toBe('Sent 09 December 2019 at 1:56pm');
    });

    it('will display message content', () => {
      const content = wrapper.find('#messageSentPanel>p').element.firstChild.data;
      expect(content).toBe('Test message');
    });
  });
});
