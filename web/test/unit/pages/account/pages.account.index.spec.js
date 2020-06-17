import AccountPage from '@/pages/account/index';
import AboutUs from '@/components/account/AboutUs';
import WebFooter from '@/components/widgets/WebFooter';
import { createStore, initFilters, mount } from '../../helpers';

describe('Account Page', () => {
  jest.mock('@/services/native-app');
  let wrapper;

  initFilters();

  const $state = {
    device: {
      isNativeApp: false,
    },
    session: {
      user: 'User',
      dateOfBirth: '5/4/2019',
      nhsNumber: '12346778',
    },
    appVersion: {
      webVersion: 'web',
    },
  };

  const buildStoreGetters = ({
    notificationsEnabled = false,
    showLinkedProfiles = false,
  } = {}) => ({
    'serviceJourneyRules/notificationsEnabled': notificationsEnabled,
    'linkedAccounts/hasLinkedAccounts': showLinkedProfiles,
    'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(true),
  });

  const createStyle = () => ({
    'list-menu': 'list-menu',
  });

  const findLinks = (linkWrappers = []) => {
    const urls = [];
    linkWrappers.forEach((linkWrapper) => {
      const link = linkWrapper.element.getAttribute('href');
      urls.push(link);
    });
    return urls;
  };

  const mountPage = ({
    notificationsEnabled = false,
    isNativeApp = false,
    showLinkedProfiles = false,
  }) => {
    const $store = createStore({ state: $state });

    $store.getters = buildStoreGetters({
      notificationsEnabled,
      showLinkedProfiles,
    });

    $state.device.isNativeApp = isNativeApp;
    return mount(AccountPage, {
      $store,
      $state,
      $style: createStyle() });
  };

  describe('on a native app', () => {
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: true,
        showLinkedProfiles: true,
      });
    });

    it('will verify that footer links are subset of links in account page', () => {
      const webFooterWrapper = mount(WebFooter);
      const aboutUsWrapper = mount(AboutUs);

      const footerLinkElements = webFooterWrapper.findAll('ul li a');
      const accountLinkElements = aboutUsWrapper.findAll('ul li a');

      const footerLinks = findLinks(footerLinkElements.wrappers);
      const accountLinks = findLinks(accountLinkElements.wrappers);
      expect(wrapper.find(AboutUs).exists()).toBe(true);
      expect(accountLinks.length).toBeGreaterThan(0);
      expect(footerLinks.every(link => accountLinks.includes(link))).toBeTruthy();
    });

    it('will have a linked profiles link', () => {
      expect(wrapper.findAll('li').at(0).text()).toContain('translate_myAccount.linkedProfilesLink');
    });

    it('will have a cookies link', () => {
      expect(wrapper.findAll('li').at(1).text()).toContain('translate_myAccount.cookiesLink');
    });

    it('will show About the NHS App component', () => {
      expect(wrapper.find(AboutUs).exists()).toBe(true);
    });
  });

  describe('on desktop with showLinkedProfiles false', () => {
    beforeEach(() => {
      wrapper = mountPage({ notificationsEnabled: true,
        isNativeApp: false,
        showLinkedProfiles: false,
      });
    });

    it('will not have a linked profiles link', () => {
      expect(wrapper.find('#linked-profiles-link').exists()).toBe(false);
    });
  });

  describe('on desktop with showLinkedProfiles true', () => {
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: false,
        showLinkedProfiles: true,
      });
    });

    it('will have a linked profiles link', () => {
      expect(wrapper.find('#linked-profiles-link').exists()).toBe(false);
    });
  });
});
