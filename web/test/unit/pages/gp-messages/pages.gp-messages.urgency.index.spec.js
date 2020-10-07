import i18n from '@/plugins/i18n';
import UrgencyPage from '@/pages/messages/gp-messages/urgency/index';
import { redirectTo, isEmptyArray } from '@/lib/utils';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount, createStore } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

jest.mock('@/lib/utils');

describe('gp messages urgency page', () => {
  let wrapper;
  let store;

  const mountPage = ({
    isNativeApp = true,
    messageRecipients = [],
    backLinkOverride = '',
    urgencyChoice = undefined,
  } = {}) => {
    store = createStore({
      state: {
        gpMessages: {
          urgencyChoice,
          messageRecipients,
        },
        device: { isNativeApp },
        navigation: {
          backLinkOverride,
        },
      },
    });
    wrapper = mount(UrgencyPage, {
      $store: store,
      mountOpts: {
        i18n,
      },
    });
  };

  describe('created', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
      EventBus.$on.mockClear();
      EventBus.$off.mockClear();
      isEmptyArray.mockClear();
    });

    describe('no available recipients', () => {
      beforeEach(async () => {
        isEmptyArray.mockImplementation(() => true);
        mountPage();
        await wrapper.vm.$nextTick();
      });

      it('will inform me if there is no recipients', () => {
        expect(store.dispatch).toHaveBeenCalledWith('gpMessages/loadRecipients');
        expect(wrapper.find('#noRecipients').exists()).toBe(true);
      });

      it('will emit UPDATE_HEADER with no recipients as event', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'messages.youCannotSendMessages');
      });

      it('will emit UPDATE_TITLE with no recipients as event', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'messages.youCannotSendMessages');
      });
    });

    describe('some available recipients', () => {
      it('will not emit an UPDATE_HEADER or UPDATE_TITLE event', async () => {
        isEmptyArray.mockImplementation(() => false);

        mountPage([{ recipientIdentifier: '1', name: 'Dr. Test' }]);
        await wrapper.vm.$nextTick();

        expect(EventBus.$emit).not.toHaveBeenCalled();
      });
    });
  });

  describe('back link clicked', () => {
    it('will redirect to gp messages inbox', () => {
      mountPage({ isNativeApp: false });
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages');
    });

    it('will redirect to the back link override', () => {
      mountPage({ isNativeApp: false, backLinkOverride: 'messages/gp-messages/urgency' });
      wrapper.vm.backLinkClicked();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/urgency');
    });
  });

  describe('continue clicked', () => {
    const messageRecipients = [{ recipientIdentifier: '1', name: 'Dr. Test' }];

    it('will show a validation error when making no choice', async () => {
      mountPage({ messageRecipients });
      await wrapper.vm.$nextTick();

      wrapper.find('#continueButton').trigger('click');

      const errorContent = wrapper.find('[data-purpose="error-container"] [data-purpose="error"]');
      const validationError = errorContent.find('ul li');
      expect(errorContent.exists()).toBe(true);
      expect(errorContent.text()).toContain('There\'s a problem');
      expect(validationError.text()).toEqual('You need to select yes or no');
    });

    it('will redirect to /gp-messages/contact-your-gp when the answer is yes', async () => {
      mountPage({ urgencyChoice: 'yes', messageRecipients });
      await wrapper.vm.$nextTick();

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/urgency/contact-your-gp');
    });

    it('will redirect to /gp-messages/recipients when the answer is no', async () => {
      mountPage({ urgencyChoice: 'no', messageRecipients });
      await wrapper.vm.$nextTick();

      wrapper.find('#continueButton').trigger('click');

      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, 'messages/gp-messages/recipients');
    });
  });
});
