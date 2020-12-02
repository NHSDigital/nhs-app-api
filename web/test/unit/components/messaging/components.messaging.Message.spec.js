import mockdate from 'mockdate';
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
    mockdate.set('2020-12-18T14:15:12.356Z');
    $store = createStore();
    wrapper = mountMessage({
      body: 'Test1\nnew Line\nregards',
      sentTime: '2020-12-14T14:15:12.356Z',
      read: false,
      id: '0000-0001',
    });
  });

  it('will replace message content new lines with `<br/>`', () => {
    expect(wrapper.find('p').html()).toBe('<p class="panel-content">Test1<br>new Line<br>regards</p>');
  });

  describe('sent time', () => {
    let sentTime;

    beforeEach(() => {
      sentTime = wrapper.find('time');
    });

    it('will format sent time to `Sent DD MMMM YYYY at h:mma` london time', () => {
      expect(sentTime.text()).toBe('Sent 14 December 2020 at 2:15pm');
    });

    it('will set datetime attribute to `YYYY-MM-DD h:mma` london time', () => {
      expect(sentTime.attributes('datetime')).toBe('2020-12-14 2:15pm');
    });
  });

  describe('load', () => {
    it('will dispatch `messaging/markAsRead` with selected message id ', () => {
      expect($store.dispatch).toBeCalledWith('messaging/markAsRead', '0000-0001');
    });

    beforeEach(() => {
      wrapper = mountMessage({
        body: 'Test1\nnew Line\nregards',
        sentTime: '2020-12-14T14:15:12.356Z',
        read: true,
        id: '0000-0001',
      });
    });

    it('will not dispatch `messaging/markAsRead` when message is read', () => {
      expect($store.dispatch).not.toBeCalledWith('messaging/markAsRead');
    });
  });

  afterEach(() => {
    mockdate.reset();
  });
});
