/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import Vue from 'vue';
import { createLocalVue, mount } from '@vue/test-utils';
import RepeatCoursesPage from '@/pages/prescriptions/repeat-courses';
import { PRESCRIPTION_CONFIRM_COURSES } from '@/lib/routes';

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

const createStore = (hasLoaded, submitted, selectedCoursesNoJs) => ({
  dispatch: jest.fn(() => Promise.resolve()),
  app: {
    router: {
      push: jest.fn(),
    },
  },
  state: {
    repeatPrescriptionCourses: {
      hasLoaded,
      submitted,
      repeatPrescriptionCourses,
      selectedCoursesNoJs,
    },
  },
});

const createRepeatCoursesPage = ($store) => {
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
    stubs: {
      'nuxt-link': '<a>Back</a>',
    },
  });
};

describe('prescriptions/repeat-courses.vue -', () => {
  describe('fetch', () => {
    it('will fetch courses when not being submitted', async () => {
      const $store = createStore(false, false, []);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
      };

      jest.spyOn($store, 'dispatch');

      // act
      const page = createRepeatCoursesPage($store);
      await page.vm.$options.fetch({ store: $store });

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/load');
    });

    it('will parse and validate values from the store when the page has been submitted without javascript - multiple courses selected', async () => {
      // arrange
      const selectedCoursesNoJs = ['repeat-course-id-1'];

      const $store = createStore(false, true, selectedCoursesNoJs);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
        'repeatPrescriptionCourses/isValid': true,
        'repeatPrescriptionCourses/specialRequestValid': true,
      };

      jest.spyOn($store, 'dispatch');
      jest.spyOn($store.app.router, 'push');

      // act
      const page = createRepeatCoursesPage($store);
      await page.vm.$options.fetch({ store: $store });

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/load');
      expect($store.app.router.push).toHaveBeenCalledWith(PRESCRIPTION_CONFIRM_COURSES.path);
      expect($store.state.repeatPrescriptionCourses.submitted).toBe(false);
    });

    it('will parse and validate values from the store when the page has been submitted without javascript - one course selected', async () => {
      // arrange
      const selectedCoursesNoJs = 'repeat-course-id-1'; // when submitted with no js, the submitted value is a single string (not an array)

      const $store = createStore(false, true, selectedCoursesNoJs);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
        'repeatPrescriptionCourses/isValid': true,
        'repeatPrescriptionCourses/specialRequestValid': true,
      };

      jest.spyOn($store, 'dispatch');
      jest.spyOn($store.app.router, 'push');

      // act
      const page = createRepeatCoursesPage($store);
      await page.vm.$options.fetch({ store: $store });

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/load');
      expect($store.app.router.push).toHaveBeenCalledWith(PRESCRIPTION_CONFIRM_COURSES.path);
      expect($store.state.repeatPrescriptionCourses.submitted).toBe(false);
    });

    it('will parse and validate values from the store when the page has been submitted without javascript - no course selected', async () => {
      // arrange
      const selectedCoursesNoJs = '';

      const $store = createStore(false, true, selectedCoursesNoJs);
      $store.getters = {
        'repeatPrescriptionCourses/selectedIds': [],
        'repeatPrescriptionCourses/isValid': false,
        'repeatPrescriptionCourses/specialRequestValid': true,
      };

      jest.spyOn($store, 'dispatch');
      jest.spyOn($store.app.router, 'push');

      // act
      const page = createRepeatCoursesPage($store);
      await page.vm.$options.fetch({ store: $store });

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/load');
      expect($store.app.router.push).not.toHaveBeenCalledWith(PRESCRIPTION_CONFIRM_COURSES.path);
      expect($store.state.repeatPrescriptionCourses.submitted).toBe(false);
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/validate', { isValid: false, submitted: true });
    });
  });
});
