/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import BookingPage from '@/pages/appointments/booking';

const $t = key => `translate_${key}`;
const $tc = key => `translate_${key}`;

const app = {
  $analytics: {
    logicError: jest.fn(),
  },
};

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
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
      app,
    };

    const page = createBookingPage($store, {});

    expect(page.find('.warning').exists()).toBeTruthy();
    expect(page.find('.warning p').text()).toContain('translate_appointments.booking.noAppointmentsAvailable');
  });

  it('will show "not match search criteria message"', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [],
          hasLoaded: true,
          selectedOptions: {
            type: 'appointment',
            location: 'my surgery',
          },
        },
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
      app,
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
});
