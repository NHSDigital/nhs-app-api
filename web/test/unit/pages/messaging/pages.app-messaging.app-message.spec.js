import Message from '@/pages/messages/app-messaging/app-message';
import { HEALTH_INFORMATION_UPDATES_PATH, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount, normaliseNewLines } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

describe('messaging message', () => {
  const panelItemClass = 'message-panel__item';

  let wrapper;
  let $store;

  const mountMessage = ({
    messageId,
    message,
    isNativeApp = true,
    error = false,
  } = {}) => {
    $store.state = {
      messaging: { error, message },
      device: { isNativeApp },
    };
    return mount(Message, {
      $route: {
        query: {
          messageId,
        },
      },
      $store,
      $style: {
        [panelItemClass]: panelItemClass,
      },
      mocks: {
        reload: jest.fn(),
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
    $store = createStore();
  });

  describe('created', () => {
    describe('has no message ID', () => {
      beforeEach(async () => {
        wrapper = mountMessage();
        await wrapper.vm.$nextTick();
      });

      it('will dispatch `messaging/clear`', () => {
        expect($store.dispatch).toBeCalledWith('messaging/clear');
      });

      it('will redirect to `HEALTH_INFORMATION_UPDATES`', () => {
        expect(redirectTo).toBeCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
      });

      it('will not dispatch `messaging/loadMessage`', () => {
        expect($store.dispatch).not.toBeCalledWith('messaging/loadMessage', expect.any(Object));
      });
    });

    describe('has message ID', () => {
      const messageId = '1234';

      describe('fail to load message', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: undefined,
            error: true,
          });
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/clear`', () => {
          expect($store.dispatch).toBeCalledWith('messaging/clear');
        });

        it('will dispatch `messaging/loadMessage` with selected messsageId', () => {
          expect($store.dispatch).toBeCalledWith('messaging/loadMessage', { messageId });
        });

        it('will not dispatch `messaging/selectSender`', () => {
          expect($store.dispatch).not.toBeCalledWith('messaging/selectSender', expect.any(Object));
        });

        it('will not redirect', () => {
          expect(redirectTo).not.toHaveBeenCalled();
        });

        it('will show the error container', () => {
          expect(wrapper.find('[data-purpose="error-container"]').exists()).toBe(true);
        });

        it('will not show the message section', () => {
          expect(wrapper.find(`.${panelItemClass}`).exists()).toBe(false);
        });
      });

      describe('no message', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: undefined,
          });
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/clear`', () => {
          expect($store.dispatch).toBeCalledWith('messaging/clear');
        });

        it('will dispatch `messaging/loadMessage` with selected messsageId', () => {
          expect($store.dispatch).toBeCalledWith('messaging/loadMessage', { messageId });
        });

        it('will redirect to `HEALTH_INFORMATION_UPDATES`', () => {
          expect(redirectTo).toBeCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('has message', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: { body: 'read message 1', sender: 'test sender', version: 0, read: true, sentTime: '2019-09-14T02:15:12.356Z' },
          });
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/clear`', () => {
          expect($store.dispatch).toBeCalledWith('messaging/clear');
        });

        it('will dispatch `messaging/loadMessage` with selected messsageId', () => {
          expect($store.dispatch).toBeCalledWith('messaging/loadMessage', { messageId: '1234' });
        });

        it('will dispatch `messaging/selectSender` with sender', () => {
          expect($store.dispatch).not.toBeCalledWith('messaging/selectSender', { sender: 'test sender' });
        });

        it('will not redirect', () => {
          expect(redirectTo).not.toHaveBeenCalled();
        });

        it('will show the message section', () => {
          expect(wrapper.find(`.${panelItemClass}`).exists()).toBe(true);
        });

        it('will not show the error container', () => {
          expect(wrapper.find('[data-purpose="error-container"]').exists()).toBe(false);
        });
      });
    });
  });

  describe('watchers', () => {
    const messageId = '1234';
    let message;

    beforeEach(() => {
      message = { id: messageId, body: 'read message 1', sender: 'test sender', sentTime: '2019-09-14T02:15:12.356Z' };

      $store.dispatch = jest.fn((action) => {
        if (action === 'messaging/loadMessage') {
          $store.state.messaging.message = message;
        }
      });
    });

    describe('isUnread', () => {
      describe('unread message', () => {
        beforeEach(async () => {
          message.read = false;

          wrapper = mountMessage({ messageId });
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/markAsRead` with message id', () => {
          expect($store.dispatch).toBeCalledWith('messaging/markAsRead', messageId);
        });
      });

      describe('read message', () => {
        beforeEach(async () => {
          message.read = true;

          wrapper = mountMessage({ messageId });
          await wrapper.vm.$nextTick();
        });

        it('will not dispatch `messaging/markAsRead`', () => {
          expect($store.dispatch).not.toBeCalledWith('messaging/markAsRead', messageId);
        });
      });
    });

    describe('sender', () => {
      describe('failed to load message', () => {
        beforeEach(async () => {
          message = null;

          wrapper = mountMessage({ messageId, error: true });
          await wrapper.vm.$nextTick();
        });

        it('will not dispatch `messaging/selectSender`', () => {
          expect($store.dispatch).not.toBeCalledWith('messaging/selectSender', expect.any(String));
        });
      });

      describe('message loaded', () => {
        beforeEach(async () => {
          wrapper = mountMessage({ messageId });
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/selectSender` with sender name', () => {
          expect($store.dispatch).toBeCalledWith('messaging/selectSender', message.sender);
        });
      });
    });
  });

  describe('back link', () => {
    let backLink;

    describe('native', () => {
      beforeEach(async () => {
        wrapper = mountMessage({
          messageId: '1234',
          message: { body: 'read message 1', sender: 'test sender', version: 0, read: true, sentTime: '2019-09-14T02:15:12.356Z' },
          isNativeApp: true,
        });
        await wrapper.vm.$nextTick();
        backLink = wrapper.find('[data-purpose=back-link]');
      });

      it('will not show', () => {
        expect(backLink.exists()).toBe(false);
      });
    });

    describe('desktop', () => {
      beforeEach(async () => {
        wrapper = mountMessage({
          messageId: '1234',
          message: { body: 'read message 1', sender: 'test sender', read: true, sentTime: '2019-09-14T02:15:12.356Z' },
          isNativeApp: false,
        });
        await wrapper.vm.$nextTick();
        backLink = wrapper.find('[data-purpose=back-link]');
      });

      it('will show', () => {
        expect(backLink.exists()).toBe(true);
      });

      it('will redirect to sender messages when clicked', () => {
        backLink.find('a').trigger('click');
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH, { sender: 'test sender' });
      });
    });
  });

  describe.each([
    ['linkify', 0, '<p class="panel-content">**Bold** <br> <a href="http://test.com" target="_blank">http://test.com</a></p>'],
    ['markown', 1, '<div class="panel-content"><p><strong>Bold</strong>http://test.com</p></div>'],
  ])('%s message', (_, version, content) => {
    let message;

    beforeEach(async () => {
      wrapper = mountMessage({
        messageId: '1234',
        message: { id: '1234', sender: 'test sender', body: '**Bold** \n http://test.com', version, read: true, sentTime: '2019-09-14T02:15:12.356Z' },
      });

      await wrapper.vm.$nextTick();
      message = wrapper.find(`.${panelItemClass}`);
    });

    it('will show', () => {
      expect(message.exists()).toBe(true);
    });

    it('will render content', () => {
      expect(normaliseNewLines(message.html())).toContain(content);
    });
  });
});
