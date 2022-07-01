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
  beforeEach(() => {
    EventBus.$emit.mockClear();
    EventBus.$on.mockClear();
    EventBus.$off.mockClear();
    isEmptyArray.mockClear();
    redirectTo.mockClear();
  });

  describe('created', () => {
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
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'messages.cannotSendGpSurgeryMessages');
      });

      it('will emit UPDATE_TITLE with no recipients as event', () => {
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'messages.cannotSendGpSurgeryMessages');
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

    beforeEach(async () => {
      mountPage({ messageRecipients });
      await wrapper.vm.$nextTick();
    });

    (describe('no choice', () => {
      it('will show an error summary', async () => {
        await wrapper.find('#continueButton').trigger('click');
        expect(wrapper.vm.isError).toBe(true);

        const summaryError = wrapper.find('#form-error-summary');
        expect(summaryError.exists()).toBe(true);
      });
    }));

    (describe('choice made', () => {
      it('will set correct validity when answer is yes', async () => {
        const radioButton = wrapper.find('#messagingUrgency-yes');
        expect(radioButton.exists()).toBe(true);
        await radioButton.setChecked();
        expect(wrapper.vm.isValid).toBe(true);

        const continueButton = wrapper.find('#continueButton');
        expect(continueButton.exists()).toBe(true);
        await continueButton.trigger('click');
        expect(wrapper.vm.isError).toBe(false);
      });

      it('will set correct validity when when the answer is no', async () => {
        const radioButton = wrapper.find('#messagingUrgency-no');
        expect(radioButton.exists()).toBe(true);
        await radioButton.setChecked();
        expect(wrapper.vm.isValid).toBe(true);

        const continueButton = wrapper.find('#continueButton');
        expect(continueButton.exists()).toBe(true);
        await continueButton.trigger('click');
        expect(wrapper.vm.isError).toBe(false);
      });
    }));
  });
});
