import Message from '@/components/messaging/Message';
import { createStore, mount } from '../../helpers';

describe('message', () => {
  let $store;
  let wrapper;

  const mountMessage = ({ body, sentTime, read, id } = {}) => mount(Message, {
    $store,
    propsData: {
      message: {
        body,
        sender: 'Test sender',
        sentTime,
        read,
        id,
      },
    },
  });

  beforeEach(() => {
    $store = createStore();
    wrapper = mountMessage({
      body: 'Test1\nnew Line\nregards',
      sentTime: '2019-09-14T02:15:12.356Z',
      read: false,
      id: '0000-0001',
    });
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

  describe('load', () => {
    it('will dispatch `messaging/markAsRead` with selected message id ', () => {
      expect($store.dispatch).toBeCalledWith('messaging/markAsRead', '0000-0001');
    });

    beforeEach(() => {
      wrapper = mountMessage({
        body: 'Test1\nnew Line\nregards',
        sentTime: '2019-09-14T02:15:12.356Z',
        read: true,
        id: '0000-0001',
      });
    });

    it('will not dispatch `messaging/markAsRead` when message is read', () => {
      expect($store.dispatch).not.toBeCalledWith('messaging/markAsRead');
    });
  });
});
