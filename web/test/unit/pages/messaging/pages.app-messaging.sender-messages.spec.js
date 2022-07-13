import mockdate from 'mockdate';
import * as dependency from '@/lib/utils';
import SenderMessages from '@/pages/messages/app-messaging/sender-messages';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { HEALTH_INFORMATION_UPDATES_PATH } from '@/router/paths';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { createStore, mount } from '../../helpers';

dependency.redirectTo = jest.fn();

const messageItemClass = 'nhs-app-message__item';
const messageSectionClass = 'nhs-app-message';
const messageUnreadIndicatorClass = 'nhs-app-message__meta';
const messageBodyUnreadClass = 'nhs-app-message__summary--unread';
const messageDateUnreadClass = 'nhs-app-message__date--unread';

const shutterContainer = '[data-purpose="shutter-container"]';

const defaultMessages = [
  { body: 'Message 1', read: true, sentTime: '2020-01-14T16:00:00.000Z', sender: 'Test Sender' },
  { body: 'Message 2', read: true, sentTime: '2020-01-14T12:00:00.000Z', sender: 'Test Sender' },
  { body: 'Unread Message 3', read: false, sentTime: '2020-01-14T00:00:00.000Z', sender: 'Test Sender' },
  { body: 'Message 4', read: true, sentTime: '2020-01-13T15:00:00.000Z', sender: 'Test Sender' },
  { body: 'Unread Message 5', read: false, sentTime: '2020-01-09T17:00:00.000Z', sender: 'Test Sender' },
];

let wrapper;
let $store;

const mountSenderMessages = ({
  sender = null,
  senderId = null,
  messages = defaultMessages,
  errored = false,
  senderIdEnabled = true,
  isNativeApp = true,
} = {}) => {
  const messaging = initialState();
  messaging.senderMessages = [{ messages }];
  messaging.error = errored;

  $store = createStore({
    state: {
      messaging,
      device: { isNativeApp },
    },
    $env: {
      MESSAGES_SENDER_ID_ENABLED: senderIdEnabled,
    },
  });

  wrapper = mount(SenderMessages, {
    $route: {
      query: senderIdEnabled
        ? { senderId }
        : { sender },
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
};

describe('messaging sender messages', () => {
  beforeEach(() => {
    mockdate.set('2020-01-14T17:00:00.000Z');
    dependency.redirectTo.mockClear();
  });

  describe('sender id not enabled', () => {
    describe('mounted', () => {
      describe('no query param provided', () => {
        beforeEach(() => {
          mountSenderMessages({ senderIdEnabled: false });
        });

        it('will dispatch messaging/clear', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/clear');
        });

        it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('incorrect query param provided', () => {
        beforeEach(() => {
          mountSenderMessages({ senderId: 'test-sender', senderIdEnabled: false });
        });

        it('will dispatch messaging/clear', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/clear');
        });

        it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });

        it('will not load messages', () => {
          expect($store.dispatch).not.toHaveBeenCalledWith('messaging/load', expect.anything);
        });
      });

      describe('correct query param provided', () => {
        beforeEach(() => {
          mountSenderMessages({ sender: 'Test Sender', senderIdEnabled: false });
        });

        it('will dispatch messaging/clear', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/clear');
        });

        it('will not redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });

        it('will load messages', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/load', { sender: 'Test Sender' });
        });
      });
    });

    describe('load messages has completed', () => {
      describe('and there has been an error', () => {
        beforeEach(() => {
          mountSenderMessages({ sender: 'Test Sender', senderIdEnabled: false, errored: true });
          $store.state.messaging.senderMessagesLoaded = true;
        });

        it('will not redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('and there are no messages', () => {
        beforeEach(() => {
          mountSenderMessages({ sender: 'Test Sender', senderIdEnabled: false, messages: [] });
          $store.state.messaging.senderMessagesLoaded = true;
        });

        it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('and there are no messages and has errored', () => {
        beforeEach(() => {
          mountSenderMessages({ sender: 'Test Sender', senderIdEnabled: false, errored: true, messages: [] });
          $store.state.messaging.senderMessagesLoaded = true;
        });

        it('will not redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('and there has been no error and messages have loaded', () => {
        describe('inbox', () => {
          let messageItems;

          beforeEach(() => {
            mountSenderMessages({ sender: 'Test Sender', senderIdEnabled: false });
            $store.state.messaging.senderMessagesLoaded = true;
            messageItems = wrapper.findAll(`.${messageItemClass}`);
          });

          it('will not show the shutter container', () => {
            expect(wrapper.find(shutterContainer).exists()).toBe(false);
          });

          it('will display all messages', () => {
            expect(messageItems.length).toBe(5);
            expect(messageItems.at(0).text()).toContain('Message 1');
            expect(messageItems.at(1).text()).toContain('Message 2');
            expect(messageItems.at(2).text()).toContain('Unread Message 3');
            expect(messageItems.at(3).text()).toContain('Message 4');
            expect(messageItems.at(4).text()).toContain('Unread Message 5');
          });

          describe.each([0, 1, 3])('read message', (messageIndex) => {
            let readMessage;

            beforeEach(() => {
              readMessage = messageItems.at(messageIndex);
            });

            it('will not style body as unread', () => {
              expect(readMessage.find(`.${messageBodyUnreadClass}`).exists()).toBe(false);
            });

            it('will not style date time as unread', () => {
              expect(readMessage.find(`.${messageDateUnreadClass}`).exists()).toBe(false);
            });

            it('will not show unread indicator', () => {
              expect(readMessage.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(false);
            });
          });

          describe.each([2, 4])('unread messages', (messageIndex) => {
            let unreadMessage;

            beforeEach(() => {
              unreadMessage = messageItems.at(messageIndex);
            });

            it('will style body as unread', () => {
              expect(unreadMessage.find(`.${messageBodyUnreadClass}`).exists()).toBe(true);
            });

            it('will style date time as unread', () => {
              expect(unreadMessage.find(`.${messageDateUnreadClass}`).exists()).toBe(true);
            });

            it('will show unread indicator', () => {
              expect(unreadMessage.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(true);
            });
          });

          describe.each([
            [0, 'today', 'Red message received at 4pm'],
            [1, 'midday', 'Red message received at Midday'],
            [2, 'midnight', 'Unread message received at Midnight'],
            [3, 'yesterday', 'Red message received  Yesterday'],
            [4, 'a few days ago', 'Unread message received on Thursday'],
          ])('aria label', (messageIndex, time, description) => {
            it(`will describe ${time} correctly`, () => {
              expect(messageItems.at(messageIndex).find('a').attributes('aria-label')).toContain(description);
            });
          });
        });

        describe.each([
          ['will not be shown on native', true],
          ['will be shown on desktop', false],
        ])('desktop back link', (description, isNativeApp) => {
          let desktopBackLink;

          beforeEach(() => {
            mountSenderMessages({ sender: 'Test Sender', senderIdEnabled: false, isNativeApp });
            $store.state.messaging.senderMessagesLoaded = true;
            desktopBackLink = wrapper.find(DesktopGenericBackLink);
          });

          it(description, () => {
            expect(desktopBackLink.exists()).toBe(!isNativeApp);
          });
        });
      });
    });
  });

  describe('sender id enabled', () => {
    describe('mounted', () => {
      describe('no query param provided', () => {
        beforeEach(() => {
          mountSenderMessages();
        });

        it('will dispatch messaging/clear', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/clear');
        });

        it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('incorrect query param provided', () => {
        beforeEach(() => {
          mountSenderMessages({ sender: 'Test Sender' });
        });

        it('will dispatch messaging/clear', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/clear');
        });

        it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });

        it('will not load messages', () => {
          expect($store.dispatch).not.toHaveBeenCalledWith('messaging/load', expect.anything);
        });
      });

      describe('correct query param provided', () => {
        beforeEach(() => {
          mountSenderMessages({ senderId: 'test-sender' });
        });

        it('will dispatch messaging/clear', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/clear');
        });

        it('will not redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });

        it('will load messages', () => {
          expect($store.dispatch).toHaveBeenCalledWith('messaging/load', { senderId: 'test-sender' });
        });
      });
    });

    describe('load messages has completed', () => {
      describe('and there has been an error', () => {
        beforeEach(() => {
          mountSenderMessages({ senderId: 'test-sender', errored: true });
          $store.state.messaging.senderMessagesLoaded = true;
        });

        it('will not redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('and there are no messages', () => {
        beforeEach(() => {
          mountSenderMessages({ senderId: 'test-sender', messages: [] });
          $store.state.messaging.senderMessagesLoaded = true;
        });

        it('will redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('and there are no messages and has errored', () => {
        beforeEach(() => {
          mountSenderMessages({ senderId: 'test-sender', errored: true, messages: [] });
          $store.state.messaging.senderMessagesLoaded = true;
        });

        it('will not redirect to HEALTH_INFORMATION_UPDATES_PATH', () => {
          expect(dependency.redirectTo).not.toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });

      describe('and there has been no error and messages have loaded', () => {
        describe('inbox', () => {
          let messageItems;

          beforeEach(() => {
            mountSenderMessages({ senderId: 'test-sender' });
            $store.state.messaging.senderMessagesLoaded = true;
            messageItems = wrapper.findAll(`.${messageItemClass}`);
          });

          it('will not show the shutter container', () => {
            expect(wrapper.find(shutterContainer).exists()).toBe(false);
          });

          it('will display all messages', () => {
            expect(messageItems.length).toBe(5);
            expect(messageItems.at(0).text()).toContain('Message 1');
            expect(messageItems.at(1).text()).toContain('Message 2');
            expect(messageItems.at(2).text()).toContain('Unread Message 3');
            expect(messageItems.at(3).text()).toContain('Message 4');
            expect(messageItems.at(4).text()).toContain('Unread Message 5');
          });

          describe.each([0, 1, 3])('read message', (messageIndex) => {
            let readMessage;

            beforeEach(() => {
              readMessage = messageItems.at(messageIndex);
            });

            it('will not style body as unread', () => {
              expect(readMessage.find(`.${messageBodyUnreadClass}`).exists()).toBe(false);
            });

            it('will not style date time as unread', () => {
              expect(readMessage.find(`.${messageDateUnreadClass}`).exists()).toBe(false);
            });

            it('will not show unread indicator', () => {
              expect(readMessage.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(false);
            });
          });

          describe.each([2, 4])('unread messages', (messageIndex) => {
            let unreadMessage;

            beforeEach(() => {
              unreadMessage = messageItems.at(messageIndex);
            });

            it('will style body as unread', () => {
              expect(unreadMessage.find(`.${messageBodyUnreadClass}`).exists()).toBe(true);
            });

            it('will style date time as unread', () => {
              expect(unreadMessage.find(`.${messageDateUnreadClass}`).exists()).toBe(true);
            });

            it('will show unread indicator', () => {
              expect(unreadMessage.find(`.${messageUnreadIndicatorClass}`).exists()).toBe(true);
            });
          });

          describe.each([
            [0, 'today', 'Red message received at 4pm'],
            [1, 'midday', 'Red message received at Midday'],
            [2, 'midnight', 'Unread message received at Midnight'],
            [3, 'yesterday', 'Red message received  Yesterday'],
            [4, 'a few days ago', 'Unread message received on Thursday'],
          ])('aria label', (messageIndex, time, description) => {
            it(`will describe ${time} correctly`, () => {
              expect(messageItems.at(messageIndex).find('a').attributes('aria-label')).toContain(description);
            });
          });
        });

        describe.each([
          ['will not be shown on native', true],
          ['will be shown on desktop', false],
        ])('desktop back link', (description, isNativeApp) => {
          let desktopBackLink;

          beforeEach(() => {
            mountSenderMessages({ senderId: 'test-sender', isNativeApp });
            $store.state.messaging.senderMessagesLoaded = true;
            desktopBackLink = wrapper.find(DesktopGenericBackLink);
          });

          it(description, () => {
            expect(desktopBackLink.exists()).toBe(!isNativeApp);
          });
        });
      });
    });
  });
});
