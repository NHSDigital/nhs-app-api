/* eslint-disable import/no-extraneous-dependencies */
import Necessity from '@/lib/necessity';
import RepeatCoursesPage from '@/pages/prescriptions/repeat-courses';
import Vue from 'vue';
import Vuex from 'vuex';
import { createLocalVue } from '@vue/test-utils';
import { PRESCRIPTION_REPEAT_COURSES_PATH } from '@/router/paths';
import { shallowMount } from '../../helpers';

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
});

const repeatPrescriptionCourses = [
  { id: 'repeat-course-id-1' },
  { id: 'repeat-course-id-2' },
  { id: 'repeat-course-id-3' },
];

const createStore = ({
  hasLoaded = true,
  validated,
  specialRequestNecessity = Necessity.Mandatory,
  specialRequest,
  error,
  hasRetried = false,
  specialRequestIds = [],
  isValid = false,
  specialRequestValid = false,
} = {}) => ({
  dispatch: jest.fn(() => Promise.resolve()),
  app: {
    router: {
      push: jest.fn(),
    },
    $analytics: {
      validationError: jest.fn(),
    },
  },
  $analytics: {
    validationError: jest.fn(),
  },
  state: {
    repeatPrescriptionCourses: {
      hasLoaded,
      repeatPrescriptionCourses,
      validated,
      specialRequestNecessity,
      specialRequest,
      error,
    },
    nominatedPharmacy: {},
    session: {
      hasRetried,
    },
    device: {
      isNativeApp: false,
    },
  },
  getters: {
    'repeatPrescriptionCourses/selectedIds': specialRequestIds,
    'repeatPrescriptionCourses/isValid': isValid,
    'repeatPrescriptionCourses/specialRequestValid': specialRequestValid,
    'session/isLoggedIn': () => true,
  },
});

const createRepeatCoursesPage = async ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);
  localVue.mixin(createMockMixinPlugin());

  const $route = {
    query: {
      hr: true,
    },
    path: PRESCRIPTION_REPEAT_COURSES_PATH,
  };

  return shallowMount(RepeatCoursesPage, {
    localVue,
    methods: {
      reload: jest.fn(),
    },
    mocks: {
      $http,
      $route,
      $store,
      $style: {
        msg: 'mock msg',
      },
      showTemplate: () => true,
    },
  });
};

describe('prescriptions/repeat-courses.vue -', () => {
  describe('mounted', () => {
    it('will fetch courses', async () => {
      // Arrange
      const $store = createStore({ hasLoaded: false });
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
      };

      jest.spyOn($store, 'dispatch');

      // Act
      await createRepeatCoursesPage($store);

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/load');
    });
  });

  describe('error', () => {
    let page;

    it('will show an error if the course selection is invalid', async () => {
      // Arrange
      const $store = createStore({
        validated: true,
        specialRequestValid: true,
      });

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(true);
    });

    it('will show an error if the special request is invalid and Mandatory', async () => {
      // Arrange
      const $store = createStore({
        submitted: false,
        validated: true,
        isValid: true,
        specialRequestIds: ['repeat-course-id-1'],
      });

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(true);
    });

    it('will not show an error if the special request is invalid and not Mandatory', async () => {
      // Arrange
      const $store = createStore({
        validated: true,
        specialRequestNecessity: Necessity.Optional,
        selectedIds: ['repeat-course-id-1'],
        isValid: true,
        specialRequestValid: false,
      });

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(false);
    });

    it('will show an error if the course selection and Mandatory special request are invalid', async () => {
      // Arrange
      const $store = createStore({
        validated: true,
        isValid: false,
        seletedIds: [],
        specialRequestValid: false,
      });

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(true);
    });

    it('will not show an error if the course selection and Mandatory special request are valid', async () => {
      // Arrange
      const $store = createStore({
        validated: true,
        specialRequest: 'specialRequest',
        selectedIds: ['repeat-course-id-1'],
        isValid: true,
        specialRequestValid: true,
      });

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(false);
    });
  });
});
