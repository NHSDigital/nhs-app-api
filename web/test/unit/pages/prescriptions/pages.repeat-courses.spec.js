/* eslint-disable import/no-extraneous-dependencies */
import each from 'jest-each';
import Necessity from '@/lib/necessity';
import RepeatCoursesPage from '@/pages/prescriptions/repeat-courses';
import { PRESCRIPTION_REPEAT_COURSES_PATH } from '@/router/paths';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

const repeatPrescriptionCourses = [
  { id: 'repeat-course-id-1' },
  { id: 'repeat-course-id-2' },
  { id: 'repeat-course-id-3' },
];

const createStore = ({
  hasLoaded = true,
  validated,
  specialRequestNecessity = Necessity.Mandatory,
  specialRequestCharacterLimit = 1000,
  specialRequest,
  error,
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
      specialRequestCharacterLimit,
      specialRequest,
      error,
    },
    nominatedPharmacy: {},
    session: {
      hasRetried: false,
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

const createRepeatCoursesPage = ($store) => {
  const $route = {
    query: {
      hr: true,
    },
    path: PRESCRIPTION_REPEAT_COURSES_PATH,
  };

  return mount(RepeatCoursesPage, {
    methods: {
      reload: jest.fn(),
    },
    mocks: {
      $route,
      $store,
      $style: {
        msg: 'mock msg',
      },
    },
  });
};

let page;

describe('prescriptions/repeat-courses.vue -', () => {
  beforeEach(() => {
    EventBus.$emit.mockClear();
  });

  describe('mounted', () => {
    it('will fetch courses', async () => {
      // Arrange
      const $store = createStore({ hasLoaded: false });
      jest.spyOn($store, 'dispatch');

      // Act
      createRepeatCoursesPage($store);

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/load');
    });
  });

  describe('special request', () => {
    each([500, 800]).it('will indicate there are %s characters remaining when set as the limit', async (limit) => {
      const $store = createStore({
        specialRequest: '',
        specialRequestCharacterLimit: limit,
      });

      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      expect(page.find('p#specialRequestCharactersRemaining').text()).toEqual(`You have ${limit} characters remaining.`);
    });

    each([
      ['0 characters', '20 characters long!!'],
      ['1 character', '19 characters long.'],
      ['2 characters', '18 characters long'],
    ]).it(
      'will indicate there is %s remaining when the limit is 20 and the request is "%s"',
      async (remainingCharacters, request) => {
        const $store = createStore({
          specialRequest: request,
          specialRequestCharacterLimit: 20,
        });

        page = createRepeatCoursesPage($store);
        await page.vm.$nextTick();

        expect(page.find('p#specialRequestCharactersRemaining').text()).toEqual(`You have ${remainingCharacters} remaining.`);
      },
    );

    each([
      [' multiple\nnew\nlines', 'multiple new lines'],
      ['  more\n\rnew\r lines\r\n\r', 'more new lines'],
      ['lots    of \r\n spaces    ', 'lots of spaces'],
    ]).it('will trim and normalise white space in the special request before dispatching to store', async (request, formattedRequest) => {
      // Arrange
      const $store = createStore({
        isValid: true,
        specialRequestValid: true,
        specialRequest: request,
      });
      jest.spyOn($store, 'dispatch');

      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      // Act
      page.vm.validate();
      await page.vm.$nextTick();

      // Assert
      expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/updateAdditionalInfo', {
        specialRequest: formattedRequest,
      });
    });

    it('will not have an aria live attribute on the special request character limit by default', () => {
      const $store = createStore({
        isValid: true,
        specialRequestValid: true,
      });

      // Act
      page = createRepeatCoursesPage($store);

      // Assert
      expect(page.vm.specialRequestAriaLive).toEqual('');
      expect(page.find('p#specialRequestCharactersRemaining').attributes()['aria-live']).toEqual('');
    });

    it('will have a polite aria live attribute on the special request character limit when special request input is focused', async () => {
      const $store = createStore({
        isValid: true,
        specialRequestValid: true,
      });
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      // Act
      page.find('#specialRequest').trigger('focus');

      // Assert
      expect(page.vm.specialRequestAriaLive).toEqual('polite');
      expect(page.find('p#specialRequestCharactersRemaining').attributes()['aria-live']).toEqual('polite');
    });

    it('will have a polite aria live attribute on the special request character limit when special request input is focused', async () => {
      const $store = createStore({
        isValid: true,
        specialRequestValid: true,
      });
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      // Act
      page.find('#specialRequest').trigger('focus');

      // Assert
      expect(page.vm.specialRequestAriaLive).toEqual('polite');
      expect(page.find('p#specialRequestCharactersRemaining').attributes()['aria-live']).toEqual('polite');
    });
  });

  describe('error', () => {
    it('will show an error if the course selection is invalid', async () => {
      // Arrange
      const $store = createStore({
        validated: true,
        specialRequestValid: true,
      });
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      // Act
      page.vm.validate();
      await page.vm.$nextTick();

      // Assert
      expect(page.vm.error).toBe(true);
      expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
    });

    it('will show an error if the special request is invalid and Mandatory', async () => {
      // Arrange
      const $store = createStore({
        submitted: false,
        validated: true,
        isValid: true,
        specialRequestIds: ['repeat-course-id-1'],
      });
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      // Act
      page.vm.validate();
      await page.vm.$nextTick();

      // Assert
      expect(page.vm.error).toBe(true);
      expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
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
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

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
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

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
      page = createRepeatCoursesPage($store);
      await page.vm.$nextTick();

      // Assert
      expect(page.vm.error).toBe(false);
    });
  });
});
