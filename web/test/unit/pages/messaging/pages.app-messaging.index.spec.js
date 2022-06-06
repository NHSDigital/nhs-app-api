import each from 'jest-each';
import Index from '@/pages/messages/app-messaging/index';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { MESSAGES_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import { createStore, mount } from '../../helpers';

dependency.redirectTo = jest.fn();

describe('messaging index', () => {
  const messageItemClass = 'nhs-app-message__item';
  const messageItemUnreadClass = 'nhs-app-message__item--unread';
  const senderSectionClass = 'nhs-app-message';
  const noMessagesSelector = '#noMessages';
  let $store;
  let wrapper;

  const mountIndex = () => mount(Index, {
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

  const createSender = ({ name, unreadCount = 0 }) => {
    $store.state.messaging.senders.push({
      name,
      unreadCount,
    });
  };

  beforeEach(() => {
    $store = createStore({
      state: {
        messaging: initialState(),
        device: {
          isNativeApp: true,
        },
      },
    });
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
      $store.state.messaging.error = true;
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
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
      wrapper = mountIndex();
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
  });

  describe('has senders', () => {
    let senderSection;

    beforeEach(async () => {
      createSender({ name: 'Test 1' });
      createSender({ name: 'Test 2', unreadCount: 2 });
      createSender({ name: 'Test 3' });
      createSender({ name: 'Test 4', unreadCount: 4 });
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

      expect(messages.at(0).text()).toContain('Test 1');
      expect(messages.at(1).text()).toContain('Test 2');
      expect(messages.at(2).text()).toContain('Test 3');
      expect(messages.at(3).text()).toContain('Test 4');
    });

    it('will not display the no messages text', () => {
      expect(wrapper.find(noMessagesSelector).exists()).toBe(false);
    });

    describe('senders with unread messages', () => {
      let unreadSenders;

      beforeEach(() => {
        unreadSenders = senderSection.findAll(`.${messageItemUnreadClass}`);
      });

      it('will have the appropriate class', () => {
        expect(unreadSenders.length).toBe(2);

        expect(unreadSenders.at(0).text()).toContain('Test 2');
        expect(unreadSenders.at(1).text()).toContain('Test 4');
      });
    });

    describe('back link', () => {
      it('will not be shown', () => {
        expect(wrapper.find('[data-purpose=back-link]').exists()).toBe(false);
      });
    });
  });

  describe('desktop', () => {
    let backLink;
    beforeEach(async () => {
      $store.state.device.isNativeApp = false;
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
      dependency.redirectTo = jest.fn();

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
      createSender({ name: 'Test sender' });

      wrapper = mountIndex();
      await wrapper.vm.$nextTick();

      const ariaLabel = wrapper.find(`.${senderSectionClass}>.${messageItemClass}>a`).attributes('aria-label');

      expect(ariaLabel).toEqual('Messages from: Test sender. ');
    });

    each([[1, ''], [5, 's']])
      .it('will include an appropriately pluralised unread count when some messages are unread', async (count, pluralisation) => {
        createSender({ name: 'Test sender', unreadCount: count });
        const expected = 'Messages from: Test sender. ' +
          `You have ${count} unread message${pluralisation}. `;

        wrapper = mountIndex();
        await wrapper.vm.$nextTick();

        const ariaLabel = wrapper.find(`.${senderSectionClass}>.${messageItemClass}>a`).attributes('aria-label');

        expect(ariaLabel).toEqual(expected);
      });
  });
});
