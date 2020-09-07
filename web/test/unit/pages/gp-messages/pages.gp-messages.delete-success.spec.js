import DeleteSuccess from '@/pages/messages/gp-messages/delete-success';
import { redirectTo } from '@/lib/utils';
import { INDEX_PATH } from '@/router/paths';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

describe('patient messaging delete success', () => {
  let wrapper;
  let store;

  const mountPage = ({
    deleteEnabled = true,
    messageDeleted = true } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
          messageDeleted,
        },
        device: { isNativeApp: false },
      },
      getters: {
        'serviceJourneyRules/deleteGpMessagesEnabled': deleteEnabled,
      },
    });

    wrapper = mount(DeleteSuccess, { $store: store });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('created', () => {
    describe('gp messages message is not deleted', () => {
      it('will redirect to home', () => {
        mountPage({ messageDeleted: false });
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
      });
    });
    describe('gp messages message is deleted', () => {
      it('will not redirect to home', () => {
        mountPage({ messageDeleted: true });
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });
  });

  describe('data', () => {
    it('will return the correct messages path', () => {
      expect(wrapper.vm.messagesPath).toBe('messages/gp-messages');
    });
  });
  describe('success page messages link clicked', () => {
    beforeEach(() => {
      mountPage();
    });

    it('will redirect to message list page', () => {
      wrapper.vm.goToMessagesClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
    });
  });
});
