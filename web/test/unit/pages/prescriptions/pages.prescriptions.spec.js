/* eslint-disable import/no-extraneous-dependencies */
/* eslint-disable import/first */
import Vuex from 'vuex';
import Vue from 'vue';
import { createLocalVue, mount } from '@vue/test-utils';
import PrescriptionsPage from '@/pages/prescriptions/index';

const mockDependency = (name) => {
  PrescriptionsPage.components[name] = {
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
      pharmacy: {},
      nominatedPharmacyEnabled: true,
    },
  },
});

const createPrescriptionsPage = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);
  localVue.mixin(createMockMixinPlugin());

  mockDependency('HistoricPrescription');

  return mount(PrescriptionsPage, {
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
    stubs: {
      'nuxt-link': '<a>Back</a>',
    },
  });
};

describe('prescriptions/index.vue -', () => {
  describe('asyncData', () => {
    it('will clear and load the prescriptions', async () => {
      const $store = createStore(true);

      jest.spyOn($store, 'dispatch');

      const page = createPrescriptionsPage($store);
      await page.vm.$options.asyncData({ store: $store });

      expect($store.dispatch).toHaveBeenCalledWith('prescriptions/clear');
      expect($store.dispatch).toHaveBeenCalledWith('prescriptions/load');
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/clear');
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/load');
    });
  });

  describe('showNoPrescriptions', () => {
    it('will show the no prescriptions banner after data loading is complete.', () => {
      const $store = createStore(true);
      const page = createPrescriptionsPage($store);
      expect(page.vm.showNoPrescriptions).toBe(true);
    });

    it('will not show the no prescriptions and yield true as data not loaded yet.', () => {
      const $store = createStore(false);
      const page = createPrescriptionsPage($store);
      expect(page.vm.showNoPrescriptions).toBe(false);
    });

    it('will show prescriptions after loading as there is data to show.', () => {
      const $store = createStore(true);
      $store.state.prescriptions.prescriptionCourses.Approved = {};
      const page = createPrescriptionsPage($store);
      expect(page.vm.showNoPrescriptions).toBe(false);
    });
  });

  describe('showPrescriptions', () => {
    it('will show prescriptions after data loading is complete.', () => {
      const $store = createStore(true);
      $store.state.prescriptions.prescriptionCourses.Approved = {};

      const page = createPrescriptionsPage($store);
      expect(page.vm.showPrescriptions).toBe(true);
    });

    it('will not show prescriptions as the prescriptions have not been loaded yet.', () => {
      const $store = createStore(false);
      $store.state.prescriptions.prescriptionCourses.Approved = {};

      const page = createPrescriptionsPage($store);
      expect(page.vm.showPrescriptions).toBe(false);
    });

    it('will not show prescriptions as there is no data to show.', () => {
      const $store = createStore(true);
      const page = createPrescriptionsPage($store);
      expect(page.vm.showPrescriptions).toBe(false);
    });
  });

  describe('showPrescriptionsInOrder', () => {
    it('will show prescriptions ordered by priority.', async () => {
      const myStore = hasLoaded => ({
        dispatch: jest.fn(() => Promise.resolve()),
        state: {
          prescriptions: {
            hasLoaded,
            prescriptionCourses: {
              Approved: [{
                courseId: 'abc',
              }],
              Requested: [{
                courseId: 'pqr',
              }],
              Rejected: [{
                courseId: 'xyz',
              }],
            },
          },
          nominatedPharmacy: {
            pharmacy: {},
            nominatedPharmacyEnabled: true,
          },
        },
      });

      const $store = myStore(true);
      jest.spyOn($store, 'dispatch');
      const page = createPrescriptionsPage($store);

      expect(page.vm.prescriptionCoursesToDisplay).toEqual({
        Requested: [{ courseId: 'pqr' }],
        Approved: [{ courseId: 'abc' }],
        Rejected: [{ courseId: 'xyz' }],
      });
    });
  });
});

