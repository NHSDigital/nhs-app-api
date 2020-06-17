import each from 'jest-each';
import ContentHeader from '@/components/widgets/ContentHeader';
import PageTitle from '@/components/widgets/PageTitle';
import { LINKED_PROFILES_NAME } from '@/router/names';
import OnUpdateHeaderMixin from '@/plugins/mixinDefinitions/OnUpdateHeaderMixin';
import { createStore, mount } from '../../helpers';

describe('ContentHeader.vue', () => {
  let getter;

  beforeEach(() => {
    getter = {};
  });

  describe('mixins', () => {
    it('will include the OnUpdateHeaderMixin', () => {
      expect(ContentHeader.mixins).toEqual([OnUpdateHeaderMixin]);
    });
  });

  describe('Login', () => {
    let $route;
    let $store;
    let wrapper;
    let $router;

    const mountAs = ({ linkedAccountsState = {} } = {}) => {
      $store = createStore({
        state: {
          device: {
            isNativeApp: true,
          },
          navigation: {
            crumbSetName: 'testCrumb',
          },
          onlineConsultations: {
            demographicsQuestionAnswered: true,
            adviceProviderName: 'eConsult Health Ltd',
          },
          serviceJourneyRules: {
            rules: {
              cdssAdvice: {
                provider: 'Test',
              },
            },
          },
          linkedAccounts: linkedAccountsState,
          session: {
            csrfToken: 'someToken',
          },
        },
        getters: getter,
      });
      $route = {
        name: 'Login',
        meta: { crumb: {} },
      };
      $router = {
        history: {
          pending: {
            name: 'Login',
          },
        },
      };
      return mount(ContentHeader, { $store, $route, $router });
    };

    it('currentBreadCrumbs will return nothing when in Login page', () => {
      wrapper = mountAs({ native: true });
      expect(wrapper.vm.currentBreadCrumbs()).toBeUndefined();
    });

    it('showYellowBanner will return undefined', () => {
      wrapper = mountAs();
      expect(wrapper.vm.showYellowBanner).toBeUndefined();
    });

    describe('when no warning banner for route', () => {
      it('will not show yellow banner when not proxying', () => {
        getter['session/isProxying'] = false;
        wrapper = mountAs();

        expect(wrapper.vm.showYellowBanner).toEqual(false);
      });

      it('will show yellow banner when proxying', () => {
        getter['session/isProxying'] = true;
        wrapper = mountAs({
          linkedAccountsState: {
            actingAsUser: {
              name: 'david',
            },
          },
        });

        const warning = wrapper.find('#acting-as-other-user-warning');

        // assert
        expect(wrapper.vm.isProxying).toBe(true);
        expect(wrapper.vm.showYellowBanner).toBe(true);
        expect(warning.exists()).toBe(true);
      });
    });
  });

  describe('Corona Virus banner on home', () => {
    let $route;
    let $store;
    let wrapper;
    let coronaVirusBanner;

    const mountAs = ({ linkedAccountsState = {} } = {}) => {
      getter['appVersion/isNativeVersionAfter'] = jest.fn(() => true);
      $store = createStore({
        state: {
          device: {
            isNativeApp: true,
          },
          navigation: {
            crumbSetName: 'testCrumb',
          },
          linkedAccounts: linkedAccountsState,
          session: {
            csrfToken: 'someToken',
          },
        },
        getters: getter,
      });
      $route = {
        name: 'index',
        meta: { crumb: {} },
      };
      return mount(ContentHeader, { $store, $route });
    };

    it('will show Corona Virus Banner when on home page and proxying is false', () => {
      wrapper = mountAs();
      coronaVirusBanner = wrapper.find('#corona-virus-banner');
      getter['session/isProxying'] = false;
      expect(wrapper.vm.showCoronaVirusBanner).toEqual(true);
      expect(coronaVirusBanner.exists()).toEqual(true);
    });

    it('will not show Corona Virus Banner when not on the home page and proxying is false', () => {
      wrapper = mountAs();
      coronaVirusBanner = wrapper.find('#corona-virus-banner');
      $route.name = LINKED_PROFILES_NAME;
      getter['session/isProxying'] = false;
      expect(wrapper.vm.showCoronaVirusBanner).toEqual(false);
      expect(coronaVirusBanner.exists()).toEqual(false);
    });

    it('will not show Corona Virus Banner when on home page and proxying is true', () => {
      getter['session/isProxying'] = true;
      wrapper = mountAs({
        linkedAccountsState: {
          actingAsUser: {
            name: 'david',
          },
        },
      });
      coronaVirusBanner = wrapper.find('#corona-virus-banner');
      expect(wrapper.vm.showCoronaVirusBanner).toEqual(false);
      expect(coronaVirusBanner.exists()).toEqual(false);
    });
  });

  describe('Admin Help and GP Advice', () => {
    let $store;
    let wrapper;

    const mountAs = ({
      demographicsQuestionAnswered = false,
      linkedAccountsState = {},
      route = { meta: { crumb: {} } },
    } = {}) => {
      getter['appVersion/isNativeVersionAfter'] = jest.fn();
      $store = createStore({
        state: {
          device: {
            isNativeApp: true,
          },
          navigation: {
            crumbSetName: 'testCrumb',
          },
          onlineConsultations: {
            demographicsQuestionAnswered,
          },
          serviceJourneyRules: {
            rules: {
              cdssAdmin: {
                provider: 'Test',
                name: 'eConsult Health Ltd',
              },
            },
          },
          linkedAccounts: linkedAccountsState,
          session: {
            csrfToken: 'someToken',
          },
        },
        getters: getter,
      });
      return mount(ContentHeader, {
        $store,
        $route: route,
      });
    };

    it('with demographics question not answered but proxying will display appropriate warning', () => {
      getter['session/isProxying'] = true;
      wrapper = mountAs({
        demographicsQuestionAnswered: false,
        route: {
          name: 'appointments-admin-help',
          meta: { crumb: {} },
        },
        linkedAccountsState: {
          actingAsUser: {
            name: 'david',
          },
        },
      });
      const warning = wrapper.find('#acting-as-other-user-warning');

      // assert
      expect(wrapper.vm.demographicsQuestionAnswered).toBe(false);
      expect(wrapper.vm.isProxying).toBe(true);
      expect(wrapper.vm.showYellowBanner).toEqual(true);
      expect(warning.exists()).toBe(true);
    });

    each([
      'appointments-gp-appointments-admin-help',
      'appointments-gp-appointments-gp-advice',
    ]).it('showYellowBanner will return true when demographics answered', (routeName) => {
      getter['session/isProxying'] = false;
      wrapper = mountAs({
        demographicsQuestionAnswered: true,
        route: {
          name: routeName,
          meta: {
            crumb: {},
            warningBanner: true,
          },
        },
      });
      expect(wrapper.vm.showYellowBanner).toEqual(true);
    });

    each(['appointments-gp-appointments-admin-help', 'appointments-gp-appointments-gp-advice'])
      .it('showYellowBanner will return false when demographics not answered', (routeName) => {
        getter['session/isProxying'] = false;
        wrapper = mountAs({
          demographicsQuestionAnswered: false,
          route: {
            name: routeName,
            meta: {
              crumb: {},
              warningBanner: true,
            },
          },
        });
        expect(wrapper.vm.showYellowBanner).toEqual(false);
      });
  });

  describe('PageTitle', () => {
    let $store;
    let wrapper;

    const mountAs = () => {
      getter['appVersion/isNativeVersionAfter'] = jest.fn();
      $store = createStore({
        state: {
          device: { isNativeApp: true },
          navigation: { crumbSetName: {} },
          onlineConsultations: { demographicsQuestionAnswered: false },
          serviceJourneyRules: {
            rules: {
              cdssAdmin: {
                provider: 'Test',
                name: 'eConsult Health Ltd',
              },
            },
          },
          linkedAccounts: {},
          session: { csrfToken: {} },
        },
        getters: getter,
      });
      return mount(ContentHeader, {
        $store,
        $route: { meta: { crumb: {} } },
      });
    };

    it('will not be shown if header is empty', () => {
      wrapper = mountAs();
      wrapper.vm.header = '';
      expect(wrapper.find(PageTitle).exists()).toBe(false);
    });

    it('will pass caption to PageTitle', () => {
      wrapper = mountAs();
      wrapper.vm.header = 'Test Header';
      wrapper.vm.caption = 'Test Caption';
      const pageTitle = wrapper.find(PageTitle);
      expect(pageTitle.vm.caption).toBe('Test Caption');
    });

    it('will set title key as combination of caption and header', () => {
      wrapper = mountAs();
      wrapper.vm.caption = 'Test Caption';
      wrapper.vm.header = 'Test Header';
      const pageTitle = wrapper.find(PageTitle);
      expect(pageTitle.vm.titleKey).toBe('Test HeaderTest Caption');
    });
  });
});
