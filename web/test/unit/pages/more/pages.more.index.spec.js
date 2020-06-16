import each from 'jest-each';
import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount, createRouter, create$T } from '../../helpers';
import { MESSAGES, HEALTH_INFORMATION_UPDATES } from '@/lib/routes';

describe('more', () => {
  let linkElement;
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    cdssAdminEnabled = false,
    isProxy = false,
    isNativeApp = false,
    context = true,
    sjrIm1MessagingEnabled = true,
    hasUnreadGpMessages = false,
    hasUnreadAppMessages = false,
    sjrMessagingEnabled = true,
    im1MessagingPracticeEnabled = true,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp },
        practiceSettings: { im1MessagingEnabled: im1MessagingPracticeEnabled },
        gpMessages: { hasUnread: hasUnreadGpMessages },
        messaging: { hasUnread: hasUnreadAppMessages },
        knownServices: {
          knownServices: [{
            id: 'pkb',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/messagingEnabled': sjrMessagingEnabled,
        'serviceJourneyRules/im1MessagingEnabled': sjrIm1MessagingEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'session/isProxying': isProxy,
      },
      $env: { YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com' },
    });
    return mount(More, { $store, $router, $t: create$T() });
  };

  beforeEach(() => {
    wrapper = mountAs();
    window.open = jest.fn();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  it('will include the organ donation link', () => {
    expect(wrapper.find(OrganDonationLink).exists()).toBe(true);
  });

  describe('gp help link', () => {
    describe('sjr cdss admin disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ cdssAdminEnabled: false });
      });

      it('will not show link', () => {
        expect(wrapper.find('#btn_gp_help').exists()).toBe(false);
      });
    });

    describe('sjr cdss admin enabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ cdssAdminEnabled: true });
      });

      it('will show link', () => {
        expect(wrapper.find('#btn_gp_help').exists()).toBe(true);
      });
    });
  });

  describe('messaging link', () => {
    beforeEach(() => {
      wrapper = mountAs();
    });

    it('will show link', () => {
      const messagingLink = wrapper.find('#btn_messages');
      expect(messagingLink.exists()).toBe(true);
    });

    it('will navigate to MESSAGES', () => {
      const event = createEvent({ currentTarget: { pathname: MESSAGES.path } });
      wrapper.vm.navigate(event);
      expect($router.push).toBeCalledWith(MESSAGES.path);
    });

    it('will only show the app messaging link if only app messaging is enabled', () => {
      wrapper = mountAs({
        sjrIm1MessagingEnabled: false,
        sjrMessagingEnabled: true,
        context: false });
      expect(wrapper.find('#btn_messages').exists()).toBe(false);
      expect(wrapper.find('#btn_appMessaging').exists()).toBe(true);
    });

    it('will dispatch the new breadcrumb when only the app messaging service is available', () => {
      wrapper = mountAs({
        sjrIm1MessagingEnabled: false,
        sjrMessagingEnabled: true,
        context: false });

      const event = createEvent({ currentTarget: { pathname: HEALTH_INFORMATION_UPDATES.path } });
      wrapper.vm.navigateToMessages(event);
      expect($store.dispatch).toHaveBeenCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyMoreCrumb');
    });

    it('will not dispatch the new breadcrumb when more than one messages service is available', () => {
      const event = createEvent({ currentTarget: { pathname: HEALTH_INFORMATION_UPDATES.path } });
      wrapper.vm.navigateToMessages(event);
      expect($store.dispatch).not.toHaveBeenCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyMoreCrumb');
    });

    it('will show the correct menu item text if only app messaging enabled', () => {
      const expectedText = 'translate_messagesHub.appMessaging.subheader';
      wrapper = mountAs({
        sjrIm1MessagingEnabled: false,
        sjrMessagingEnabled: true,
        context: false });

      expect(wrapper.find('#btn_appMessaging').text()).toContain(expectedText);
    });

    it('will show the correct menu item text if more than app messaging enabled', () => {
      const expectedText = 'translate_sc04.messages.subheader';
      expect(wrapper.find('#btn_messages').text()).toContain(expectedText);
    });
  });

  describe('view third-party shared links link', () => {
    each([
      ['pkb', true, false, true],
      ['pkb', true, true, false],
      ['pkb', false, false, false],
      ['cie', true, false, true],
      ['cie', true, true, false],
      ['cie', false, false, false],
    ]).describe('%s secondary shared links enabled is %s, proxy is %s', (
      provider, context, isProxy, expectedResult,
    ) => {
      switch (provider) {
        case 'cie':
          linkElement = '#btn_pkb_cie_shared_links';
          break;
        case 'pkb':
          linkElement = '#btn_pkb_shared_links';
          break;
        default:
          break;
      }

      beforeEach(() => {
        wrapper = mountAs({ context, isProxy });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });

  describe('unread messages indicator', () => {
    const getUnreadIndicator = wrapperObj =>
      wrapperObj.find('#btn_messages_unreadIndicator');

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

  describe('methods', () => {
    describe('navigate', () => {
      it('will navigate to event current target path name', () => {
        wrapper.vm.navigate(createEvent({ currentTarget: { pathname: '/event/path' } }));
        expect($router.push).toHaveBeenCalledWith('/event/path');
      });
    });

    describe('navigateToDataSharing', () => {
      it('will navigate to event current target path name if native', () => {
        wrapper = mountAs({ isNativeApp: true });
        const event = createEvent({ currentTarget: { pathname: '/event/path' } });
        wrapper.vm.navigateToDataSharing(event);

        expect($router.push).toHaveBeenCalledWith('/event/path');
        expect(event.preventDefault).toHaveBeenCalled();
      });
      it('will navigate to ndop home page if not native', () => {
        wrapper = mountAs();
        const event = createEvent({ currentTarget: { pathname: 'testYourDataMattersUrl.com' } });
        wrapper.vm.navigateToDataSharing(event);

        expect($router.push).not.toHaveBeenCalledWith('testYourDataMattersUrl.com');
        expect(event.preventDefault).not.toHaveBeenCalled();
        expect(window.open).toHaveBeenCalledWith('testYourDataMattersUrl.com', '_blank');
      });
    });
  });
});
