/* eslint-disable import/no-extraneous-dependencies */
import Vuex from 'vuex';
import BookingPage from '@/pages/appointments/booking';
import { mount, createLocalVue } from '@vue/test-utils';
import { create$T } from '../../helpers';

const $t = create$T();
const $tc = $t;

const availableAppointments = (options = {}) => ({
  slots: options.slots || [],
  filteredSlots: options.filteredSlots || [],
  hasLoaded: options.hasLoaded || false,
  selectedOptions: options.selectedOptions || {
    type: '',
    location: '',
    clinician: '',
    date: '',
  },
});

const createBookingPage = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  const $style = {
    mainShowingSlots: 'mainShowingSlots',
  };

  return mount(BookingPage, {
    localVue,
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
      'page-title': '<div></div>',
    },
  });
};

describe('booking.vue - noAvailableAppointments', () => {
  it('will return true when slots has been loaded and are empty', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ hasLoaded: true }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.noAvailableAppointments).toBeDefined();
    expect(page.vm.noAvailableAppointments).toBeTruthy();
  });

  it('will return false when slots has been loaded and are not empty', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ slots: [{}], hasLoaded: true }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.noAvailableAppointments).toBeDefined();
    expect(page.vm.noAvailableAppointments).toBeFalsy();
  });

  it('will return false when slots has not been loaded', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ slots: new Map() }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.noAvailableAppointments).toBeDefined();
    expect(page.vm.noAvailableAppointments).toBeFalsy();
  });
});

describe('booking.vue - showNoMatchingWarning', () => {
  it('will return true when slots has been loaded and are not empty', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ slots: [{}], hasLoaded: true }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.availableAppointments).toBeDefined();
    expect(page.vm.availableAppointments).toBeTruthy();
  });

  it('will return false when slots has been loaded and are empty', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ hasLoaded: true }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.availableAppointments).toBeDefined();
    expect(page.vm.availableAppointments).toBeFalsy();
  });

  it('will return false when slots has not been loaded', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.availableAppointments).toBeDefined();
    expect(page.vm.availableAppointments).toBeFalsy();
  });
});

describe('booking.vue - notMatchSearchCriteria', () => {
  it('will return false when filtered slots are not empty', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ slots: [{}], filteredSlots: [[['2018-04-12'], [{}]]] }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and type has been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and location has been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and location and type have not been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    };

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return true when filtered slots are empty and location and type have been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: availableAppointments({ selectedOptions: {
          type: 'type',
          location: 'location' },
        }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
      app: {
        $analytics: {
          logicError: jest.fn(),
        },
      },
    };

    const page = createBookingPage($store);
    page.vm.filterSlots();

    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeTruthy();
  });
});
