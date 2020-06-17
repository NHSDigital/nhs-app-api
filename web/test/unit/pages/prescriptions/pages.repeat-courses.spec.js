/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import Vue from 'vue';
import { createLocalVue, mount } from '@vue/test-utils';
import RepeatCoursesPage from '@/pages/prescriptions/repeat-courses';
import Necessity from '@/lib/necessity';

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

const createStore = (
  hasLoaded,
  validated,
  specialRequestNecessity,
  specialRequest,
) => ({
  dispatch: jest.fn(() => Promise.resolve()),
  app: {
    router: {
      push: jest.fn(),
    },
    $analytics: {
      validationError: jest.fn(),
    },
  },
  state: {
    repeatPrescriptionCourses: {
      hasLoaded,
      repeatPrescriptionCourses,
      validated,
      specialRequestNecessity,
      specialRequest,
    },
  },
});

const createRepeatCoursesPage = async ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);
  localVue.mixin(createMockMixinPlugin());

  return mount(RepeatCoursesPage, {
    localVue,
    mocks: {
      $http,
      $store,
      $t: jest.fn(),
      showTemplate: () => true,
    },
  });
};

describe('prescriptions/repeat-courses.vue -', () => {
  describe('mounted', () => {
    it('will fetch courses', async () => {
      // Arrange
      const $store = createStore(false);
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
      const $store = createStore(true, true);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
        'repeatPrescriptionCourses/isValid': false,
        'repeatPrescriptionCourses/specialRequestValid': true,
      };

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(true);
    });

    it('will show an error if the special request is invalid and Mandatory', async () => {
      // Arrange
      const $store = createStore(true, true, Necessity.Mandatory);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': ['repeat-course-id-1'],
        'repeatPrescriptionCourses/isValid': true,
        'repeatPrescriptionCourses/specialRequestValid': false,
      };

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(true);
    });

    it('will not show an error if the special request is invalid and not Mandatory', async () => {
      // Arrange
      const $store = createStore(true, [], true, Necessity.Optional);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': ['repeat-course-id-1'],
        'repeatPrescriptionCourses/isValid': true,
        'repeatPrescriptionCourses/specialRequestValid': false,
      };

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(false);
    });

    it('will show an error if the course selection and Mandatory special request are invalid', async () => {
      // Arrange
      const $store = createStore(true, true, Necessity.Mandatory);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
        'repeatPrescriptionCourses/isValid': false,
        'repeatPrescriptionCourses/specialRequestValid': false,
      };

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(true);
    });

    it('will not show an error if the course selection and Mandatory special request are valid', async () => {
      // Arrange
      const $store = createStore(true, true, Necessity.Mandatory, 'specialRequest');
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': ['repeat-course-id-1'],
        'repeatPrescriptionCourses/isValid': true,
        'repeatPrescriptionCourses/specialRequestValid': true,
      };

      // Act
      page = await createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.error).toBe(false);
    });
  });
});
