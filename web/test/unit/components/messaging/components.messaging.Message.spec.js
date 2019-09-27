import Message from '@/components/messaging/Message';
import { mount } from '../../helpers';

describe('message', () => {
  const mountMessage = ({ body, sentTime } = {}) => mount(Message, {
    propsData: {
      message: {
        body,
        sender: 'Test sender',
        sentTime,
      },
    },
  });
  let wrapper;

  beforeEach(() => {
    wrapper = mountMessage({ body: 'Test1\nnew Line\nregards', sentTime: '2019-09-14T02:15:12.356Z' });
  });

  it('will replace message content new lines with `<br/>`', () => {
    expect(wrapper.find('p').html()).toBe('<p>Test1<br>new Line<br>regards</p>');
  });

  describe('sent time', () => {
    let sentTime;

    beforeEach(() => {
      sentTime = wrapper.find('time');
    });

    it('will format sent time to `h:mma, DD MMMM YYYY` london time', () => {
      expect(sentTime.text()).toBe('Sent 3:15am, 14 September 2019');
    });

    it('will set datetime attribute to `YYYY-MM-DD h:mma` london time', () => {
      expect(sentTime.attributes('datetime')).toBe('2019-09-14 3:15am');
    });
  });
});
