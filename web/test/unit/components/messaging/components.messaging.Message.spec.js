import mockdate from 'mockdate';
import Message from '@/components/messaging/Message';
import { createStore, mount, normaliseNewLines } from '../../helpers';

describe('message', () => {
  let $store;
  let wrapper;

  const mountMessage = ({
    body = 'Lorem ipsum \n dolor sit amet',
    id = '0000-0001',
    sentTime = '2020-12-14T14:15:12.356Z',
    read = false,
    version = 1,
  } = {}) => mount(Message, {
    $store,
    propsData: {
      message: {
        body,
        id,
        sender: 'Test sender',
        sentTime,
        read,
        version,
      },
    },
  });

  beforeEach(() => {
    mockdate.set('2020-12-18T14:15:12.356Z');
    $store = createStore();
    wrapper = mountMessage();
  });

  describe('message version', () => {
    let content;
    describe('Linkify', () => {
      beforeEach(() => {
        wrapper = mountMessage({
          version: 0,
        });
        content = normaliseNewLines(wrapper.find('.panel-content').html());
      });

      it('will render content in a `<p>` tag including `<br>` tags for new lines', () => {
        expect(content).toBe('<p class="panel-content">Lorem ipsum <br> dolor sit amet</p>');
      });
    });

    describe('Markdown', () => {
      beforeEach(() => {
        wrapper = mountMessage({
          body: 'Lorem ipsum \n dolor sit amet',
          version: 1,
        });
        content = normaliseNewLines(wrapper.find('.panel-content').html());
      });

      it('will wrap content in a `<div>` then a `<p>` tag', () => {
        expect(content).toBe('<div class="panel-content"><p>Lorem ipsum dolor sit amet</p></div>');
      });
    });
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
        read: true,
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
