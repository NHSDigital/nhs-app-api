import RecipientsPage from '@/pages/messages/gp-messages/recipients';
import { redirectTo, isEmptyArray } from '@/lib/utils';
import { mount, createStore } from '../../helpers';

jest.mock('@/lib/utils');

describe('gp messages recipients page', () => {
  let wrapper;
  let store;

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

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('created', () => {
    describe('no message recipients', () => {
      const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];

      beforeEach(() => {
        isEmptyArray.mockReturnValueOnce(true);
        mountPage({ messageRecipients });
      });

      it('will redirect to the inbox', async () => {
        expect(isEmptyArray).toHaveBeenCalledWith(messageRecipients);
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
      });
    });

    describe('has message recipients', () => {
      const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];

      beforeEach(() => {
        isEmptyArray.mockReturnValueOnce(false);
        mountPage({ messageRecipients });
      });

      it('will not redirect', async () => {
        expect(isEmptyArray).toHaveBeenCalledWith(messageRecipients);
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });
  });

  describe('template', () => {
    describe('back link clicked', () => {
      it('will redirect to urgency question', () => {
        mountPage({ isNativeApp: false });

        wrapper.vm.backLinkClicked();

        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/urgency');
      });
    });

    describe('has message recipients', () => {
      it('will show a list of recipients', () => {
        const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];

        mountPage({ messageRecipients });

        expect(wrapper.find('#recipientsMenuList').exists()).toBe(true);
      });
    });
  });
});
