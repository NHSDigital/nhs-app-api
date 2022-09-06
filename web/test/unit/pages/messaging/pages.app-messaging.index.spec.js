import each from 'jest-each';
import Index from '@/pages/messages/app-messaging/index';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { MESSAGES_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import { createStore, mount } from '../../helpers';

dependency.redirectTo = jest.fn();

const messageItemClass = 'nhs-app-message__item';
const messageItemUnreadClass = 'nhs-app-message__item--unread';
const senderSectionClass = 'nhs-app-message';
const noMessagesSelector = '#noMessages';
const senderAndMessageCountSelector = '#unreadMessageAndSenderHeading';
let $store;
let wrapper;

const defaultSenders = [
  { id: 'test-1', name: 'Test Sender 1', unreadCount: 0 },
  { id: 'test-2', name: 'Test Sender 2', unreadCount: 2 },
  { id: 'test-3', name: 'Test Sender 3', unreadCount: 0 },
  { id: 'test-4', name: 'Test Sender 4', unreadCount: 4 },
];

const mountIndex = ({
  messagingError = false,
  senders = defaultSenders,
  isNativeApp = true,
} = {}) => {
  const messagingState = initialState();
  messagingState.error = messagingError;
  messagingState.senders = senders;
  messagingState.totalUnreadSendersCount = senders.filter(sender => sender.unreadCount > 0).length;
  messagingState.totalUnreadMessageCount = senders.reduce((total, sender) => (sender.unreadCount > 0 ? total + sender.unreadCount : total), 0);

  $store = createStore({
    state: {
      messaging: messagingState,
      device: {
        isNativeApp,
      },
    },
  });

  return mount(Index, {
    $store,
    $style: {
      [messageItemClass]: messageItemClass,
      [messageItemUnreadClass]: messageItemUnreadClass,
      [senderSectionClass]: senderSectionClass,
    },
    mocks: {
      reload: jest.fn(),
    },
  });
};

describe('messaging index', () => {
  beforeEach(() => {
    dependency.redirectTo.mockClear();
  });

  describe('created', () => {
    beforeEach(async () => {
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
    });

    it('will dispatch messaging/clear', () => {
      expect($store.dispatch).toBeCalledWith('messaging/clear');
    });

    it('will dispatch `messaging/loadSenders`', () => {
      expect($store.dispatch).toBeCalledWith('messaging/loadSenders');
    });
  });

  describe('failed to load senders', () => {
    beforeEach(async () => {
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
      $store.state.messaging.error = true;
    });

    it('will show the shutter container', () => {
      expect(wrapper.find('[data-purpose="shutter-container"]').exists()).toBe(true);
    });

    it('will not show senders section', () => {
      expect(wrapper.find(`.${senderSectionClass}`).exists()).toBe(false);
    });

    it('will not display the no messages text', () => {
      expect(wrapper.find(noMessagesSelector).exists()).toBe(false);
    });
  });

  describe('has no senders', () => {
    beforeEach(async () => {
      wrapper = mountIndex({ senders: [] });
      await wrapper.vm.$nextTick();
    });

    it('will not show senders section', () => {
      expect(wrapper.find(`.${senderSectionClass}`).exists()).toBe(false);
    });

    it('will display the no messages text', () => {
      const noMessages = wrapper.find(noMessagesSelector);
      expect(noMessages.exists()).toBe(true);
      expect(noMessages.text()).toBe('You have no messages.');
    });

    it('will not display unread message count and sender count', () => {
      expect(wrapper.find(senderAndMessageCountSelector).exists()).toBe(false);
    });
  });

  describe('has senders', () => {
    let senderSection;

    beforeEach(async () => {
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
      senderSection = wrapper.find(`.${senderSectionClass}`);
    });

    it('will show senders section', () => {
      expect(senderSection.exists()).toBe(true);
    });

    it('will show all senders', () => {
      const messages = senderSection.findAll(`.${messageItemClass}`);
      expect(messages.length).toBe(4);

      expect(messages.at(0).text()).toContain('Test Sender 1');
      expect(messages.at(1).text()).toContain('Test Sender 2');
      expect(messages.at(2).text()).toContain('Test Sender 3');
      expect(messages.at(3).text()).toContain('Test Sender 4');
    });

    it('will not display the no messages text', () => {
      expect(wrapper.find(noMessagesSelector).exists()).toBe(false);
    });

    beforeEach(async () => {
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
      senderSection = wrapper.find(`.${senderSectionClass}`);
    });

    it('will pass the sender id in the query parameters to the sender messages page', () => {
      const messageLink = senderSection.find(`.${messageItemClass} a`);
      messageLink.trigger('click');

      expect(dependency.redirectTo).toBeCalledWith(
        wrapper.vm,
        'messages/app-messaging/sender-messages',
        { senderId: 'test-1' },
      );
      expect(messageLink.attributes().href).toBe('messages/app-messaging/sender-messages?senderId=test-1');
    });

    describe('senders with unread messages', () => {
      let unreadSenders;

      beforeEach(() => {
        unreadSenders = senderSection.findAll(`.${messageItemUnreadClass}`);
      });

      it('will have the appropriate class', () => {
        expect(unreadSenders.length).toBe(2);

        expect(unreadSenders.at(0).text()).toContain('Test Sender 2');
        expect(unreadSenders.at(1).text()).toContain('Test Sender 4');
      });
    });

    describe('back link', () => {
      it('will not be shown', () => {
        expect(wrapper.find('[data-purpose=back-link]').exists()).toBe(false);
      });
    });
  });


  describe('unread message and sender count', () => {
    it('will display with plural wording for multiple unread messages and single sender', async () => {
      wrapper = mountIndex({ senders: [{ id: 'test-1', name: 'Test Sender 1', unreadCount: 9 }] });
      await wrapper.vm.$nextTick();
      const unreadMessageAndSenderBlock = wrapper.find('#unreadMessageAndSenderHeading');
      expect(unreadMessageAndSenderBlock.text()).toBe('You have 9 unread messages from 1 sender');
    });

    it('will display with plural wording for multiple unread messages and multiple senders', async () => {
      wrapper = mountIndex({ senders: [{ id: 'test-1', name: 'Test Sender 1', unreadCount: 2 },
        { id: 'test-2', name: 'Test Sender 2', unreadCount: 12 }] });
      await wrapper.vm.$nextTick();
      const unreadMessageAndSenderBlock = wrapper.find('#unreadMessageAndSenderHeading');
      expect(unreadMessageAndSenderBlock.text()).toBe('You have 14 unread messages from 2 senders');
    });

    it('will display with singular wording for unread messages and sender', async () => {
      wrapper = mountIndex({ senders: [{ id: 'test-1', name: 'Test Sender 1', unreadCount: 1 }] });
      await wrapper.vm.$nextTick();
      const unreadMessageAndSenderBlock = wrapper.find('#unreadMessageAndSenderHeading');
      expect(unreadMessageAndSenderBlock.text()).toBe('You have 1 unread message from 1 sender');
    });
  });


  describe('desktop', () => {
    let backLink;
    beforeEach(async () => {
      wrapper = mountIndex({ isNativeApp: false });
      await wrapper.vm.$nextTick();
      dependency.redirectTo.mockClear();

      backLink = wrapper.find('[data-purpose=main-back-button]');
    });

    it('backlink will be shown', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('backlink will redirect to messages hub', () => {
      expect(backLink.attributes('href')).toBe(MESSAGES_PATH);
    });
  });

  describe('sender aria label', () => {
    it('will indicate the sender and date of the last message received', async () => {
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();

      const ariaLabel = wrapper.find(`.${senderSectionClass}>.${messageItemClass}>a`).attributes('aria-label');

      expect(ariaLabel).toEqual('Messages from: Test Sender 1. ');
    });

    each([[1, ''], [5, 's']])
      .it('will include an appropriately pluralised unread count when some messages are unread', async (count, pluralisation) => {
        const expected = `Messages from: Test Sender. You have ${count} unread message${pluralisation}. `;

        wrapper = mountIndex({ senders: [{ id: 'test-sender', name: 'Test Sender', unreadCount: count }] });
        await wrapper.vm.$nextTick();

        const ariaLabel = wrapper.find(`.${senderSectionClass}>.${messageItemClass}>a`).attributes('aria-label');

        expect(ariaLabel).toEqual(expected);
      });
  });
});
