import PrescriptionsPage from '@/pages/prescriptions/index';
import { createRouter, createStore, mount } from '../../helpers';
import { NOMINATED_PHARMACY,
  NOMINATED_PHARMACY_CHECK,
  NOMINATED_PHARMACY_SEARCH,
  PRESCRIPTIONS,
  PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';

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
      },
    });
    $store.app.$analytics = {
      trackButtonClick: jest.fn(),
    };

    return mount(PrescriptionsPage, {
      $route: {
        name: PRESCRIPTIONS.name,
      },
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
      },
    });

    $store.app.$analytics = {
      trackButtonClick: jest.fn(),
    };
    return mount(PrescriptionsPage, {
      $route: {
        name: PRESCRIPTIONS.name,
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

    it('will show pharmacy section', () => {
      expect(wrapper.find('#nominated-pharmacy-section').exists()).toBe(true);
    });

    describe('pharmacy link', () => {
      let nominatedPharmacyLink;

      beforeEach(() => {
        global.digitalData = {};
        nominatedPharmacyLink = wrapper.find('#nominated-pharmacy');
      });

      it('will exist', () => {
        expect(nominatedPharmacyLink.exists()).toEqual(true);
      });

      describe('clicked', () => {
        beforeEach(() => {
          nominatedPharmacyLink.trigger('click');
        });

        it('will track nominated pharmacy search path when no nominated pharmacy is assigned', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(NOMINATED_PHARMACY_SEARCH.path, true);
        });

        it('will redirect to nominated pharmacy search page when no nominated pharmacy is assigned', () => {
          expect($router.push).toHaveBeenCalledWith(NOMINATED_PHARMACY_SEARCH.path);
        });
      });
    });

    describe('order a repeat prescription button', () => {
      let button;

      beforeEach(() => {
        button = wrapper.find('#order-prescription-button');
      });

      describe('clicked', () => {
        beforeEach(() => {
          button.trigger('click');
        });

        it('will track nominated pharmacy check path', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(NOMINATED_PHARMACY_CHECK.path, true);
        });

        it('will redirect to nominated pharmacy check page', () => {
          expect($router.push).toHaveBeenCalledWith(NOMINATED_PHARMACY_CHECK.path);
        });
      });
    });
  });

  describe('nominated pharmacy is enabled and assigned', () => {
    beforeEach(() => {
      wrapper = mountPageWithNominatedPharmacy(true);
    });

    it('will show pharmacy section', () => {
      expect(wrapper.find('#nominated-pharmacy-section').exists()).toBe(true);
    });

    describe('pharmacy link', () => {
      let nominatedPharmacyLink;

      beforeEach(() => {
        global.digitalData = {};
        nominatedPharmacyLink = wrapper.find('#nominated-pharmacy');
      });

      it('will exist', () => {
        expect(nominatedPharmacyLink.exists()).toEqual(true);
      });

      describe('clicked', () => {
        beforeEach(() => {
          nominatedPharmacyLink.trigger('click');
        });

        it('will track nominated pharmacy search path when no nominated pharmacy is assigned', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(NOMINATED_PHARMACY.path, true);
        });

        it('will redirect to nominated pharmacy search page when no nominated pharmacy is assigned', () => {
          expect($router.push).toHaveBeenCalledWith(NOMINATED_PHARMACY.path);
        });
      });
    });
  });

  describe('nominated pharmacy is not enabled', () => {
    beforeEach(() => {
      wrapper = mountPage(false);
    });

    it('will not show pharmacy section', () => {
      expect(wrapper.find('#nominated-pharmacy-section').exists()).toBe(false);
    });

    describe('order new repeat prescription button', () => {
      let button;

      beforeEach(() => {
        button = wrapper.find('#order-prescription-button');
      });

      describe('clicked', () => {
        beforeEach(() => {
          button.trigger('click');
        });

        it('will track prescriptions repeat courses path', () => {
          expect($store.app.$analytics.trackButtonClick)
            .toHaveBeenCalledWith(PRESCRIPTION_REPEAT_COURSES.path, true);
        });

        it('will redirect to prescriptions repeat courses', () => {
          expect($router.push).toHaveBeenCalledWith(PRESCRIPTION_REPEAT_COURSES.path);
        });
      });
    });
  });
});
