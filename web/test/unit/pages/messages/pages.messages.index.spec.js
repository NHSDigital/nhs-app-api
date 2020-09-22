import each from 'jest-each';
import i18n from '@/plugins/i18n';
import Messages from '@/pages/messages/';
import { redirectTo } from '@/lib/utils';
import { GP_MESSAGES_PATH, HEALTH_INFORMATION_UPDATES_PATH } from '@/router/paths';
import { mount, createRouter, createStore } from '../../helpers';

jest.mock('@/lib/utils');

let linkElement;
let wrapper;
let $store;
let $router;

const mountPage = ({
  sjrMessagingEnabled = true,
  sjrIm1MessagingEnabled = true,
  practiceIm1MessagingEnabled = true,
  context = true,
  isProxying = false,
  isProofLevel9 = true,
  hasUnreadAppMessages = false,
  hasUnreadGPMessages = false,
} = {}) => {
  $router = createRouter();
  $store = createStore({
    state: {
      practiceSettings: { im1MessagingEnabled: practiceIm1MessagingEnabled },
      device: { isNativeApp: false },
      gpMessages: { hasUnread: hasUnreadGPMessages },
      messaging: { hasUnread: hasUnreadAppMessages },
      knownServices: {
        knownServices: [{
          id: 'pkb',
          url: 'www.url.com',
        }],
      },
    },
    getters: {
      'serviceJourneyRules/messagingEnabled': sjrMessagingEnabled,
      'serviceJourneyRules/im1MessagingEnabled': sjrIm1MessagingEnabled,
      'serviceJourneyRules/silverIntegrationEnabled': () => (context),
      'session/isProxying': isProxying,
      'session/isProofLevel9': isProofLevel9,
    },
  });
  wrapper = mount(Messages, { $store, $router, mountOpts: { i18n } });
};

describe('messages page', () => {
  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('no messaging services available', () => {
    mountPage({
      practiceIm1MessagingEnabled: false,
      sjrIm1MessagingEnabled: false,
      context: false,
      sjrMessagingEnabled: false,
    });
    const im1MessagingLink = wrapper.find('#btn_im1_messaging');
    const noMessagesText = wrapper.find('[data-purpose="no-messages-available"]');

    expect(im1MessagingLink.exists()).toBe(false);
    expect(noMessagesText.exists()).toBe(true);
    expect(noMessagesText.text()).toEqual('You have no messages.');
  });

  describe('im1 messaging services', () => {
    describe('practice disabled or sjr disabled', () => {
      each([
        { practice: true, sjr: false },
        { practice: false, sjr: true },
        { practice: false, sjr: false },
      ]).it('will not show link', ({ practice, sjr }) => {
        mountPage({
          practiceIm1MessagingEnabled: practice,
          sjrIm1MessagingEnabled: sjr,
        });
        expect(wrapper.find('#btn_im1_messaging').exists()).toBe(false);
      });
    });

    describe('messaging link', () => {
      each([
        { sjr: true, expected: true },
        { sjr: false, expected: false },
      ]).it('will not show link', ({ sjr, expected }) => {
        mountPage({
          sjrMessagingEnabled: sjr,
        });
        expect(wrapper.find('#btn_appMessaging').exists()).toBe(expected);
      });
    });

    describe('practice and sjr enabled', () => {
      it('will show link', () => {
        mountPage();
        const im1MessagingLink = wrapper.find('#btn_im1_messaging');
        const im1MessagingLinkSubHeader = im1MessagingLink.find('h2');
        const im1MessagingLinkBody = im1MessagingLink.find('p');

        expect(im1MessagingLink.exists()).toBe(true);
        expect(im1MessagingLinkSubHeader.exists()).toBe(true);
        expect(im1MessagingLinkBody.exists()).toBe(true);
        expect(im1MessagingLinkSubHeader.text()).toEqual('GP surgery messages');
        expect(im1MessagingLinkBody.text()).toEqual('Send or view messages from your GP surgery');
      });
    });
  });

  describe('unread message indicators', () => {
    each([
      ['show the indicator when there is unread GP messages', true, true],
      ['show the indicator when there is no unread GP messages', false, false],
    ]).it('will %s', async (_, hasUnread, indicatorShown) => {
      mountPage({ hasUnreadGPMessages: hasUnread });
      await wrapper.vm.$nextTick();
      expect(wrapper.find('#btn_im1_messaging_unreadIndicator').exists()).toBe(indicatorShown);
    });

    each([
      ['show the indicator when there is unread app messages', true, true],
      ['not show the indicator when there is no unread app messages', false, false],
    ]).it('will %s', async (_, hasUnread, indicatorShown) => {
      mountPage({ hasUnreadAppMessages: hasUnread });
      await wrapper.vm.$nextTick();
      expect(wrapper.find('#btn_appMessaging_unreadIndicator').exists()).toBe(indicatorShown);
    });
  });

  describe('view third-party messaging', () => {
    each([
      ['pkb', true, false, true, true],
      ['pkb', true, false, false, false],
      ['pkb', true, true, true, false],
      ['pkb', false, false, true, false],
      ['cie', true, false, true, true],
      ['cie', true, false, false, false],
      ['cie', true, true, true, false],
      ['cie', false, false, true, false],
    ]).describe('%s messaging enabled is %s, proxy is %s', (
      provider, context, isProxying, isProofLevel9, expectedResult,
    ) => {
      beforeEach(() => {
        switch (provider) {
          case 'cie':
            linkElement = '#btn_pkb_cie_messages_and_consultations';
            break;
          case 'pkb':
            linkElement = '#btn_pkb_messages_and_consultations';
            break;
          default:
            break;
        }

        mountPage({ context, isProxying, isProofLevel9 });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });

  describe('navigateToGpMessages', () => {
    it('will redirect to gp messages', () => {
      mountPage();
      wrapper.vm.navigateToGpMessages();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, GP_MESSAGES_PATH);
    });
  });

  describe('navigateToAppMessages', () => {
    it('will redirect to app messages', () => {
      mountPage();
      wrapper.vm.navigateToAppMessages();
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES_PATH);
    });

    it('will not set route crumb to appMessagesOnlyCrumb when not the only messages service enabled', () => {
      mountPage();
      wrapper.vm.navigateToAppMessages();
      expect($store.dispatch).not.toHaveBeenCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
    });

    it('will set route crumb to appMessagesOnlyCrumb when the only messages service enabled', () => {
      mountPage({
        sjrIm1MessagingEnabled: false,
        context: false,
      });
      wrapper.vm.navigateToAppMessages();
      expect($store.dispatch).toHaveBeenCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
    });
  });
});
