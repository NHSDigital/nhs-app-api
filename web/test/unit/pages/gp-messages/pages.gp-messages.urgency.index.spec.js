import UrgencyPage from '@/pages/messages/gp-messages/urgency/index';
import { mount, createStore } from '../../helpers';
import { redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('gp messages urgency page', () => {
  let wrapper;
  let store;

  const mountPage = ({
    isNativeApp = true,
    messageRecipients = [],
    urgencyChoice = undefined,
  } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
          urgencyChoice,
          messageRecipients,
        },
        device: { isNativeApp },
      },
    });
    wrapper = mount(UrgencyPage, {
      $store: store,
    });
  };

  describe('fetch', () => {
    it('will inform me if there is no recipients', async () => {
      mountPage();
      await wrapper.vm.$options.fetch({
        store,
        redirect: jest.fn(),
        app: { i18n: { t: jest.fn() } },
      });

      expect(store.dispatch).toHaveBeenCalledWith('gpMessages/loadRecipients');
      expect(wrapper.find('#noRecipients').exists()).toBe(true);
    });
  });

  describe('back link clicked', () => {
    it('will redirect to gp messages inbox', () => {
      mountPage({ isNativeApp: false });
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages');
    });
  });

  describe('continue clicked', () => {
    const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];

    it('will show a validation error when making no choice', async () => {
      mountPage({ messageRecipients });

      wrapper.find('#continueButton').trigger('click');

      const errorContent = wrapper.find('[data-purpose="error-container"] [data-purpose="error"]');
      const validationError = errorContent.find('ul li');
      expect(errorContent.exists()).toBe(true);
      expect(errorContent.text()).toContain('translate_im02.noOptionSelectedErrorHeader');
      expect(validationError.text()).toEqual('translate_im02.noOptionSelectedErrorText');
    });

    it('will redirect to /gp-messages/contact-your-gp when the answer is yes', async () => {
      mountPage({ urgencyChoice: 'yes', messageRecipients });

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages/urgency/contact-your-gp');
    });

    it('will redirect to /gp-messages/recipients when the answer is no', async () => {
      mountPage({ urgencyChoice: 'no', messageRecipients });

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, '/messages/gp-messages/recipients');
    });
  });
});
