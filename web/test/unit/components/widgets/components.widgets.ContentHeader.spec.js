import each from 'jest-each';
import { createStore, mount } from '../../helpers';
import ContentHeader from '@/components/widgets/ContentHeader';

describe('ContentHeader.vue', () => {
  let getter;

  beforeEach(() => {
    getter = {};
  });

  describe('Login', () => {
    let $route;
    let $store;
    let wrapper;

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
      return mount(ContentHeader, { $store, $route });
    };

    it('currentBreadCrumbs will return nothing when in Login page', () => {
      wrapper = mountAs({ native: true });
      expect(wrapper.vm.currentBreadCrumbs).toEqual([]);
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
