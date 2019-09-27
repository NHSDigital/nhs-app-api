import Messages from '@/pages/messaging/messages';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { createStore, mount } from '../../helpers';

describe('messaging messages', () => {
  const pageDividerClass = 'page-divider';
  const panelItemClass = 'nhsuk-panel-group__item';
  const readSectionId = 'readSection';
  const unreadSectionId = 'unreadSection';
  let $store;
  let wrapper;

  const mountMessages = () => mount(Messages, {
    $store,
    $style: {
      [pageDividerClass]: pageDividerClass,
      [panelItemClass]: panelItemClass,
    },
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        messaging: initialState(),
      },
    });
  });

  describe('has no messages', () => {
    beforeEach(() => {
      wrapper = mountMessages();
    });

    it('will not show read section', () => {
      expect(wrapper.find(`#${readSectionId}`).exists()).toBe(false);
    });

    it('will not show unread page divider', () => {
      expect(wrapper.find(`.${pageDividerClass}`).exists()).toBe(false);
    });

    it('will not show unread section', () => {
      expect(wrapper.find(`#${unreadSectionId}`).exists()).toBe(false);
    });
  });

  describe('has read messages', () => {
    let readSection;

    beforeEach(() => {
      $store.state.messaging.readMessages = [
        { body: 'read message 1', sender: 'Test 1', sentTime: '2019-09-14T02:15:12.356Z' },
        { body: 'read message 2', sender: 'Test 2', sentTime: '2019-09-14T02:16:12.356Z' },
        { body: 'read message 3', sender: 'Test 3', sentTime: '2019-09-14T02:17:12.356Z' },
      ];
      wrapper = mountMessages();
      readSection = wrapper.find(`#${readSectionId}`);
    });

    it('will show read section', () => {
      expect(readSection.exists()).toBe(true);
    });

    it('will show all read messages', () => {
      const readMessages = readSection.findAll(`.${panelItemClass}`);
      expect(readMessages.length).toBe(3);
      expect(readMessages.at(0).text()).toContain('read message 1');
      expect(readMessages.at(1).text()).toContain('read message 2');
      expect(readMessages.at(2).text()).toContain('read message 3');
    });

    it('will not show unread page divider', () => {
      expect(wrapper.find(`.${pageDividerClass}`).exists()).toBe(false);
    });

    it('will not show unread section', () => {
      expect(wrapper.find(`#${unreadSectionId}`).exists()).toBe(false);
    });
  });

  describe('has unread messages', () => {
    let unreadSection;

    beforeEach(() => {
      $store.state.messaging.unreadMessages = [
        { body: 'unread message 1', sender: 'Test 1', sentTime: '2019-09-14T02:15:12.356Z' },
        { body: 'unread message 2', sender: 'Test 2', sentTime: '2019-09-14T02:16:12.356Z' },
        { body: 'unread message 3', sender: 'Test 3', sentTime: '2019-09-14T02:17:12.356Z' },
      ];
      wrapper = mountMessages();
      unreadSection = wrapper.find(`#${unreadSectionId}`);
    });

    it('will not show read section', () => {
      expect(wrapper.find(`#${readSectionId}`).exists()).toBe(false);
    });

    it('will show unread page divider', () => {
      expect(wrapper.find(`.${pageDividerClass}`).exists()).toBe(true);
    });

    it('will show unread section', () => {
      expect(unreadSection.exists()).toBe(true);
    });

    it('will show all unread messages', () => {
      const unreadMessages = unreadSection.findAll(`.${panelItemClass}`);
      expect(unreadMessages.length).toBe(3);
      expect(unreadMessages.at(0).text()).toContain('unread message 1');
      expect(unreadMessages.at(1).text()).toContain('unread message 2');
      expect(unreadMessages.at(2).text()).toContain('unread message 3');
    });
  });

  describe('has both messages', () => {
    let unreadSection;
    let readSection;

    beforeEach(() => {
      $store.state.messaging.readMessages = [
        { body: 'read message 1', sender: 'Test 1', sentTime: '2019-09-14T02:15:12.356Z' },
        { body: 'read message 2', sender: 'Test 2', sentTime: '2019-09-14T02:16:12.356Z' },
        { body: 'read message 3', sender: 'Test 3', sentTime: '2019-09-14T02:17:12.356Z' },
      ];
      $store.state.messaging.unreadMessages = [
        { body: 'unread message 1', sender: 'Test 1', sentTime: '2019-09-14T02:18:12.356Z' },
        { body: 'unread message 2', sender: 'Test 2', sentTime: '2019-09-14T02:19:12.356Z' },
        { body: 'unread message 3', sender: 'Test 3', sentTime: '2019-09-14T02:20:12.356Z' },
      ];
      wrapper = mountMessages();
      readSection = wrapper.find(`#${readSectionId}`);
      unreadSection = wrapper.find(`#${unreadSectionId}`);
    });

    it('will show read section', () => {
      expect(readSection.exists()).toBe(true);
    });

    it('will show all read messages', () => {
      const readMessages = readSection.findAll(`.${panelItemClass}`);
      expect(readMessages.length).toBe(3);
      expect(readMessages.at(0).text()).toContain('read message 1');
      expect(readMessages.at(1).text()).toContain('read message 2');
      expect(readMessages.at(2).text()).toContain('read message 3');
    });

    it('will show unread page divider', () => {
      expect(wrapper.find(`.${pageDividerClass}`).exists()).toBe(true);
    });

    it('will show unread section', () => {
      expect(unreadSection.exists()).toBe(true);
    });

    it('will show all unread messages', () => {
      const unreadMessages = unreadSection.findAll(`.${panelItemClass}`);
      expect(unreadMessages.length).toBe(3);
      expect(unreadMessages.at(0).text()).toContain('unread message 1');
      expect(unreadMessages.at(1).text()).toContain('unread message 2');
      expect(unreadMessages.at(2).text()).toContain('unread message 3');
    });
  });
});
