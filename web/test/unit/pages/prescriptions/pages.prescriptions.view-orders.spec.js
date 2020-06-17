/* eslint-disable import/no-extraneous-dependencies */
/* eslint-disable import/first */
import Vuex from 'vuex';
import Vue from 'vue';
import { createLocalVue, mount } from '@vue/test-utils';
import ViewOrders from '@/pages/prescriptions/view-orders';

const mockDependency = (name) => {
  ViewOrders.components[name] = {
    computed: {},
    staticRenderFns: [],
    name,
  };
};

const createMockMixinPlugin = () => Vue.mixin({
  computed: {
    showTemplate: {
      get() {
        return false;
      },
      set() {
      },
    },
  },
  methods: {
    getMedicationCourseStatus() {
      return null;
    },
  },
});

const createStore = hasLoaded => ({
  dispatch: jest.fn(() => Promise.resolve()),
  state: {
    prescriptions: {
      hasLoaded,
      prescriptionCourses: {},
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'Boots',
      },
      nominatedPharmacyEnabled: true,
    },
  },
  getters: {
    'serviceJourneyRules/nominatedPharmacyEnabled': false,
  },
});

const createViewOrdersPrescriptionsPage = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);
  localVue.mixin(createMockMixinPlugin());

  mockDependency('HistoricPrescription');

  return mount(ViewOrders, {
    localVue,
    mocks: {
      $http,
      $store,
      $t: jest.fn(),
      $style: {
        info: 'info',
      },
      showTemplate: () => true,
    },
  });
};

describe('prescriptions/view-orders.vue -', () => {
  describe('page load', () => {
    it('will clear and load the prescriptions and will clear and load nominated pharmacy when enabled via sjr and is not already loaded', async () => {
      const $store = createStore(true);
      $store.getters['serviceJourneyRules/nominatedPharmacyEnabled'] = true;
      $store.state.nominatedPharmacy.hasLoaded = false;
      jest.spyOn($store, 'dispatch');

      await createViewOrdersPrescriptionsPage($store);

      expect($store.dispatch)
        .toHaveBeenCalledWith('prescriptions/clear');
      expect($store.dispatch)
        .toHaveBeenCalledWith('prescriptions/load');
      expect($store.dispatch)
        .toHaveBeenCalledWith('nominatedPharmacy/clear');
      expect($store.dispatch)
        .toHaveBeenCalledWith('nominatedPharmacy/load');
    });

    it('will clear and load the prescriptions but will not clear and load nominated pharmacy when enabled via sjr and is already loaded', async () => {
      const $store = createStore(true);
      $store.getters['serviceJourneyRules/nominatedPharmacyEnabled'] = true;
      $store.state.nominatedPharmacy.hasLoaded = true;
      jest.spyOn($store, 'dispatch');

      createViewOrdersPrescriptionsPage($store);

      expect($store.dispatch)
        .toHaveBeenCalledWith('prescriptions/clear');
      expect($store.dispatch)
        .toHaveBeenCalledWith('prescriptions/load');
      expect($store.dispatch)
        .not
        .toHaveBeenCalledWith('nominatedPharmacy/clear');
      expect($store.dispatch)
        .not
        .toHaveBeenCalledWith('nominatedPharmacy/load');
    });

    it('will clear and load prescriptions, but will not clear and load nominated pharmacy when not enabled via sjr', async () => {
      const $store = createStore(true);
      jest.spyOn($store, 'dispatch');

      createViewOrdersPrescriptionsPage($store);

      expect($store.dispatch).toHaveBeenCalledWith('prescriptions/clear');
      expect($store.dispatch).toHaveBeenCalledWith('prescriptions/load');
      expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/clear');
      expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/load');
    });
  });

  describe('showNoPrescriptions', () => {
    it('will show the no prescriptions banner after data loading is complete.', () => {
      const $store = createStore(true);

      const page = createViewOrdersPrescriptionsPage($store);
      expect(page.vm.showNoPrescriptions)
        .toBe(true);
    });

    it('will not show the no prescriptions and yield true as data not loaded yet.', () => {
      const $store = createStore(false);

      const page = createViewOrdersPrescriptionsPage($store);
      expect(page.vm.showNoPrescriptions)
        .toBe(false);
    });

    it('will show prescriptions after loading as there is data to show.', () => {
      const $store = createStore(true);
      $store.state.prescriptions.prescriptionCourses.Approved = {};
      const page = createViewOrdersPrescriptionsPage($store);
      expect(page.vm.showNoPrescriptions)
        .toBe(false);
    });
  });


  describe('showPrescriptions', () => {
    it('will show prescriptions after data loading is complete.', () => {
      const $store = createStore(true);
      $store.state.prescriptions.prescriptionCourses.Approved = {};

      const page = createViewOrdersPrescriptionsPage($store);
      expect(page.vm.showPrescriptions)
        .toBe(true);
    });

    it('will not show prescriptions as the prescriptions have not been loaded yet.', () => {
      const $store = createStore(false);
      $store.state.prescriptions.prescriptionCourses.Approved = {};

      const page = createViewOrdersPrescriptionsPage($store);
      expect(page.vm.showPrescriptions)
        .toBe(false);
    });

    it('will not show prescriptions as there is no data to show.', () => {
      const $store = createStore(true);
      const page = createViewOrdersPrescriptionsPage($store);
      expect(page.vm.showPrescriptions)
        .toBe(false);
    });
  });

  describe('showPrescriptionsInOrder', () => {
    it('will show prescriptions as returned by backend without status or date information', async () => {
      const myStore = hasLoaded => ({
        dispatch: jest.fn(() => Promise.resolve()),
        state: {
          prescriptions: {
            hasLoaded,
            prescriptionCourses: [
              {
                courseId: 'abc',
              },
              {
                courseId: 'xyz',
              },
              {
                courseId: 'pqr',
              },
            ],
          },
          nominatedPharmacy: {
            pharmacy: {},
            nominatedPharmacyEnabled: true,
          },
        },
        getters: {
          'serviceJourneyRules/nominatedPharmacyEnabled': false,
        },
      });

      const $store = myStore(true);

      jest.spyOn($store, 'dispatch');
      const page = createViewOrdersPrescriptionsPage($store);

      expect(page.vm.prescriptionCoursesToDisplay)
        .toEqual([
          { courseId: 'abc', statusDisplayPriority: undefined },
          { courseId: 'xyz', statusDisplayPriority: undefined },
          { courseId: 'pqr', statusDisplayPriority: undefined },
        ]);
    });

    it('will show prescriptions as ordered by date desc and then status in terms of priority', async () => {
      const myStore = hasLoaded => ({
        dispatch: jest.fn(() => Promise.resolve()),
        state: {
          prescriptions: {
            hasLoaded,
            prescriptionCourses: [
              {
                courseId: 'def',
                status: 'Approved',
                orderDate: '2020-10-01T00:00:00+00:00',
              },
              {
                courseId: 'abc',
                status: 'Approved',
                orderDate: '2019-10-01T00:00:00+00:00',
              },
              {
                courseId: 'xyz',
                status: 'Rejected',
                orderDate: '2019-10-01T00:00:00+00:00',
              },
              {
                courseId: 'pqr',
                status: 'Requested',
                orderDate: '2019-10-01T00:00:00+00:00',
              },
            ],
          },
          nominatedPharmacy: {
            pharmacy: {},
            nominatedPharmacyEnabled: true,
          },
        },
        getters: {
          'serviceJourneyRules/nominatedPharmacyEnabled': false,
        },
      });

      const $store = myStore(true);

      jest.spyOn($store, 'dispatch');
      const page = createViewOrdersPrescriptionsPage($store);

      expect(page.vm.prescriptionCoursesToDisplay)
        .toEqual([
          { courseId: 'def', orderDate: '2020-10-01T00:00:00+00:00', statusDisplayPriority: 2, status: 'Approved' },
          { courseId: 'xyz', orderDate: '2019-10-01T00:00:00+00:00', statusDisplayPriority: 1, status: 'Rejected' },
          { courseId: 'abc', orderDate: '2019-10-01T00:00:00+00:00', statusDisplayPriority: 2, status: 'Approved' },
          { courseId: 'pqr', orderDate: '2019-10-01T00:00:00+00:00', statusDisplayPriority: 3, status: 'Requested' },
        ]);
    });
  });
});
