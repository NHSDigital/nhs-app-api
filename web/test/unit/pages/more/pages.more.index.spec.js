import * as utils from '@/lib/utils';
import each from 'jest-each';
import More from '@/pages/more';
import { MESSAGES_PATH } from '@/router/paths';
import { createStore, mount } from '../../helpers';

describe('more', () => {
  let wrapper;
  let $store;

  const mountAs = ({
    isProxying = false,
    isNativeApp = false,
    isProofLevel9 = true,
    sjrIm1MessagingEnabled = true,
    sjrMessagingEnabled = true,
    silverIntegrationMessagesEnabled = true,
    hasUnreadGpMessages = false,
    hasUnreadAppMessages = false,
    im1MessagingPracticeEnabled = true,
  } = {}) => {
    $store = createStore({
      state: {
        device: { isNativeApp },
        practiceSettings: { im1MessagingEnabled: im1MessagingPracticeEnabled },
        gpMessages: { hasUnread: hasUnreadGpMessages },
        messaging: { hasUnread: hasUnreadAppMessages },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationMessagesEnabled': silverIntegrationMessagesEnabled,
        'serviceJourneyRules/messagingEnabled': sjrMessagingEnabled,
        'serviceJourneyRules/im1MessagingEnabled': sjrIm1MessagingEnabled,
        'session/isProxying': isProxying,
        'session/isProofLevel9': isProofLevel9,
      },
    });
    return mount(More, { $store });
  };

  beforeEach(() => {
    wrapper = mountAs();
    global.open = jest.fn();
    global.digitalData = {};
    utils.redirectTo = jest.fn();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  describe('not only app messaging is available', () => {
    each([
      [true, true, true], // all available
      [true, true, false], // all except silver messaging
      [false, true, false], // im1 messaging only
      [false, true, true], // all except app messaging
      [false, false, true], // silver messaging only
      [true, false, true], // all except im1 messaging
      [false, false, false], // messaging is disabled
    ]).describe('app messaging is %s, IM1 messaging is %s, silver messaging is %s', (
      sjrMessagingEnabled,
      sjrIm1MessagingEnabled,
      silverIntegrationMessagesEnabled,
    ) => {
      beforeEach(() => {
        wrapper = mountAs({
          sjrIm1MessagingEnabled,
          sjrMessagingEnabled,
          silverIntegrationMessagesEnabled,
          isProofLevel9: true,
        });
      });

      it('will not show app messaging link', () => {
        expect(wrapper.find('#btn_appMessaging').exists()).toBe(false);
      });

      describe('messaging link', () => {
        let messagingLink;

        beforeEach(() => {
          messagingLink = wrapper.find('#btn_messages');
        });

        it('will exist', () => {
          expect(messagingLink.exists()).toBe(true);
        });

        describe('click', () => {
          beforeEach(() => {
            messagingLink.trigger('click');
          });
          it('will redirect to MESSAGES', () => {
            expect(utils.redirectTo).toBeCalledWith(wrapper.vm, MESSAGES_PATH);
          });

          it('will not set breadcrumb', () => {
            expect($store.dispatch).not.toBeCalledWith('navigation/setRouteCrumb', expect.anything());
          });
        });
      });
    });
  });

  describe('unread messages indicator', () => {
    const getUnreadIndicator = wrapperObj =>
      wrapperObj.find('#btn_messages_discIndicator');

    describe('has unread indicators', () => {
      beforeEach(async () => {
        wrapper = mountAs({ hasUnreadAppMessages: true, hasUnreadGpMessages: true });
        await wrapper.vm.$nextTick();
      });

      it('will show the the unread indicator', () => {
        expect(getUnreadIndicator(wrapper).exists()).toBe(true);
      });
    });

    it('will not show the unread indicators when there are no unread messages', () => {
      expect(getUnreadIndicator(wrapper).exists()).toBe(false);
    });
  });
});
