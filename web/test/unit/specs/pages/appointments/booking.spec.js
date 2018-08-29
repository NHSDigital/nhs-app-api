/* eslint-disable import/extensions */
/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import BookingPage from '@/pages/appointments/booking';

const $t = key => `translate_${key}`;
const $tc = key => `translate_${key}`;

const createBookingPage = ($store, data = []) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const $style = {
    mainShowingSlots: 'mainShowingSlots',
    warning: 'warning',
    error: 'error',
  };

  return mount(BookingPage, {
    localVue,
    data,
    mocks: {
      $http,
      $store,
      $t,
      $tc,
      $style,
      showTemplate: () => true,
    },
    stubs: {
      'nuxt-link': '<a>Back</a>',
    },
  });
};


describe('booking.vue', () => {
  it('will show "no slot message"', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const page = createBookingPage($store, {});

    expect(page.find('.warning').exists()).toBeTruthy();
    expect(page.find('.warning p').text()).toContain('translate_appointments.booking.noAppointmentsAvailable');
  });

  it('will show "mot match search criteria message"', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const data = {
      filters: {
        type: 'Emergency',
        location: 'Leeds',
      },
    };

    const page = createBookingPage($store, data);
    page.vm.filterSlots();

    expect(page.find('.warning').exists()).toBeTruthy();
    expect(page.findAll('.warning p').at(0).text()).toContain('appointments.booking.adjustSearch.line1');
    expect(page.findAll('.warning p').at(1).text()).toContain('appointments.booking.adjustSearch.line2');
  });

  it('will do not show validation error messages', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const page = createBookingPage($store, {});

    expect(page.find('.warning').exists()).toBeFalsy();
    expect(page.vm.showValidationError).toBeFalsy();
    expect(page.vm.validationError.isTypeValid).toBeTruthy();
    expect(page.vm.validationError.isLocationValid).toBeTruthy();
  });

  it('will show validation errors', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const data = {
      showValidationError: true,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
    };

    const page = createBookingPage($store, data);

    expect(page.find('.error').exists()).toBeTruthy();
    expect(page.findAll('.error p').length).toEqual(1);
    expect(page.findAll('.error ul li').length).toEqual(1);
    expect(page.findAll('.error p').at(0).text()).toContain('appointments.booking.validationErrors.problemFound');
    expect(page.findAll('.error ul li').at(0).text()).toContain('appointments.booking.validationErrors.slot');
  });

  it('will show validation error when type is not selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const data = {
      showValidationError: true,
      validationError: {
        isTypeValid: false,
        isLocationValid: true,
      },
    };

    const page = createBookingPage($store, data);

    expect(page.find('.error').exists()).toBeTruthy();
    expect(page.findAll('.error p').length).toEqual(1);
    expect(page.findAll('.error ul li').length).toEqual(2);
    expect(page.findAll('.error p').at(0).text()).toContain('appointments.booking.validationErrors.problemFound');
    expect(page.findAll('.error ul li').at(0).text()).toContain('appointments.booking.validationErrors.type');
    expect(page.findAll('.error ul li').at(1).text()).toContain('appointments.booking.validationErrors.slot');
  });

  it('will show validation error when location is not selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const data = {
      showValidationError: true,
      validationError: {
        isTypeValid: true,
        isLocationValid: false,
      },
    };

    const page = createBookingPage($store, data);

    expect(page.find('.error').exists()).toBeTruthy();
    expect(page.findAll('.error p').length).toEqual(1);
    expect(page.findAll('.error ul li').length).toEqual(2);
    expect(page.findAll('.error p').at(0).text()).toContain('appointments.booking.validationErrors.problemFound');
    expect(page.findAll('.error ul li').at(0).text()).toContain('appointments.booking.validationErrors.location');
    expect(page.findAll('.error ul li').at(1).text()).toContain('appointments.booking.validationErrors.slot');
  });

  it('will show validation error when type and location are not selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const data = {
      showValidationError: true,
      validationError: {
        isTypeValid: false,
        isLocationValid: false,
      },
    };

    const page = createBookingPage($store, data);

    expect(page.find('.error').exists()).toBeTruthy();
    expect(page.findAll('.error p').length).toEqual(1);
    expect(page.findAll('.error ul li').length).toEqual(3);
    expect(page.findAll('.error p').at(0).text()).toContain('appointments.booking.validationErrors.problemFound');
    expect(page.findAll('.error ul li').at(0).text()).toContain('appointments.booking.validationErrors.type');
    expect(page.findAll('.error ul li').at(1).text()).toContain('appointments.booking.validationErrors.location');
    expect(page.findAll('.error ul li').at(2).text()).toContain('appointments.booking.validationErrors.slot');
  });
});
