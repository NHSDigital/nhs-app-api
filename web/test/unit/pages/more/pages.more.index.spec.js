import * as utils from '@/lib/utils';
import each from 'jest-each';
import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { MESSAGES_PATH, DATA_SHARING_OVERVIEW_PATH } from '@/router/paths';
import { YOUR_NHS_DATA_MATTERS_URL } from '@/router/externalLinks';
import { createStore, mount } from '../../helpers';

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
          }, {
            id: 'engage',
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
            expect(utils.redirectTo).toBeCalledWith(wrapper.vm, MESSAGES_PATH);
          });

          it('will not set breadcrumb', () => {
            expect($store.dispatch).not.toBeCalledWith('navigation/setRouteCrumb', expect.anything());
          });
        });
      });
    });
  });

  describe('view third-party links', () => {
    each([
      ['engage', 'admin', true, false, true],
      ['engage', 'admin', true, true, false],
      ['engage', 'admin', false, false, false],
      ['pkb', 'secondary shared links', true, false, true],
      ['pkb', 'secondary shared links', true, true, false],
      ['pkb', 'secondary shared links', false, false, false],
      ['cie', 'secondary shared links', true, false, true],
      ['cie', 'secondary shared links', true, true, false],
      ['cie', 'secondary shared links', false, false, false],
    ]).describe('%s %s enabled is %s, proxy is %s', (
      provider, _, context, isProxying, expectedResult,
    ) => {
      beforeEach(() => {
        switch (provider) {
          case 'engage':
            linkElement = '#btn_engage_admin';
            break;
          case 'cie':
            linkElement = '#btn_pkb_cie_shared_links';
            break;
          case 'pkb':
            linkElement = '#btn_pkb_shared_links';
            break;
          default:
            break;
        }

        wrapper = mountAs({ context, isProxying });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
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

  describe('methods', () => {
    describe('navigateToDataSharing', () => {
      it('will redirect to DATA_SHARING_OVERVIEW_PATH if native', () => {
        wrapper = mountAs({ isNativeApp: true });

        wrapper.vm.navigateToDataSharing();

        expect(utils.redirectTo).toHaveBeenCalledWith(wrapper.vm, DATA_SHARING_OVERVIEW_PATH);
      });

      it('will navigate to ndop home page if not native', () => {
        wrapper = mountAs();

        wrapper.vm.navigateToDataSharing();

        expect(window.open).toHaveBeenCalledWith(YOUR_NHS_DATA_MATTERS_URL, '_blank');
      });
    });
  });
});
