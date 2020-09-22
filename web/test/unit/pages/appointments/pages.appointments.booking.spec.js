import BookingPage from '@/pages/appointments/gp-appointments/booking';
import i18n from '@/plugins/i18n';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

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

  beforeEach(() => {
    EventBus.$emit.mockClear();
  });

  describe('available appointments has no slots', () => {
    it('will show "no slot message"', async () => {
      createBookingPage();

      await wrapper.vm.$nextTick();

      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'appointments.book.noAppointmentsAvailable');
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'appointments.book.noAppointmentsAvailable');

      const warning = wrapper.find('[data-purpose="no-appointments-warning"]');
      expect(warning.exists()).toBeTruthy();
      expect(warning.findAll('p').at(0).text()).toBe('You\'ll need to contact your GP surgery to book an appointment.');
      expect(warning.findAll('p').at(1).text()).toBe('For urgent medical advice, go to 111.nhs.uk or call 111.');
      expect(warning.findAll('h2').at(0).text()).toBe('If you think you might have coronavirus');
      expect(warning.findAll('p').at(2).text()).toBe('Stay at home and avoid close contact with other people.');
      expect(warning.findAll('p').at(3).text()).toBe('Use the 111 coronavirus service to see if you need medical help.');
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

      const warning = wrapper.find('[data-purpose="no-appointments-matching-filter"]');
      expect(warning.exists()).toBeTruthy();
      expect(warning.find('h2').text()).toBe('No appointments available for your search');
      expect(warning.findAll('p').at(0).text()).toBe('You can choose different filter options, or select "No preference" for the practice member, to show any available appointments.');
      expect(warning.findAll('p').at(1).text()).toBe('If you cannot find the appointment you need, contact your GP surgery.');
      expect(warning.findAll('p').at(2).text()).toBe('For urgent medical advice, go to 111.nhs.uk or call 111.');
    });
  });

  describe('error loading appointments and no slots', () => {
    it('does not update title', async () => {
      createBookingPage({ error: {} });

      await wrapper.vm.$nextTick();

      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_HEADER, 'appointments.book.noAppointmentsAvailable');
      expect(EventBus.$emit).not.toHaveBeenCalledWith(UPDATE_TITLE, 'appointments.book.noAppointmentsAvailable');
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
});
