import BookingPage from '@/pages/appointments/gp-appointments/booking';
import Vue from 'vue';
import { createStore, mount } from '../../helpers';

Vue.mixin({
  methods: {
    hasConnectionProblem() {
      return false;
    },
  },
});

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
  const $style = {
    mainShowingSlots: 'mainShowingSlots',
  };

  return mount(BookingPage, {
    $store,
    $style,
  });
};

describe('showNoMatchingWarning', () => {
  it('will return true when slots has been loaded and are not empty', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments({ slots: [{}], hasLoaded: true }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.availableAppointments).toBeDefined();
    expect(page.vm.availableAppointments).toBeTruthy();
  });

  it('will return false when slots has been loaded and are empty', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments({ hasLoaded: true }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => true,
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.availableAppointments).toBeDefined();
    expect(page.vm.availableAppointments).toBeFalsy();
  });

  it('will return false when slots has not been loaded', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.availableAppointments).toBeDefined();
    expect(page.vm.availableAppointments).toBeFalsy();
  });
});

describe('notMatchSearchCriteria', () => {
  it('will return false when filtered slots are not empty', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments({ slots: [{}], filteredSlots: [[['2018-04-12'], [{}]]] }),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and type has been selected', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and location has been selected', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return false when filtered slots are empty and location and type have not been selected', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments(),
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    const page = createBookingPage($store);
    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeFalsy();
  });

  it('will return true when filtered slots are empty and location and type have been selected', () => {
    const $store = createStore({
      state: {
        availableAppointments: availableAppointments({
          slots: [{}],
          hasLoaded: true,
          selectedOptions: {
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
    });

    const page = createBookingPage($store);
    page.vm.filterSlots();

    expect(page.vm.showNoMatchingWarning).toBeDefined();
    expect(page.vm.showNoMatchingWarning).toBeTruthy();
  });
});
