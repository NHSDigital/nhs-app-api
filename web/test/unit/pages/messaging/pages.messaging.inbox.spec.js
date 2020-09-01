import i18n from '@/plugins/i18n';
import Index from '@/pages/messages/app-messaging/index';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { MESSAGES_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import { createStore, mount } from '../../helpers';

dependency.redirectTo = jest.fn();

describe('messaging index', () => {
  const messageItemClass = 'nhs-app-message__item';
  const messageItemUnreadClass = 'nhs-app-message__item--unread';
  const messageSectionClass = 'nhs-app-message';
  const noMessagesSelector = '#noMessages';
  let $store;
  let wrapper;

  const mountIndex = () => mount(Index, {
    $store,
    $style: {
      [messageItemClass]: messageItemClass,
      [messageItemUnreadClass]: messageItemUnreadClass,
      [messageSectionClass]: messageSectionClass,
    },
    mountOpts: { i18n },
  });

  const createSummaryMessage = ({ body, sender, unreadCount = 0, sentTime = '2019-09-14T02:15:12.356Z' }) => {
    $store.state.messaging.senderMessages.push({
      sender,
      unreadCount,
      messages: [{
        sender,
        body,
        sentTime,
      }],
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

  describe('fetch', () => {
    beforeEach(async () => {
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
    });

    it('will dispatch `messaging/load`', () => {
      expect($store.dispatch).toBeCalledWith('messaging/load');
    });
  });

  describe('has no messages', () => {
    beforeEach(() => {
      wrapper = mountIndex();
    });

    it('will not show messages section', () => {
      expect(wrapper.find(`.${messageSectionClass}`).exists()).toBe(false);
    });

    it('will display the no messages text', () => {
      const noMessages = wrapper.find(noMessagesSelector);
      expect(noMessages.exists()).toBe(true);
      expect(noMessages.text()).toBe('You have no messages');
    });
  });

  describe('has messages', () => {
    let messageSection;

    beforeEach(async () => {
      createSummaryMessage({ body: 'summary message 1', sender: 'Test 1' });
      createSummaryMessage({ body: 'unread summary message 2', sender: 'Test 2', unreadCount: 2 });
      createSummaryMessage({ body: 'summary message 3', sender: 'Test 3' });
      createSummaryMessage({ body: 'unread summary message 4', sender: 'Test 4', unreadCount: 4 });
      wrapper = mountIndex();
      await wrapper.vm.$nextTick();
      messageSection = wrapper.find(`.${messageSectionClass}`);
    });

    it('will show messages section', () => {
      expect(messageSection.exists()).toBe(true);
    });

    it('will show all messages', () => {
      const messages = messageSection.findAll(`.${messageItemClass}`);
      expect(messages.length).toBe(4);
      expect(messages.at(0).text()).toContain('summary message 1');
      expect(messages.at(1).text()).toContain('unread summary message 2');
      expect(messages.at(2).text()).toContain('summary message 3');
      expect(messages.at(3).text()).toContain('unread summary message 4');
    });

    it('will not display the no messages text', () => {
      const noMessages = wrapper.find(noMessagesSelector);
      expect(noMessages.exists()).toBe(false);
    });

    describe('unread messages', () => {
      let unreadMessages;

      beforeEach(() => {
        unreadMessages = messageSection.findAll(`.${messageItemUnreadClass}`);
      });

      it('will have the appropriate class', () => {
        expect(unreadMessages.length).toBe(2);
        expect(unreadMessages.at(0).text()).toContain('unread summary message 2');
        expect(unreadMessages.at(1).text()).toContain('unread summary message 4');
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

      backLink = wrapper.find('[data-purpose=back-link]');
    });

    it('backlink will be shown', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('backlink will redirect to messages hub', () => {
      backLink.find('a').trigger('click');
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, MESSAGES_PATH);
    });
  });
});
