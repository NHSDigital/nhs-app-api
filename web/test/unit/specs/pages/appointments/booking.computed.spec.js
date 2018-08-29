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

describe('booking.vue - noAvailableAppointments', () => {
  it('will return true when slots has been loaded and are empty', () => {
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
    expect(page.vm.noAvailableAppointments).toBeTruthy();
  });

  it('will return false when slots has been loaded and are not empty', () => {
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
    expect(page.vm.noAvailableAppointments).toBeFalsy();
  });

  it('will return false when slots has not been loaded', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: new Map(),
          filteredSlots: [],
          hasLoaded: false,
        },
      },
    };

    const page = createBookingPage($store, {});
    expect(page.vm.noAvailableAppointments).toBeFalsy();
  });
});

describe('booking.vue - showNoMatchingWarning', () => {
  it('will return true when slots has been loaded and are not empty', () => {
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
    expect(page.vm.availableAppointments).toBeTruthy();
  });

  it('will return false when slots has been loaded and are empty', () => {
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
    expect(page.vm.availableAppointments).toBeFalsy();
  });

  it('will return false when slots has not been loaded', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [],
          filteredSlots: [],
          hasLoaded: false,
        },
      },
    };

    const page = createBookingPage($store, {});
    expect(page.vm.availableAppointments).toBeFalsy();
  });
});

describe('booking.vue - notMatchSearchCriteria', () => {
  it('will return false when filtered slots are not empty', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [{}],
          filteredSlots: [[['2018-04-12'], [{}]]],
        },
      },
    };

    const page = createBookingPage($store, {});
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and type has been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [],
          filteredSlots: [],
        },
      },
    };

    const data = {
      filters: {
        type: 'Emergency',
        location: '',
      },
    };

    const page = createBookingPage($store, data);
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and location has been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [],
          filteredSlots: [],
        },
      },
    };

    const data = {
      filters: {
        type: '',
        location: 'Leeds',
      },
    };

    const page = createBookingPage($store, data);
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and location and type have not been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [],
          filteredSlots: [],
        },
      },
    };

    const data = {
      filters: {
        type: '',
        location: '',
      },
    };

    const page = createBookingPage($store, data);
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return true when filtered slots are empty and location and type have been selected', () => {
    const $store = {
      dispatch: jest.fn(),
      state: {
        availableAppointments: {
          slots: [],
          filteredSlots: [],
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

    expect(page.vm.showNoMatchingWarning).toBeTruthy();
  });
});
