import PrescriptionsPage from '@/pages/prescriptions/index';
import * as dependency from '@/lib/utils';
import {
  NOMINATED_PHARMACY_PATH,
  NOMINATED_PHARMACY_CHECK_PATH,
  NOMINATED_PHARMACY_INTERRUPT_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
} from '@/router/paths';
import {
  PRESCRIPTIONS_NAME,
} from '@/router/names';
import { createRouter, createStore, mount } from '../../helpers';

describe('prescriptions/index.vue', () => {
  let wrapper;
  let $store;
  let $router;

  const mountPage = (isEnabled) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
        prescriptions: {
          hasLoaded: true,
          prescriptionCourses: {},
        },
        nominatedPharmacy: {
          pharmacy: {
            pharmacyName: undefined,
          },
        },
      },
      getters: {
        'nominatedPharmacy/nominatedPharmacyEnabled': isEnabled,
        'serviceJourneyRules/nominatedPharmacyEnabled': isEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => ({ serviceProvider: 'pkb',
          serviceType: 'messages' }),
      },
    });
    $store.app.$analytics = {
      trackButtonClick: jest.fn(),
    };
    dependency.redirectTo = jest.fn();

    return mount(PrescriptionsPage, {
      $router,
      $store,
      stubs: {
        'page-title': '<div></div>',
      },
    });
  };

  const mountPageWithNominatedPharmacy = (isEnabled) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
        prescriptions: {
          hasLoaded: true,
          prescriptionCourses: {},
        },
        nominatedPharmacy: {
          pharmacy: {
            pharmacyName: 'Boots',
          },
        },
      },
      getters: {
        'nominatedPharmacy/nominatedPharmacyEnabled': isEnabled,
        'serviceJourneyRules/nominatedPharmacyEnabled': isEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => ({ serviceProvider: 'pkb',
          serviceType: 'messages' }),
      },
    });

    $store.app.$analytics = {
      trackButtonClick: jest.fn(),
    };
    dependency.redirectTo = jest.fn();

    return mount(PrescriptionsPage, {
      $route: {
        name: PRESCRIPTIONS_NAME,
      },
      $router,
      $store,
      stubs: {
        'page-title': '<div></div>',
      },
    });
  };

  describe('nominated pharmacy is enabled but not assigned', () => {
    beforeEach(() => {
      wrapper = mountPage(true);
    });

    describe('nominated pharmacy panel', () => {
      let nominatedPharmacyPanel;

      beforeEach(() => {
        global.digitalData = {};
        nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      });

      it('will exist', () => {
        expect(nominatedPharmacyPanel.exists()).toEqual(true);
      });

      describe('clicked', () => {
        beforeEach(() => {
          nominatedPharmacyPanel.trigger('click');
        });

        it('will track nominated pharmacy interrupt path when no nominated pharmacy is assigned', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(NOMINATED_PHARMACY_INTERRUPT_PATH, true);
        });

        it('will redirect to nominated pharmacy interrupt page when no nominated pharmacy is assigned', () => {
          expect(dependency.redirectTo)
            .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT_PATH);
        });
      });
    });

    describe('order a repeat prescription button', () => {
      let button;

      beforeEach(() => {
        button = wrapper.find('#repeat-prescription-button');
      });

      describe('clicked', () => {
        beforeEach(() => {
          button.trigger('click');
        });

        it('will track nominated pharmacy check path', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(NOMINATED_PHARMACY_CHECK_PATH, true);
        });

        it('will redirect to nominated pharmacy check page', () => {
          expect(dependency.redirectTo)
            .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_CHECK_PATH);
        });
      });
    });
  });

  describe('nominated pharmacy is enabled and assigned', () => {
    beforeEach(() => {
      wrapper = mountPageWithNominatedPharmacy(true);
    });

    describe('nominated pharmacy panel', () => {
      let nominatedPharmacyPanel;

      beforeEach(() => {
        global.digitalData = {};
        nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      });

      it('will exist', () => {
        expect(nominatedPharmacyPanel.exists()).toEqual(true);
      });

      describe('clicked', () => {
        beforeEach(() => {
          nominatedPharmacyPanel.trigger('click');
        });

        it('will track nominated pharmacy search path when no nominated pharmacy is assigned', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(NOMINATED_PHARMACY_PATH, true);
        });

        it('will redirect to nominated pharmacy search page when no nominated pharmacy is assigned', () => {
          expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_PATH);
        });
      });
    });
  });

  describe('nominated pharmacy is not enabled', () => {
    beforeEach(() => {
      wrapper = mountPage(false);
    });

    it('will not show pharmacy section', () => {
      expect(wrapper.find('#nominatedPharmacyPanel').exists()).toBe(false);
    });

    describe('order new repeat prescription button', () => {
      let button;

      beforeEach(() => {
        button = wrapper.find('#repeat-prescription-button');
      });

      describe('clicked', () => {
        beforeEach(() => {
          button.trigger('click');
        });

        it('will track prescriptions repeat courses path', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(PRESCRIPTION_REPEAT_COURSES_PATH, true);
        });

        it('will redirect to prescriptions repeat courses', () => {
          expect(dependency.redirectTo)
            .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES_PATH);
        });
      });
    });
  });
});
