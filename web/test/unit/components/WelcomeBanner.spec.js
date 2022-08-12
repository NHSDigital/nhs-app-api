import WelcomeBanner from '@/components/WelcomeBanner';
import { INDEX } from '@/router/routes/general';
import { createStore, mockCookies, shallowMount } from '../helpers';

jest.mock('@/components/widgets/HotJar', () => {});

const createPage = ($store, route = INDEX) => {
  WelcomeBanner.components.HotJar = {
    computed: {},
    staticRenderFns: [],
    name: 'HotJar',
  };

  return shallowMount(WelcomeBanner, {
    $env: {
      ANALYTICS_SCRIPT_URL: 'test script',
    },
    $store,
    $route: {
      ...route,
    },
  });
};

const createLayoutStore = (isProxying = false) => createStore({
  $cookies: mockCookies(),
  getters: {
    'session/isProxying': isProxying,
    'session/currentProfile': { name: 'name' },
  },
  state: {
    linkedAccounts: {
      actingAsUser: {
        id: 'user-id-0',
        fullName: 'mr user 0',
        ageMonths: '10',
        ageYears: '26',
        gpPracticeName: 'practice x',
      },
    },
  },
});

describe('Welcome Display', () => {
  let $store;
  let wrapper;
  describe('Welcome Banner Display', () => {
    beforeEach(() => {
      window.matchMedia = jest.fn(() => ({ matches: true, addEventListener: jest.fn() }));
    });
    it('will display the proxy welcome section when proxying', () => {
      $store = createLayoutStore(true);
      wrapper = createPage($store, INDEX);
      const proxyWelcomeSection = wrapper.find('[id="proxy-welcome-section"]');
      expect(proxyWelcomeSection.exists()).toBe(true);
    });

    it('will display the non proxy welcome section when not proxying', () => {
      $store = createLayoutStore(false);
      wrapper = createPage($store, INDEX);
      const welcomeSection = wrapper.find('[id="welcome-section"]');
      expect(welcomeSection.exists()).toBe(true);
    });
  });

  describe('Picture Banner Display', () => {
    beforeEach(() => {
      window.matchMedia = jest.fn(() => ({ matches: false, addEventListener: jest.fn() }));
    });
    it('will display the proxy picture section when proxying', () => {
      $store = createLayoutStore(true);
      wrapper = createPage($store, INDEX);
      const proxyWelcomeSection = wrapper.find('[id="proxy-picture-banner-section"]');
      expect(proxyWelcomeSection.exists()).toBe(true);
    });

    it('will display the non proxy picture section when not proxying', () => {
      $store = createLayoutStore(false);
      wrapper = createPage($store, INDEX);
      const welcomeSection = wrapper.find('[id="picture-banner-section"]');
      expect(welcomeSection.exists()).toBe(true);
    });
  });
});
