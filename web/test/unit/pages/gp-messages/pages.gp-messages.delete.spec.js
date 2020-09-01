import Delete from '@/pages/messages/gp-messages/delete';
import i18n from '@/plugins/i18n';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../../helpers';

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
      mountOpts: { i18n },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('created', () => {
    describe('selected message id is undefined', () => {
      it('will redirect to gp messages', () => {
        mountPage();
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
      });
    });

    describe('selected message id is not undefined', () => {
      it('will not redirect to gp messages', () => {
        mountPage({ selectedId: '1' });
        expect(redirectTo).not.toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
      });
    });
  });

  describe('beforeRouteEnter', () => {
    describe('coming from gp messages view details', () => {
      it('will not redirect to gp messages', () => {
        const next = jest.fn();
        Delete.beforeRouteEnter(undefined, { path: 'messages/gp-messages/view-details' }, next);
        expect(next).toHaveBeenCalledWith(undefined);
      });
    });

    describe('not coming from gp messages view details', () => {
      it('will redirect to gp messages', () => {
        const next = jest.fn();
        Delete.beforeRouteEnter(undefined, { path: '/somewhere-not-gp-messages' }, next);
        expect(next).toHaveBeenCalledWith('messages/gp-messages');
      });
    });
  });

  describe('computed and data', () => {
    beforeEach(() => {
      mountPage({ selectedId: '1' });
    });

    it('will return the correct messages path', () => {
      expect(wrapper.vm.messagesPath).toBe('messages/gp-messages');
    });

    it('will return the correct message details path', () => {
      expect(wrapper.vm.messageDetailsPath).toBe('messages/gp-messages/view-details');
    });

    it('will return the correct message id', () => {
      expect(wrapper.vm.messageID).toBe('1');
    });

    it('will return the correct delete button text reference', () => {
      expect(wrapper.text()).toContain('Delete conversation');
    });
  });

  describe('back link clicked', () => {
    it('will redirect to message details page', () => {
      mountPage({ isNativeApp: false, selectedId: '1' });
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/view-details');
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
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/delete-success');
    });
  });
});
