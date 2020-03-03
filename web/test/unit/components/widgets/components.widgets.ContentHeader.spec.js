import each from 'jest-each';
import { createStore, mount } from '../../helpers';
import ContentHeader from '@/components/widgets/ContentHeader';
import { LINKED_PROFILES } from '@/lib/routes';


describe('ContentHeader.vue', () => {
  let getter;

  beforeEach(() => {
    getter = {};
  });

  describe('Login', () => {
    let $route;
    let $store;
    let wrapper;
    let $router;

    const mountAs = ({ native = true, linkedAccountsState = {} }) => {
      getter['appVersion/isNativeVersionAfter'] = jest.fn();
      $store = createStore({
        state: {
          device: {
            isNativeApp: native,
          },
          header: {
            headerText: 'Test',
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
      expect(wrapper.vm.currentBreadCrumbs()).toEqual([]);
    });

    it('showYellowBanner will return undefined', () => {
      wrapper = mountAs({ native: true });
      expect(wrapper.vm.showYellowBanner).toBeUndefined();
    });

    describe('when no warning banner for route', () => {
      it('will not show yellow banner when not proxying', () => {
        getter['session/isProxying'] = false;
        wrapper = mountAs({ native: true });

        expect(wrapper.vm.showYellowBanner).toEqual(false);
      });

      it('will show yellow banner when proxying', () => {
        getter['session/isProxying'] = true;
        wrapper = mountAs({
          native: true,
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

    const mountAs = ({ native = true, linkedAccountsState = {} }) => {
      getter['appVersion/isNativeVersionAfter'] = jest.fn();
      $store = createStore({
        state: {
          device: {
            isNativeApp: native,
          },
          navigation: {
            crumbSetName: 'testCrumb',
          },
          header: {
            headerText: 'Test',
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
      };
      return mount(ContentHeader, { $store, $route });
    };

    it('will show Corona Virus Banner when on home page and proxying is false', () => {
      wrapper = mountAs({ native: true });
      coronaVirusBanner = wrapper.find('#corona-virus-banner');
      getter['session/isProxying'] = false;
      expect(wrapper.vm.showCoronaVirusBanner).toEqual(true);
      expect(coronaVirusBanner.exists()).toEqual(true);
    });

    it('will not show Corona Virus Banner when not on the home page and proxying is false', () => {
      wrapper = mountAs({ native: true });
      coronaVirusBanner = wrapper.find('#corona-virus-banner');
      $route.name = LINKED_PROFILES.name;
      getter['session/isProxying'] = false;
      expect(wrapper.vm.showCoronaVirusBanner).toEqual(false);
      expect(coronaVirusBanner.exists()).toEqual(false);
    });

    it('will not show Corona Virus Banner when on home page and proxying is true', () => {
      getter['session/isProxying'] = true;
      wrapper = mountAs({
        native: true,
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
      native = true,
      demographicsQuestionAnswered = false,
      linkedAccountsState = {},
      route,
    }) => {
      getter['appVersion/isNativeVersionAfter'] = jest.fn();
      $store = createStore({
        state: {
          device: {
            isNativeApp: native,
          },
          header: {
            headerText: 'Test',
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
        stubs: {
          'nuxt-link': '<a></a>',
        },
      });
    };

    it('with demographics question not answered but proxying will display appropriate warning', () => {
      getter['session/isProxying'] = true;
      wrapper = mountAs({
        native: true,
        demographicsQuestionAnswered: false,
        route: {
          name: 'appointments-admin-help',
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
      'appointments-admin-help',
      'appointments-gp-advice',
    ]).it('showYellowBanner will return true when demographics answered', (routeName) => {
      getter['session/isProxying'] = false;
      wrapper = mountAs({
        native: true,
        demographicsQuestionAnswered: true,
        route: {
          name: routeName,
          warningBanner: true,
        },
      });
      expect(wrapper.vm.showYellowBanner).toEqual(true);
    });

    each(['appointments-admin-help', 'appointments-gp-advice'])
      .it('showYellowBanner will return false when demographics not answered', (routeName) => {
        getter['session/isProxying'] = false;
        wrapper = mountAs({
          native: true,
          demographicsQuestionAnswered: false,
          route: {
            name: routeName,
            warningBanner: true,
          },
        });
        expect(wrapper.vm.showYellowBanner).toEqual(false);
      });
  });
});
