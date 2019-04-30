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
    });
  });

  describe('order a repeat prescription button', () => {
    it('will exist when app is native', () => {
      const button = wrapper.find('#order-prescription-button');
      expect(button.exists()).toBe(true);
    });

    it('won\'t exist when app is not native', () => {
      state.device.isNativeApp = false;
      const button = wrapper.find('#order-prescription-button');
      expect(button.exists()).toBe(false);
    });
  });
});
