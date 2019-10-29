import each from 'jest-each';
import { createStore, mount } from '../../helpers';
import ContentHeader from '@/components/widgets/ContentHeader';

describe('ContentHeader.vue', () => {
  describe('Login', () => {
    let $route;
    let $store;
    let wrapper;

    const mountAs = ({ native = true }) => {
      const getter = {};
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

    beforeEach(() => {
      wrapper = mountAs({ native: true });
    });

    it('currentBreadCrumbs will return nothing when in Login page', () => {
      expect(wrapper.vm.currentBreadCrumbs).toEqual([]);
    });
    it('showBanner will return undefined', () => {
      expect(wrapper.vm.showBanner).toBeUndefined();
    });
  });
  describe('Admin Help and GP Advice', () => {
    let $store;
    let wrapper;

    const mountAs = ({ native = true, demographicsQuestionAnswered = true, route }) => {
      const getter = {};
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

    each([
      'appointments-admin-help',
      'appointments-gp-advice',
    ]).it('showBanner will return true when demographics answered', (routeName) => {
      wrapper = mountAs({
        native: true,
        route: {
          name: routeName,
          warningBanner: true,
        },
      });
      expect(wrapper.vm.showBanner).toEqual(true);
    });

    each(['appointments-admin-help', 'appointments-gp-advice'])
      .it('showBanner will return false when demographics not answered', (routeName) => {
        wrapper = mountAs({
          native: true,
          demographicsQuestionAnswered: false,
          route: {
            name: routeName,
            warningBanner: true,
          },
        });
        expect(wrapper.vm.showBanner).toEqual(false);
      });
  });
});

