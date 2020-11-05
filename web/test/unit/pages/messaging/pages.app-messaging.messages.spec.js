import Messages from '@/pages/messages/app-messaging/app-message';
import { HEALTH_INFORMATION_UPDATES_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

describe('messaging messages', () => {
  const pageDividerClass = 'page-divider';
  const panelItemClass = 'message-panel__item';
  const readSectionId = 'readSection';
  const unreadSectionId = 'unreadSection';
  const testSender = 'test sender';

  let wrapper;
  let $store;

  const mountMessages = ({
    sender,
    senderMessages = [],
    isNativeApp = true,
  } = {}) => {
    $store = createStore({
      state: {
        messaging: {
          senderMessages,
          selectedSender: sender,
          hasUnread: false,
        },
        device: {
          isNativeApp,
        },
      },
    });
    return mount(Messages, {
      $store,
      $style: {
        [pageDividerClass]: pageDividerClass,
        [panelItemClass]: panelItemClass,
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('created', () => {
    describe('has selected sender', () => {
      beforeEach(async () => {
        wrapper = mountMessages({
          sender: testSender,
          senderMessages: [{
            sender: testSender,
            messages: [{ body: 'read message 1', read: true }],
          }],
        });
        await wrapper.vm.$nextTick();
      });

      it('will dispatch `messaging/load` with selected sender', () => {
        expect($store.dispatch).toBeCalledWith('messaging/load', { sender: testSender });
      });

      it('will not redirect', () => {
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });

    describe('has no selected sender', () => {
      beforeEach(async () => {
        wrapper = mountMessages();
        await wrapper.vm.$nextTick();
      });

      it('will not dispatch', () => {
        expect($store.dispatch).not.toBeCalled();
      });

      it('will redirect to `HEALTH_INFORMATION_UPDATES`', () => {
        expect(redirectTo).toBeCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
      });
    });

    describe('back link', () => {
      let backLink;

      describe('native', () => {
        beforeEach(async () => {
          wrapper = mountMessages({
            sender: testSender,
            senderMessages: [{
              sender: testSender,
              messages: [{ body: 'read message 1', read: true }],
            }],
          });
          await wrapper.vm.$nextTick();
          backLink = wrapper.find('[data-purpose=back-link]');
        });

        it('will not be shown', () => {
          expect(backLink.exists()).toBe(false);
        });
      });

      describe('desktop', () => {
        beforeEach(async () => {
          wrapper = mountMessages({
            sender: testSender,
            senderMessages: [{
              sender: testSender,
              messages: [{ body: 'read message 1', read: true }],
            }],
            isNativeApp: false,
          });
          await wrapper.vm.$nextTick();
          backLink = wrapper.find('[data-purpose=back-link]');
        });

        it('will be shown', () => {
          expect(backLink.exists()).toBe(true);
        });

        it('backlink will redirect to app messages', () => {
          backLink.find('a').trigger('click');
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
        });
      });
    });
  });

  describe('created', () => {
    describe('has no messages', () => {
      beforeEach(async () => {
        wrapper = mountMessages({ sender: testSender });
        await wrapper.vm.$nextTick();
      });

      it('will redirect to `HEALTH_INFORMATION_UPDATES`', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
      });
    });

    describe('has messages', () => {
      beforeEach(async () => {
        wrapper = mountMessages({
          sender: testSender,
          senderMessages: [{
            sender: testSender,
            messages: [{ body: 'read message 1', read: true }],
          }],
        });
        await wrapper.vm.$nextTick();
      });

      it('will not redirect', () => {
        expect(redirectTo).not.toBeCalled();
      });
    });
  });

  describe('has only read messages', () => {
    let readSection;

    beforeEach(async () => {
      wrapper = mountMessages({
        sender: testSender,
        senderMessages: [{
          sender: testSender,
          messages: [
            { body: 'read message 1', read: true },
            { body: 'read message 2', read: true },
            { body: 'read message 3', read: true },
          ],
        }],
      });
      await wrapper.vm.$nextTick();
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

    beforeEach(async () => {
      wrapper = mountMessages({
        sender: testSender,
        senderMessages: [{
          sender: testSender,
          messages: [
            { body: 'unread message 1', read: false },
            { body: 'unread message 2', read: false },
            { body: 'read message 4', read: true },
            { body: 'unread message 3', read: false },
          ],
        }],
      });
      await wrapper.vm.$nextTick();
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

    beforeEach(async () => {
      wrapper = mountMessages({
        sender: testSender,
        senderMessages: [{
          sender: testSender,
          messages: [
            { body: 'read message 1', read: true },
            { body: 'read message 2', read: true },
            { body: 'read message 3', read: true },
            { body: 'unread message 1', read: false },
            { body: 'unread message 2', read: false },
            { body: 'read message 4', read: true },
            { body: 'unread message 3', read: false },
          ],
        }],
      });
      await wrapper.vm.$nextTick();
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
