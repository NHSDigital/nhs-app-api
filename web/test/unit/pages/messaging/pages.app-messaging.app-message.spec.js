import Message from '@/pages/messages/app-messaging/app-message';
import { HEALTH_INFORMATION_UPDATES_PATH, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH } from '@/router/paths';
import * as utils from '@/lib/utils';
import { createStore, mount, normaliseNewLines } from '../../helpers';

utils.redirectTo = jest.fn();

const panelItemClass = 'message-panel__item';
const messageReplyContainerClass = 'messageReply';
const errorPanelId = 'form-error-summary';

let wrapper;
let $store;

const mountMessage = ({
  messageId,
  message,
  isNativeApp = true,
  error = false,
} = {}) => {
  $store = createStore({
    state: {
      messaging: { error, message },
      device: { isNativeApp },
    },
  });

  return mount(Message, {
    $route: {
      query: {
        messageId,
      },
    },
    $store,
    $style: {
      [panelItemClass]: panelItemClass,
      [messageReplyContainerClass]: messageReplyContainerClass,
    },
    mocks: {
      reload: jest.fn(),
    },
  });
};

describe('messaging message', () => {
  beforeEach(() => {
    utils.redirectTo.mockClear();
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
        expect(utils.redirectTo).toBeCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
      });

      it('will not dispatch `messaging/loadMessage`', () => {
        expect($store.dispatch).not.toBeCalledWith('messaging/loadMessage', expect.anything);
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

        it('will not redirect', () => {
          expect(utils.redirectTo).not.toHaveBeenCalled();
        });

        it('will show the shutter container', () => {
          expect(wrapper.find('[data-purpose="shutter-container"]').exists()).toBe(true);
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
          expect(utils.redirectTo).toBeCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('has message', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: { id: messageId, body: 'read message 1', sender: 'test sender', version: 0, read: true, sentTime: '2019-09-14T02:15:12.356Z' },
          });
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/clear`', () => {
          expect($store.dispatch).toBeCalledWith('messaging/clear');
        });

        it('will dispatch `messaging/loadMessage` with selected messsageId', () => {
          expect($store.dispatch).toBeCalledWith('messaging/loadMessage', { messageId: '1234' });
        });

        it('will not redirect', () => {
          expect(utils.redirectTo).not.toHaveBeenCalled();
        });

        it('will show the message section', () => {
          expect(wrapper.find(`.${panelItemClass}`).exists()).toBe(true);
        });

        it('will not show the shutter container', () => {
          expect(wrapper.find('[data-purpose="shutter-container"]').exists()).toBe(false);
        });

        it('will not display the message reply container', () => {
          expect(wrapper.find(`.${messageReplyContainerClass}`).exists()).toBe(false);
        });
      });

      describe('has message with single keyword reply option', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: {
              id: messageId,
              body: 'read message 1',
              sender: 'test sender',
              version: 0,
              read: true,
              sentTime: '2019-09-14T02:15:12.356Z',
              reply: {
                options: [{
                  code: 'CANCEL',
                  display: 'CANCEL',
                }],
              },
            },
          });
          await wrapper.vm.$nextTick();
        });

        it('will display the message reply container', () => {
          expect(wrapper.find(`.${messageReplyContainerClass}`).exists()).toBe(true);
        });

        it('will dispatch action to save message response and reload message', async () => {
          await wrapper.vm.sendClicked('CANCEL', true, false);
          expect($store.dispatch).toBeCalledWith('messaging/recordMessageResponse', { messageId: '1234', response: 'CANCEL' });
          await wrapper.vm.$nextTick();
          expect($store.dispatch).toBeCalledWith('messaging/loadMessage', { messageId: '1234' });
        });
        it('will show checkbox error message when checkbox is empty', async () => {
          await wrapper.vm.sendClicked('', false, true);
          expect(wrapper.find(`#${errorPanelId}`).text()).toContain('Select the option if you want to reply to this message');
        });
      });

      describe('has message with multiple keyword reply options', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: {
              id: messageId,
              body: 'read message 1',
              sender: 'test sender',
              version: 0,
              read: true,
              sentTime: '2019-09-14T02:15:12.356Z',
              reply: {
                options: [{
                  code: 'YES',
                  display: 'YES',
                },
                {
                  code: 'NO',
                  display: 'NO',
                },
                {
                  code: 'NEVER',
                  display: 'NEVER',
                }],
              },
            },
          });
          await wrapper.vm.$nextTick();
        });

        it('will show radio button error message when radio button is empty', async () => {
          await wrapper.vm.sendClicked('', false, false);
          expect(wrapper.find(`#${errorPanelId}`).text()).toContain('Select an option if you want to reply to this message');
        });
      });

      describe('has message with keyword replies', () => {
        beforeEach(async () => {
          wrapper = mountMessage({
            messageId,
            message: {
              body: 'read message 1',
              sender: 'test sender',
              version: 0,
              read: true,
              sentTime: '2019-09-14T02:15:12.356Z',
              reply: {
                options: [{
                  code: 'CANCEL',
                  display: 'CANCEL',
                }],
              },
            },
          });
          await wrapper.vm.$nextTick();
        });

        it('will display the message reply container', () => {
          expect(wrapper.find(`.${messageReplyContainerClass}`).exists()).toBe(true);
        });
      });
    });
  });

  describe('watchers', () => {
    describe('isUnread', () => {
      describe('unread message', () => {
        beforeEach(async () => {
          wrapper = mountMessage({ messageId: '1234' });
          $store.state.messaging.message = { id: '1234', body: 'read message 1', sender: 'test sender', sentTime: '2019-09-14T02:15:12.356Z', read: false };
          await wrapper.vm.$nextTick();
        });

        it('will dispatch `messaging/markAsRead` with message id', () => {
          expect($store.dispatch).toBeCalledWith('messaging/markAsRead', '1234');
        });
      });

      describe('read message', () => {
        beforeEach(async () => {
          wrapper = mountMessage({ messageId: '1234' });
          $store.state.messaging.message = { id: '1234', body: 'read message 1', sender: 'test sender', sentTime: '2019-09-14T02:15:12.356Z', read: true };
          await wrapper.vm.$nextTick();
        });

        it('will not dispatch `messaging/markAsRead`', () => {
          expect($store.dispatch).not.toBeCalledWith('messaging/markAsRead', '1234');
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
          message: { body: 'read message 1', senderId: 'test-sender', sender: 'test sender', read: true, sentTime: '2019-09-14T02:15:12.356Z' },
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
        expect(utils.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH, { senderId: 'test-sender' });
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
