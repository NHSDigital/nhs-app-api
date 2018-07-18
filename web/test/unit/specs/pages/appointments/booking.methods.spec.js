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

  const $router = {
    push: jest.fn(),
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
      $router,
    },
    stubs: {
      'nuxt-link': '<a>Back</a>',
    },
  });
};

describe('booking.vue - validate', () => {
  it('will return false when filters have not been initialized', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: null,
          selectedOptions: {
            location: '',
          },
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeTruthy();
    expect(page.vm.validationError.isTypeValid).toBeFalsy();
    expect(page.vm.validationError.isLocationValid).toBeFalsy();
  });

  it('will return false when filters have not been initialized  and default location has been set', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: null,
          selectedOptions: {
            location: 'Leeds',
          },
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeTruthy();
    expect(page.vm.validationError.isTypeValid).toBeFalsy();
    expect(page.vm.validationError.isLocationValid).toBeTruthy();
  });

  it('will return false when filters have not been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: null,
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
      filters: {
        type: '',
        location: '',
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeTruthy();
    expect(page.vm.validationError.isTypeValid).toBeFalsy();
    expect(page.vm.validationError.isLocationValid).toBeFalsy();
  });

  it('will return false when type has not been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: null,
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
      filters: {
        type: '',
        location: 'Leeds',
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeTruthy();
    expect(page.vm.validationError.isTypeValid).toBeFalsy();
    expect(page.vm.validationError.isLocationValid).toBeTruthy();
  });

  it('will return false when location has not been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: null,
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
      filters: {
        type: 'Emergency',
        location: '',
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeTruthy();
    expect(page.vm.validationError.isTypeValid).toBeTruthy();
    expect(page.vm.validationError.isLocationValid).toBeFalsy();
  });

  it('will return false when filter has been selected but slot has not been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: null,
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
      filters: {
        type: 'Emergency',
        location: 'Leeds',
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeTruthy();
    expect(page.vm.validationError.isTypeValid).toBeTruthy();
    expect(page.vm.validationError.isLocationValid).toBeTruthy();
  });

  it('will return true when filter has been selected ans slot has been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          selectedSlot: {},
        },
      },
    };

    const data = {
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
      filters: {
        type: 'Emergency',
        location: 'Leeds',
      },
    };

    const page = createBookingPage($store, data);
    page.vm.validate();

    expect(page.vm.showValidationError).toBeFalsy();
    expect(page.vm.validationError.isTypeValid).toBeTruthy();
    expect(page.vm.validationError.isLocationValid).toBeTruthy();
  });
});
