import RecipientsPage from '@/pages/messages/gp-messages/recipients';
import { mount, createStore } from '../../helpers';
import { redirectTo, isEmptyArray } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('gp messages recipients page', () => {
  let wrapper;
  let store;
  let redirect;

  const mountPage = ({
    isNativeApp = true,
    messageRecipients = [],
  } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
          messageRecipients,
          loadedRecipients: true,
        },
        device: { isNativeApp },
      },
    });
    wrapper = mount(RecipientsPage, {
      $store: store,
    });
  };

  describe('fetch', () => {
    beforeEach(() => {
      redirect = jest.fn();
    });

    describe('no message recipients', () => {
      it('will redirect to the inbox', async () => {
        isEmptyArray.mockReturnValueOnce(true);
        mountPage();

        await wrapper.vm.$options.fetch({ store, redirect });

        expect(isEmptyArray).toHaveBeenCalledWith([]);
        expect(redirect).toHaveBeenCalledWith('/messages/gp-messages');
      });
    });

    describe('has message recipients', () => {
      it('will not redirect to home', async () => {
        const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];
        isEmptyArray.mockReturnValueOnce(false);
        mountPage({ messageRecipients });

        await wrapper.vm.$options.fetch({ store, redirect });

        expect(isEmptyArray).toHaveBeenCalledWith(messageRecipients);
        expect(redirect).not.toHaveBeenCalledWith('/messages/gp-messages');
      });
    });
  });

  describe('template', () => {
    describe('back link clicked', () => {
      it('will redirect to urgency question', () => {
        mountPage({ isNativeApp: false });

        wrapper.vm.backLinkClicked();

        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages/urgency');
      });
    });

    describe('has message recipients', () => {
      it('will show a list of recipients', () => {
        const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];

        mountPage({ messageRecipients });

        expect(wrapper.find('#recipientsMenuList').exists()).toBe(true);
        expect(store.dispatch).not.toHaveBeenCalledWith('header/updateHeaderText');
      });
    });
  });
});
