import i18n from '@/plugins/i18n';
import PrescriptionsPage from '@/pages/prescriptions/index';
import { create$T, createRouter, createStore, mount } from '../../helpers';

const $t = create$T();

describe('prescriptions page', () => {
  let $store;
  let $style;
  let wrapper;
  let $router;
  let state;
  let button;

  const createState = () => {
    state = {
      prescriptions: {
        hasLoaded: true,
        prescriptionCourses: {},
      },
      device: {
        source: 'iOS',
        isNativeApp: true,
      },
      nominatedPharmacy: {
        nominatedPharmacyEnabled: true,
        pharmacy: {
          pharmacyType: 'P1',
        },
      },
    };
    return state;
  };

  beforeEach(() => {
    $router = createRouter();
    $store = createStore({ state: createState(),
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => ({ serviceProvider: 'pkb',
          serviceType: 'messages' }),
      } });
    wrapper = mount(PrescriptionsPage, {
      $router,
      $store,
      $t,
      $style,
      stubs: {
        'page-title': '<div></div>',
      },
      mountOpts: {
        i18n,
      },
    });
    button = wrapper.find('#repeat-prescription-button');
  });

  describe('order a repeat prescription button', () => {
    it('will exist on page', () => {
      expect(button.exists()).toBe(true);
    });

    it('will have the correct text', () => {
      expect(button.text()).toEqual('Order a repeat prescription');
    });
  });
});
