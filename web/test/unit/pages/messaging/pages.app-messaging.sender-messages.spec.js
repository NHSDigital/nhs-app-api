import mockdate from 'mockdate';
import * as dependency from '@/lib/utils';
import SenderMessages from '@/pages/messages/app-messaging/sender-messages';
import { HEALTH_INFORMATION_UPDATES_PATH } from '@/router/paths';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { createStore, mount } from '../../helpers';

describe('messaging sender messages', () => {
  const messageItemClass = 'nhs-app-message__item';
  const messageSectionClass = 'nhs-app-message';
  const messageUnreadIndicatorClass = 'nhs-app-message__meta';
  const messageBodyUnreadClass = 'nhs-app-message__summary--unread';
  const messageDateUnreadClass = 'nhs-app-message__date--unread';
  let $store;
  let wrapper;

  const mountSenderMessages = ({ sender }) => mount(SenderMessages, {
    $route: {
      query: {
        sender,
      },
    },
    $store,
    $style: {
      [messageItemClass]: messageItemClass,
      [messageSectionClass]: messageSectionClass,
      [messageUnreadIndicatorClass]: messageUnreadIndicatorClass,
      [messageBodyUnreadClass]: messageBodyUnreadClass,
      [messageDateUnreadClass]: messageDateUnreadClass,
    },
    mocks: {
      reload: jest.fn(),
    },
  });

  const createSenderMessages = ({ messages = [] }) => {
    $store.state.messaging.senderMessages.push({
      sender: 'Test Sender',
      messages,
    });
  };

  beforeEach(() => {
    dependency.redirectTo = jest.fn();

    $store = createStore({
      state: {
        messaging: initialState(),
        device: {
          isNativeApp: true,
        },
      },
    });
  });

  describe('no query or state selected sender', () => {
    beforeEach(async () => {
      $store.state.messaging.selectedSender = null;
      wrapper = mountSenderMessages({ sender: null });
    });

    it('will dispatch messaging/clear', () => {
      expect($store.dispatch).toBeCalledWith('messaging/clear');
    });

    it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
      expect(dependency.redirectTo)
        .toBeCalledWith(expect.any(Object), HEALTH_INFORMATION_UPDATES_PATH);
    });
  });

  describe('with state selected sender', () => {
    beforeEach(async () => {
      $store.state.messaging.selectedSender = 'Test Sender';
      createSenderMessages({
        messages: [
          { body: 'Message 1', read: true, sentTime: '2020-01-14T16:00:00.000Z' },
        ],
      });
      wrapper = mountSenderMessages({ sender: null });
    });

    it('will dispatch `messaging/clear`', () => {
      expect($store.dispatch).toBeCalledWith('messaging/clear');
    });

    it('will dispatch `messaging/load`', () => {
      expect($store.dispatch).toBeCalledWith('messaging/load', { sender: 'Test Sender' });
    });

    it('will not redirect', () => {
      expect(dependency.redirectTo).not.toBeCalled();
    });
  });

  describe('with selected sender', () => {
    const sender = 'Test Sender';

    describe('fail to load messages', () => {
      beforeEach(async () => {
        $store.state.messaging.error = true;
        wrapper = mountSenderMessages({ sender });
        await wrapper.vm.$nextTick();
      });

      it('will dispatch messaging/clear', () => {
        expect($store.dispatch).toBeCalledWith('messaging/clear');
      });

      it('will dispatch messaging/load', () => {
        expect($store.dispatch).toBeCalledWith('messaging/load', { sender: 'Test Sender' });
      });

      it('will show the shutter container', () => {
        expect(wrapper.find('[data-purpose="shutter-container"]').exists()).toBe(true);
      });

      it('will not show the messages section', () => {
        expect(wrapper.find(`.${messageSectionClass}`).exists()).toBe(false);
      });
    });

    describe('no messages', () => {
      beforeEach(async () => {
        createSenderMessages({ messages: [] });
        wrapper = mountSenderMessages({ sender });
      });

      it('will dispatch messaging/clear', () => {
        expect($store.dispatch).toBeCalledWith('messaging/clear');
      });

      it('will dispatch messaging/load', () => {
        expect($store.dispatch).toBeCalledWith('messaging/load', { sender: 'Test Sender' });
      });

      it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
        expect(dependency.redirectTo)
          .toBeCalledWith(expect.any(Object), HEALTH_INFORMATION_UPDATES_PATH);
      });
    });

    describe('has messages', () => {
      beforeEach(async () => {
        mockdate.set('2020-01-14T17:00:00.000Z');
        createSenderMessages({
          messages: [
            { body: 'Message 1', read: true, sentTime: '2020-01-14T16:00:00.000Z' },
            { body: 'Message 2', read: true, sentTime: '2020-01-14T12:00:00.000Z' },
            { body: 'Unread Message 3', read: false, sentTime: '2020-01-14T00:00:00.000Z' },
            { body: 'Message 4', read: true, sentTime: '2020-01-13T15:00:00.000Z' },
            { body: 'Unread Message 5', read: false, sentTime: '2020-01-09T17:00:00.000Z' },
          ],
        });
        $store.state.device.isNativeApp = true;
        wrapper = mountSenderMessages({ sender });
        await wrapper.vm.$nextTick();
      });

      afterEach(() => {
        mockdate.reset();
      });

      it('will dispatch messaging/clear', () => {
        expect($store.dispatch).toBeCalledWith('messaging/clear');
      });

      it('will dispatch messaging/load', () => {
        expect($store.dispatch).toBeCalledWith('messaging/load', { sender: 'Test Sender' });
      });

      it('will not redirect', () => {
        expect(dependency.redirectTo).not.toBeCalled();
      });

      it('will not show the shutter container', () => {
        expect(wrapper.find('[data-purpose="shutter-container"]').exists()).not.toBe(true);
      });

      describe('back link', () => {
        const backLinkData = '[data-purpose=back-link]';

        it('will not show on native', () => {
          expect(wrapper.find(backLinkData).exists()).toBe(false);
        });

        describe('desktop', () => {
          let backLink;

          beforeEach(async () => {
            $store.state.device.isNativeApp = false;
            wrapper = mountSenderMessages({ sender });
            await wrapper.vm.$nextTick();
            backLink = wrapper.find(backLinkData);
          });

          it('will show', () => {
            expect(backLink.exists()).toBe(true);
          });

          it('will redirect HEALTH_INFORMATION_UPDATES_PATH when clicked', () => {
            backLink.find('a').trigger('click');

            expect(dependency.redirectTo)
              .toBeCalledWith(expect.any(Object), HEALTH_INFORMATION_UPDATES_PATH);
          });
        });
      });

      describe('inbox', () => {
        let messageItems;

        beforeEach(() => {
          messageItems = wrapper.findAll(`.${messageItemClass}`);
        });

        it('will display all messages', () => {
          expect(messageItems.length).toBe(5);
          expect(messageItems.at(0).text()).toContain('Message 1');
          expect(messageItems.at(1).text()).toContain('Message 2');
          expect(messageItems.at(2).text()).toContain('Unread Message 3');
          expect(messageItems.at(3).text()).toContain('Message 4');
          expect(messageItems.at(4).text()).toContain('Unread Message 5');
        });

        describe('read messages', () => {
          let readMessage1;
          let readMessage2;
          let readMessage4;

          beforeEach(() => {
            readMessage1 = messageItems.at(0);
            readMessage2 = messageItems.at(1);
            readMessage4 = messageItems.at(3);
          });

          it('will not style body as unread', () => {
            expect(readMessage1.find(`.${messageBodyUnreadClass}`).exists()).toBe(false);
            expect(readMessage2.find(`.${messageBodyUnreadClass}`).exists()).toBe(false);
            expect(readMessage4.find(`.${messageBodyUnreadClass}`).exists()).toBe(false);
          });

          it('will not style date time as unread', () => {
            expect(readMessage1.find(`.${messageDateUnreadClass}`).exists()).toBe(false);
            expect(readMessage2.find(`.${messageDateUnreadClass}`).exists()).toBe(false);
            expect(readMessage4.find(`.${messageDateUnreadClass}`).exists()).toBe(false);
          });

          it('will not show unread indicator', () => {
            expect(readMessage1.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(false);
            expect(readMessage2.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(false);
            expect(readMessage4.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(false);
          });
        });

        describe('unread messages', () => {
          let unreadMessage3;
          let unreadMessage5;

          beforeEach(() => {
            unreadMessage3 = messageItems.at(2);
            unreadMessage5 = messageItems.at(4);
          });

          it('will style body as unread', () => {
            expect(unreadMessage3.find(`.${messageBodyUnreadClass}`).exists()).toBe(true);
            expect(unreadMessage5.find(`.${messageBodyUnreadClass}`).exists()).toBe(true);
          });

          it('will style date time as unread', () => {
            expect(unreadMessage3.find(`.${messageDateUnreadClass}`).exists()).toBe(true);
            expect(unreadMessage5.find(`.${messageDateUnreadClass}`).exists()).toBe(true);
          });

          it('will show unread indicator', () => {
            expect(unreadMessage3.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(true);
            expect(unreadMessage5.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(true);
          });
        });

        describe.each([
          ['today time', 'Red', 'at', 0, '4pm'],
          ['midday time', 'Red', 'at', 1, 'Midday'],
          ['midnight time', 'Unread', 'at', 2, 'Midnight'],
          ['yesterday', 'Red', '', 3, 'Yesterday'],
          ['a few days ago', 'Unread', 'on', 4, 'Thursday'],
        ])('aria label', (description, prefix, timePrefix, messageIndex, date) => {
          it(`will display ${description} correctly`, () => {
            expect(messageItems.at(messageIndex).find('a').attributes('aria-label'))
              .toContain(`${prefix} message received ${timePrefix} ${date}.`);
          });
        });
      });
    });
  });
});
