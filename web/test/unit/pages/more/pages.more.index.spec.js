import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createEvent, createStore, mount, createRouter } from '../../helpers';
import { MESSAGES } from '@/lib/routes';

describe('more', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    cdssAdminEnabled = false,
    isProxying = false,
    isNativeApp = false,
    context = true,
    hasUnreadGpMessages = false,
    hasUnreadAppMessages = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp },
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
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'session/isProxying': isProxying,
      },
      $env: { YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com' },
    });
    return mount(More, { $store, $router });
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
  });

  describe('pkb shared links link', () => {
    const getPkbSharedLinksLink = wrapperObj =>
      wrapperObj.find('#btn_pkb_shared_links');

    describe('pkb shared links enabled, not proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ isProxying: false });
      });

      it('will show link', () => {
        expect(getPkbSharedLinksLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb shared links disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: false, isProxying: false });
      });

      it('will not show link', () => {
        expect(getPkbSharedLinksLink(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb shared links enabled, proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ isProxying: true });
      });

      it('will not show link', () => {
        expect(getPkbSharedLinksLink(wrapper).exists()).toBe(false);
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
