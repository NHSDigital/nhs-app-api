import DeleteSuccess from '@/pages/messages/gp-messages/delete-success';
import { create$T, createStore, mount } from '../../helpers';
import { isFalsy, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('patient messaging delete success', () => {
  let wrapper;
  let store;
  let redirect;
  let $t;

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
    $t = create$T();

    wrapper = mount(DeleteSuccess, {
      $store: store,
      $t,
    });
  };

  beforeEach(() => {
    redirect = jest.fn();
  });

  describe('fetch deleted', () => {
    let messageDeleted;
    describe('gp messages message is not deleted', () => {
      beforeEach(async () => {
        messageDeleted = false;
        isFalsy.mockImplementation(messageDeleted).mockReturnValue(true);

        mountPage({ messageDeleted });
        await wrapper.vm.$options.fetch({ store, redirect });
      });

      it('will redirect to home', () => {
        expect(redirect).toHaveBeenCalledWith('/');
      });
    });
  });
  describe('data', () => {
    it('will return the correct messages path', () => {
      expect(wrapper.vm.messagesPath).toBe('/messages/gp-messages');
    });
  });
  describe('success page messages link clicked', () => {
    beforeEach(() => {
      mountPage();
    });

    it('will redirect to message list page', () => {
      wrapper.vm.goToMessagesClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages');
    });
  });
});
