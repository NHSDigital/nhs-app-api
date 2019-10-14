import { createStore, mount } from '../../helpers';
import ContentHeader from '../../../../src/components/widgets/ContentHeader';

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
    it('showBanner will return false', () => {
      expect(wrapper.vm.showBanner).toEqual(false);
    });
  });
  describe('appointments-admin-help', () => {
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
      $route = {
        name: 'appointments-admin-help',
        warningBanner: true,
      };
      return mount(ContentHeader, {
        $store,
        $route,
        stubs: {
          'nuxt-link': '<a></a>',
        },
      });
    };

    beforeEach(() => {
      wrapper = mountAs({ native: true });
    });

    it('showBanner will return true', () => {
      expect(wrapper.vm.showBanner).toEqual(true);
    });
  });
});

