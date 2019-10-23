import Index from '@/pages/messaging/index';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { create$T, createStore, mount } from '../../helpers';

describe('messaging index', () => {
  const messageItemClass = 'nhs-app-message__item';
  const messageItemUnreadClass = 'nhs-app-message__item--unread';
  const messageSectionClass = 'nhs-app-message';
  let $store;
  let wrapper;
  let $t;

  const mountIndex = () => mount(Index, {
    $store,
    $style: {
      [messageItemClass]: messageItemClass,
      [messageItemUnreadClass]: messageItemUnreadClass,
      [messageSectionClass]: messageSectionClass,
    },
    $t,
  });

  const createSummaryMessage = ({ body, sender, unreadCount = 0 }) => {
    $store.state.messaging.senderMessages.push({
      sender,
      unreadCount,
      messages: [{
        sender,
        body,
      }],
    });
  };

  beforeEach(() => {
    $store = createStore({
      state: {
        messaging: initialState(),
      },
    });
    $t = create$T();
  });

  describe('fetch', () => {
    beforeEach(() => {
      wrapper = mountIndex();
      wrapper.vm.$options.fetch({ store: $store });
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

    it('will translate the no message text', () => {
      expect($t).toHaveBeenCalledWith('messaging.index.noMessages');
    });
  });

  describe('has messages', () => {
    let messageSection;

    beforeEach(() => {
      createSummaryMessage({ body: 'summary message 1', sender: 'Test 1' });
      createSummaryMessage({ body: 'unread summary message 2', sender: 'Test 2', unreadCount: 2 });
      createSummaryMessage({ body: 'summary message 3', sender: 'Test 3' });
      createSummaryMessage({ body: 'unread summary message 4', sender: 'Test 4', unreadCount: 4 });
      wrapper = mountIndex();
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

    it('will not translate the no message text', () => {
      expect($t).not.toHaveBeenCalledWith('messaging.index.noMessages');
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
  });
});
