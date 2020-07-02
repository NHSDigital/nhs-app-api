import * as utils from '@/lib/utils';
import each from 'jest-each';
import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount } from '../../helpers';
import { MESSAGES, HEALTH_INFORMATION_UPDATES } from '@/lib/routes';

describe('more', () => {
  let linkElement;
  let wrapper;
  let $store;

  const mountAs = ({
    cdssAdminEnabled = false,
    isProxying = false,
    isNativeApp = false,
    isProofLevel9 = true,
    context = true,
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
        knownServices: {
          knownServices: [{
            id: 'pkb',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationMessagesEnabled': silverIntegrationMessagesEnabled,
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/messagingEnabled': sjrMessagingEnabled,
        'serviceJourneyRules/im1MessagingEnabled': sjrIm1MessagingEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'session/isProxying': isProxying,
        'session/isProofLevel9': isProofLevel9,
      },
      $env: { YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com' },
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
            expect(utils.redirectTo).toBeCalledWith(wrapper.vm, MESSAGES.path);
          });

          it('will not set breadcrumb', () => {
            expect($store.dispatch).not.toBeCalledWith('navigation/setRouteCrumb', expect.anything());
          });
        });
      });
    });
  });

  describe('only app messaging available', () => {
    each([
      [false, true], // app messages only
      [true, false], // app messages and (silver messaging but not P9)
    ]).describe('silver messaging is %s, proof level 9 is %s', (
      silverIntegrationMessagesEnabled,
      isProofLevel9,
    ) => {
      beforeEach(() => {
        wrapper = mountAs({
          sjrMessagingEnabled: true,
          sjrIm1MessagingEnabled: false,
          silverIntegrationMessagesEnabled,
          isProofLevel9,
        });
      });

      it('will not show messages link', () => {
        expect(wrapper.find('#btn_messages').exists()).toBe(false);
      });

      describe('app messaging link', () => {
        let appMessagingLink;

        beforeEach(() => {
          appMessagingLink = wrapper.find('#btn_appMessaging');
        });

        it('will exist', () => {
          expect(appMessagingLink.exists()).toBe(true);
        });

        describe('click', () => {
          beforeEach(() => {
            appMessagingLink.trigger('click');
          });

          it('will redirect to HEALTH_INFORMATION_UPDATES', () => {
            expect(utils.redirectTo).toBeCalledWith(wrapper.vm, HEALTH_INFORMATION_UPDATES.path);
          });

          it('will set breadcrumb to `appMessagesOnlyCrumb`', () => {
            expect($store.dispatch).toBeCalledWith('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
          });
        });
      });
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
      provider, context, isProxying, expectedResult,
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
        wrapper = mountAs({ context, isProxying });
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
        expect(utils.redirectTo).toBeCalledWith(wrapper.vm, '/event/path');
      });
    });

    describe('navigateToDataSharing', () => {
      it('will navigate to event current target path name if native', () => {
        wrapper = mountAs({ isNativeApp: true });
        const event = createEvent({ currentTarget: { pathname: '/event/path' } });
        wrapper.vm.navigateToDataSharing(event);

        expect(utils.redirectTo).toBeCalledWith(wrapper.vm, '/event/path');
        expect(event.preventDefault).toHaveBeenCalled();
      });
      it('will navigate to ndop home page if not native', () => {
        wrapper = mountAs();
        const event = createEvent({ currentTarget: { pathname: 'testYourDataMattersUrl.com' } });
        wrapper.vm.navigateToDataSharing(event);

        expect(utils.redirectTo).not.toHaveBeenCalled();
        expect(event.preventDefault).not.toHaveBeenCalled();
        expect(global.open).toHaveBeenCalledWith('testYourDataMattersUrl.com', '_blank');
      });
    });
  });
});
