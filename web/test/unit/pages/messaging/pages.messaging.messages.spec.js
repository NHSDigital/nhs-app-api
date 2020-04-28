import Messages from '@/pages/messages/app-messaging/app-message';
import { initialState } from '@/store/modules/messaging/mutation-types';
import { HEALTH_INFORMATION_UPDATES } from '@/lib/routes';
import { createStore, mount } from '../../helpers';

describe('messaging messages', () => {
  const pageDividerClass = 'page-divider';
  const panelItemClass = 'message-panel__item';
  const readSectionId = 'readSection';
  const unreadSectionId = 'unreadSection';
  const sender = 'test sender';
  let $store;
  let storeRedirect;
  let wrapper;

  const mountMessages = () => mount(Messages, {
    $store,
    $style: {
      [pageDividerClass]: pageDividerClass,
      [panelItemClass]: panelItemClass,
    },
  });

  const createSenderMessage = (messages) => {
    $store.state.messaging.senderMessages = [{
      sender,
      messages,
    }];
  };

  beforeEach(() => {
    storeRedirect = jest.fn();
    $store = createStore({
      context: {
        redirect: storeRedirect,
      },
      state: {
        messaging: initialState(),
      },
    });
  });

  describe('fetch', () => {
    let redirect;

    const fetch = ({ selectedSender = '' } = {}) => {
      redirect = jest.fn();
      $store.state.messaging.selectedSender = selectedSender;
      wrapper.vm.$options.fetch({ redirect, store: $store });
    };

    beforeEach(() => {
      wrapper = mountMessages();
    });

    describe('has selected sender', () => {
      const selectedSender = 'test1';

      beforeEach(() => {
        fetch({ selectedSender });
      });

      it('will dispatch `messaging/load` with selected sender', () => {
        expect($store.dispatch).toBeCalledWith('messaging/load', selectedSender);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });

    describe('has no selected sender', () => {
      beforeEach(() => {
        redirect = jest.fn();
        fetch();
      });

      it('will not dispatch', () => {
        expect($store.dispatch).not.toBeCalled();
      });

      it('will redirect to `HEALTH_INFORMATION_UPDATES`', () => {
        expect(redirect).toBeCalledWith(HEALTH_INFORMATION_UPDATES.path);
      });
    });
  });

  describe('created', () => {
    beforeEach(() => {
      process.server = true;
    });

    describe('has no messages', () => {
      beforeEach(() => {
        wrapper = mountMessages();
      });

      it('will redirect to `HEALTH_INFORMATION_UPDATES`', () => {
        expect(storeRedirect).toBeCalledWith(HEALTH_INFORMATION_UPDATES.path);
      });
    });

    describe('has messages', () => {
      beforeEach(() => {
        createSenderMessage([
          { body: 'read message 1', read: true },
        ]);

        wrapper = mountMessages();
      });

      it('will not redirect', () => {
        expect(storeRedirect).not.toBeCalled();
      });
    });
  });

  describe('has only read messages', () => {
    let readSection;

    beforeEach(() => {
      createSenderMessage([
        { body: 'read message 1', read: true },
        { body: 'read message 2', read: true },
        { body: 'read message 3', read: true },
      ]);
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

  describe('has a read message before unread, shows as only unread messages', () => {
    let unreadSection;

    beforeEach(() => {
      createSenderMessage([
        { body: 'unread message 1', read: false },
        { body: 'unread message 2', read: false },
        { body: 'read message 4', read: true },
        { body: 'unread message 3', read: false },
      ]);
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
      expect(unreadMessages.length).toBe(4);
      expect(unreadMessages.at(0).text()).toContain('unread message 1');
      expect(unreadMessages.at(1).text()).toContain('unread message 2');
      expect(unreadMessages.at(2).text()).toContain('read message 4');
      expect(unreadMessages.at(3).text()).toContain('unread message 3');
    });
  });

  describe('has read and unread messages', () => {
    let unreadSection;
    let readSection;

    beforeEach(() => {
      createSenderMessage([
        { body: 'read message 1', read: true },
        { body: 'read message 2', read: true },
        { body: 'read message 3', read: true },
        { body: 'unread message 1', read: false },
        { body: 'unread message 2', read: false },
        { body: 'read message 4', read: true },
        { body: 'unread message 3', read: false },
      ]);
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
      expect(unreadMessages.length).toBe(4);
      expect(unreadMessages.at(0).text()).toContain('unread message 1');
      expect(unreadMessages.at(1).text()).toContain('unread message 2');
      expect(unreadMessages.at(2).text()).toContain('read message 4');
      expect(unreadMessages.at(3).text()).toContain('unread message 3');
    });
  });
});
