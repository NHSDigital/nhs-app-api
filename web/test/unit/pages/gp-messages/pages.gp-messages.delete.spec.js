import Delete from '@/pages/messages/gp-messages/delete';
import { createStore, mount } from '../../helpers';
import { redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils', () => {
  const { isBlankString } = jest.requireActual('@/lib/utils');

  // we want to mock redirectTo but NOT isBlankString
  return {
    isBlankString,
    redirectTo: jest.fn(),
  };
});


describe('patient messaging delete', () => {
  let wrapper;
  let store;
  let redirect;

  const mountPage = ({
    deleteEnabled = true,
    selectedId = undefined,
    messageDeleted = false,
    currentRoutePath = '/messages/gp-messages/view-details',
  } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
          selectedMessageId: selectedId,
          messageDeleted,
        },
        device: { isNativeApp: false },
      },
      getters: {
        'serviceJourneyRules/deleteGpMessagesEnabled': deleteEnabled,
      },
      router: {
        currentRoute: { path: currentRoutePath },
      },
    });
    wrapper = mount(Delete, {
      $store: store,
    });
  };

  describe('fetch', () => {
    beforeEach(() => {
      redirect = jest.fn();
    });

    describe('selected message id is undefined', () => {
      it('will redirect to gp messages', async () => {
        mountPage();
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).toHaveBeenCalledWith('/messages/gp-messages');
      });
    });

    describe('current route is not gp messages view details', () => {
      it('will redirect to gp messages', async () => {
        mountPage({ currentRoutePath: '/more' });
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).toHaveBeenCalledWith('/messages/gp-messages');
      });
    });

    describe('current route is view details and selected message id is defined', () => {
      it('will not redirect', async () => {
        mountPage({ selectedId: '1' });
        await wrapper.vm.$options.fetch({ store, redirect });
        expect(redirect).not.toHaveBeenCalled();
      });
    });
  });

  describe('computed and data', () => {
    beforeEach(() => {
      mountPage({ selectedId: '1' });
    });

    it('will return the correct messages path', () => {
      expect(wrapper.vm.messagesPath).toBe('/messages/gp-messages');
    });

    it('will return the correct message details path', () => {
      expect(wrapper.vm.messageDetailsPath).toBe('/messages/gp-messages/view-details');
    });

    it('will return the correct message id', () => {
      expect(wrapper.vm.messageID).toBe('1');
    });

    it('will return the correct delete button text reference', () => {
      expect(wrapper.vm.buttonText).toBe('gp_messages.delete.backButtonText.text');
    });
  });

  describe('back link clicked', () => {
    it('will redirect to message details page', () => {
      mountPage({ isNativeApp: false, selectedId: '1' });
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages/view-details');
    });
  });

  describe('delete clicked', () => {
    beforeEach(async () => {
      mountPage({ isNativeApp: false, selectedId: '1', messageDeleted: true });
      await wrapper.vm.deleteButtonClicked();
    });

    it('will call delete', () => {
      expect(store.dispatch).toHaveBeenCalledWith('gpMessages/deleteMessage', '1');
    });

    it('will call redirect', () => {
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages/delete-success');
    });
  });
});
