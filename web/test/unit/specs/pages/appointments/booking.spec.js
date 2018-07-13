/* eslint-disable import/extensions */
/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import BookingPage from '@/pages/appointments/booking';

const $t = key => `translate_${key}`;

const createBookingPage = ($store, data = []) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const $style = {
    mainShowingSlots: 'mainShowingSlots',
  };

  return mount(BookingPage, {
    localVue,
    data,
    mocks: {
      $http,
      $store,
      $t,
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
          slots: new Map(),
          filteredSlots: [],
          hasLoaded: true,
        },
      },
    };

    const page = createBookingPage($store, {});

    expect(page.find('.warning').exists()).toBeTruthy();
    expect(page.find('.warning p').text()).toContain('translate_appointments.booking.noAppointmentsAvailable');
  });

  it('will do not show validation error messages', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map([['2018-04-12'], [{}]]),
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
});
