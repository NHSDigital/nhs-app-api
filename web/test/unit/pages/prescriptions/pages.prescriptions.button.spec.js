import PrescriptionsPage from '@/pages/prescriptions/index';
import { create$T, createRouter, createStore, mount } from '../../helpers';

const $t = create$T();

describe('prescriptions page', () => {
  let $store;
  let $style;
  let wrapper;
  let $router;
  let state;

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
    $store = createStore({ state: createState() });
    wrapper = mount(PrescriptionsPage, {
      $router,
      $store,
      $t,
      $style,
      stubs: {
        'page-title': '<div></div>',
      },
    });
  });

  describe('order a repeat prescription button', () => {
    it('will exist on page', () => {
      const button = wrapper.find('#order-prescription-button');
      expect(button.exists()).toBe(true);
    });
  });
});
