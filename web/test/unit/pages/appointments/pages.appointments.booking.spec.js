import each from 'jest-each';
import BookingPage from '@/pages/appointments/gp-appointments/booking';
import { createStore, mount } from '../../helpers';

const createBookingPage = ({ $route, $store, data }) => mount(BookingPage, {
  $route,
  $style: {
    mainShowingSlots: 'mainShowingSlots',
    warning: 'warning',
    error: 'error',
  },
  $store,
  data,
  methods: {
    reload: jest.fn(),
  },
  stubs: {
    'page-title': '<div></div>',
    'nuxt-link': '<a>Back</a>',
  },
});

describe('booking.vue', () => {
  let $store;
  let state;
  let wrapper;

  beforeEach(() => {
    state = {
      availableAppointments: {
        slots: [],
        filteredSlots: [],
        hasLoaded: true,
        error: null,
      },
      myAppointments: {
        disableCancellation: false,
      },
      device: {
        source: 'web',
      },
    };
    $store = createStore({ state });
    wrapper = createBookingPage({ $store });
  });

  it('will show "no slot message"', () => {
    expect(wrapper.find('.warning').exists()).toBeTruthy();
    expect(wrapper.find('.warning p').text()).toContain('translate_appointments.booking.noAppointmentsAvailable');
  });

  it('will show "not match search criteria message"', () => {
    state.availableAppointments.slots.push({});
    state.availableAppointments.selectedOptions = {
      type: 'appointment',
      location: 'my surgery',
    };

    const data = () => ({
      filters: {
        type: 'Emergency',
        location: 'Leeds',
      },
    });

    wrapper = createBookingPage({ $store, data });
    wrapper.vm.filterSlots();

    expect(wrapper.find('.warning').exists()).toBeTruthy();
    expect(wrapper.findAll('.warning p').at(0).text()).toContain('appointments.booking.adjustSearch.line1');
    expect(wrapper.findAll('.warning p').at(1).text()).toContain('appointments.booking.adjustSearch.line2');
  });

  describe('asyncData', () => {
    describe('query has filter', () => {
      beforeEach(async () => {
        await wrapper.vm.$options.asyncData({
          store: $store,
          req: {
            url: '/foo?time-period=123456',
          },
        });
      });

      it('will dispatch `availableAppointments/init`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/init');
      });

      it('will dispatch `availableAppointments/load`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/load');
      });

      it('will dispatch `availableAppointments/setSelectedFilters` with filter values', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/setSelectedFilters', {
          'time-period': '123456',
          date: '123456',
        });
      });

      it('will dispatch `availableAppointments/filter`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/filter');
      });
    });

    describe('query has no filter', () => {
      beforeEach(async () => {
        await wrapper.vm.$options.asyncData({ store: $store,
          req: {
            url: '/foo?nothing=123456',
          },
        });
      });

      it('will dispatch `availableAppointments/init`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/init');
      });

      it('will dispatch `availableAppointments/load`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/load');
      });

      it('will not dispatch `availableAppointments/setSelectedFilters`', () => {
        expect($store.dispatch).not.toBeCalledWith('availableAppointments/setSelectedFilters', expect.any(Object));
      });

      it('will not dispatch `availableAppointments/filter`', () => {
        expect($store.dispatch).not.toBeCalledWith('availableAppointments/filter');
      });
    });
  });

  describe('watch - $route.query.ts', () => {
    beforeEach(() => {
      wrapper = createBookingPage({ $route: { query: {} }, $store });
    });

    describe('query has filter', () => {
      beforeEach(() => {
        wrapper.setData({
          $route: {
            query: {
              'time-period': 'time-period',
              ts: 'testTs',
            },
          },
        });
      });

      it('will dispatch `availableAppointments/init`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/init');
      });

      it('will dispatch `availableAppointments/load`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/load');
      });
    });

    describe('query has no filter', () => {
      beforeEach(() => {
        wrapper.setData({ $route: { query: { ts: 'testTs' } } });
      });

      it('will dispatch `availableAppointments/init`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/init');
      });

      it('will dispatch `availableAppointments/load`', () => {
        expect($store.dispatch).toBeCalledWith('availableAppointments/load');
      });
    });
  });

  describe('errors', () => {
    beforeEach(() => {
      wrapper = createBookingPage({ $store });
    });

    each([
      403,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      state.availableAppointments.error = { status };
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });
});
