import BookingPage from '@/pages/appointments/gp-appointments/booking';
import each from 'jest-each';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../../helpers';

describe('booking.vue', () => {
  let wrapper;
  let $store;

  const createBookingPage = ({
    slots = [],
    selectedOptions,
    queryName = 'nothing',
    error = undefined,
  } = {}) => {
    $store = createStore({
      state: {
        availableAppointments: {
          selectedOptions,
          slots,
          filteredSlots: [],
          hasLoaded: true,
          error,
        },
        myAppointments: {
          disableCancellation: false,
        },
        device: {
          source: 'web',
        },
      },
    });

    wrapper = mount(BookingPage, {
      $route: {
        path: '/foo',
        query: {
          [queryName]: '123456',
        },
      },
      $style: {
        mainShowingSlots: 'mainShowingSlots',
        warning: 'warning',
        error: 'error',
      },
      $store,
      methods: {
        reload: jest.fn(),
      },
      stubs: {
        'page-title': '<div></div>',
      },
      mountOpts: {
        i18n,
      },
    });
  };

  describe('available appointments has no slots', () => {
    it('will show "no slot message"', () => {
      createBookingPage();

      expect(wrapper.find('.warning').exists()).toBeTruthy();
      expect(wrapper.find('.warning p').text()).toContain('No appointments available');
    });
  });

  describe('no matching slots', () => {
    it('will show "not match search criteria message"', () => {
      const slots = [{}];
      const selectedOptions = {
        type: 'appointment',
        location: 'my surgery',
      };

      createBookingPage({ slots, selectedOptions });
      wrapper.vm.filterSlots();

      expect(wrapper.find('.warning').exists()).toBeTruthy();
      expect(wrapper.findAll('.warning p').at(0).text()).toBe('Try to filter appointments by a different period or select "No preference" for the practice member. If you cannot find the appointment you need, call your GP surgery.');
      expect(wrapper.findAll('.warning p').at(1).text()).toBe('If it\'s urgent and you do not know what to do, call 111 to get help near you.');
    });
  });

  describe('created', () => {
    describe('query has filter', () => {
      beforeEach(() => {
        createBookingPage({ queryName: 'time-period' });
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
      beforeEach(() => {
        createBookingPage();
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
        createBookingPage({ queryName: 'time-period' });
        wrapper.setData({ $route: { query: { ts: 'testTs' } } });
      });

      it('will dispatch `availableAppointments/init`', () => {
        expect($store.dispatch).toHaveBeenNthCalledWith(3, 'availableAppointments/init');
      });

      it('will dispatch `availableAppointments/load`', () => {
        expect($store.dispatch).toHaveBeenNthCalledWith(4, 'availableAppointments/load');
      });
    });

    describe('query has no filter', () => {
      beforeEach(() => {
        createBookingPage();
        wrapper.setData({ $route: { query: { ts: 'testTs' } } });
      });

      it('will dispatch `availableAppointments/init`', () => {
        expect($store.dispatch).toHaveBeenNthCalledWith(3, 'availableAppointments/init');
      });

      it('will dispatch `availableAppointments/load`', () => {
        expect($store.dispatch).toHaveBeenNthCalledWith(4, 'availableAppointments/load');
      });
    });
  });

  describe('errors', () => {
    each([
      403,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      createBookingPage({ error: { status } });
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });
});
