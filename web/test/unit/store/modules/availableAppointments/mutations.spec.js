import each from 'jest-each';
import mutations from '@/store/modules/availableAppointments/mutations';
import DateProvider from '@/services/DateProvider';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import FilterDataProvider from './mutation/FilterDataProvider';

const callLoadMutation = (state, data, sixteenWeeksSlotsEnabled = true) => {
  mutations.LOAD.call({
    $env: { SIXTEEN_WEEKS_SLOTS_ENABLED: sixteenWeeksSlotsEnabled },
  }, state, data);
};

describe('mutations', () => {
  describe('ADD_ERROR', () => {
    const error = {
      status: 500,
      serviceDeskReference: 'xxx',
    };
    let state;

    beforeEach(() => {
      state = {
        error: null,
      };
      mutations.ADD_ERROR(state, error);
    });

    it('will set error to the passed value', () => {
      expect(state.error).toBe(error);
    });
  });

  describe('CLEAR', () => {
    it('will set default values', () => {
      const slot = { id: 1 };
      const slots = [slot];
      const state = {
        slots,
        filteredSlots: [['2018-12-01', [slot]]],
        hasLoaded: true,
        selectedSlot: slot,
        booked: true,
        error: { status: 500 },
        filtersOptions: {
          types: ['option 1', 'options 2'],
          locations: ['option 1', 'options 2'],
          clinicians: ['option 1', 'options 2'],
          dates: ['option 1', 'options 2'],
        },
        selectedOptions: {
          type: 'Emergency',
          location: 'Leeds',
          clinician: 'Dr Who',
          date: DateFilterValues.ALL,
        },
      };

      const expectedState = {
        slots: [],
        filteredSlots: [],
        telephoneNumbers: [],
        hasLoaded: false,
        selectedSlot: slot,
        booked: true,
        error: null,
        filtersOptions: {
          types: [],
          locations: [],
          clinicians: [],
          dates: [],
        },
        selectedOptions: {
          type: '',
          location: '',
          clinician: '',
          date: DateFilterValues.ALL,
        },
      };

      mutations.CLEAR(state);
      expect(state).toEqual(expectedState);
    });

    it('will not deselect selected slot', () => {
      const slot = { id: 1 };
      const state = {
        selectedSlot: slot,
      };

      mutations.CLEAR(state);
      expect(state.selectedSlot).toEqual(slot);
    });

    it('will not change booked property', () => {
      const state = {
        booked: true,
      };

      mutations.CLEAR(state);
      expect(state.booked).toEqual(true);
    });
  });

  describe('CLEAR_ERROR', () => {
    let state;

    beforeEach(() => {
      state = {
        error: {
          status: 500,
        },
      };
      mutations.CLEAR_ERROR(state);
    });

    it('will clear error', () => {
      expect(state.error).toBeNull();
    });
  });

  describe('DESELECT', () => {
    it('will deselect the selected slot', () => {
      const slot = { id: 1 };
      const state = {
        selectedSlot: slot,
      };

      mutations.DESELECT(state, slot);
      expect(state.selectedSlot).toEqual(null);
      expect(slot).toEqual({ id: 1 });
    });
  });

  describe('FILTER', () => {
    const dataProvider = new FilterDataProvider(DateProvider);
    const data = dataProvider.generate();
    const state = {};

    const dataCollection = [
      data.slot1, data.slot2, data.slot3,
      data.slot4, data.slot5, data.slot6,
      data.slot7, data.slot8, data.slot9,
      data.slot10, data.slot11,
    ];

    beforeEach(() => {
      const slots = dataCollection;
      mutations.INIT(state);
      callLoadMutation(state, { slots });
    });

    each([
      [
        {
          type: '',
          location: 'Leeds',
          clinician: 'Dr Who',
          date: DateFilterValues.TODAY,
        },
        [],
      ],
      [
        {
          type: 'Baby immunisations',
          location: '',
          clinician: 'Dr Who',
          date: DateFilterValues.TODAY,
        },
        [],
      ],
      [
        {
          type: '',
          location: '',
          clinician: 'Dr Who',
          date: DateFilterValues.TODAY,
        },
        [],
      ],
    ]).it('will return empty array if mandatory field are not selected', (selectedOptions, expectedSlots) => {
      state.selectedOptions = selectedOptions;

      mutations.FILTER(state);

      expect(state.filteredSlots).toEqual(expectedSlots);
    });
  });

  describe('INIT', () => {
    it('will set default values', () => {
      const slot = { id: 1 };
      const slots = [slot];

      const state = {
        slots,
        filteredSlots: [['2018-12-01', [slot]]],
        hasLoaded: true,
        selectedSlot: slot,
        booked: true,
        error: { status: 500 },
        filtersOptions: {
          types: ['option 1', 'options 2'],
          locations: ['option 1', 'options 2'],
          clinicians: ['option 1', 'options 2'],
          dates: ['option 1', 'options 2'],
        },
        selectedOptions: {
          type: 'Emergency',
          location: 'Leeds',
          clinician: 'Dr Who',
          date: DateFilterValues.ALL,
        },
      };

      const expectedState = {
        slots: [],
        filteredSlots: [],
        telephoneNumbers: [],
        hasLoaded: false,
        selectedSlot: null,
        booked: false,
        error: null,
        filtersOptions: {
          types: [],
          locations: [],
          clinicians: [],
          dates: [],
        },
        selectedOptions: {
          type: '',
          location: '',
          clinician: '',
          date: DateFilterValues.ALL,
        },
      };

      mutations.INIT(state);
      expect(state).toEqual(expectedState);
    });
  });

  describe('LOAD', () => {
    it('will return slots', () => {
      const slots = [
        {
          id: 1,
          type: 'Baby immunisations',
          startTime: '2018-04-21T17:11:59.084865+01:00',
          endTime: '2018-04-21T17:11:59.084865+01:00',
          location: 'Leeds',
          clinicians: ['Dr Who'],
        },
        {
          id: 3,
          type: 'Emergency',
          startTime: '2018-04-23T17:13:59.084865+01:00',
          endTime: '2018-04-23T17:13:59.084865+01:00',
          location: 'Bristol',
          clinicians: ['Dr House', 'Dr Drake Ramoray'],
        },
        {
          id: 2,
          type: 'Emergency',
          startTime: '2018-04-22T17:13:59.084865+01:00',
          endTime: '2018-04-22T17:13:59.084865+01:00',
          location: 'London',
          clinicians: ['Dr House', 'Dr Drake Ramoray'],
        },
        {
          id: 1,
          type: 'Baby immunisations',
          startTime: '2018-04-22T17:11:59.084865+01:00',
          endTime: '2018-04-22T17:11:59.084865+01:00',
          location: 'Leeds',
          clinicians: ['Dr Who'],
        },
      ];
      const data = { slots };
      const state = {};

      mutations.INIT(state);
      callLoadMutation(state, data);

      expect(state.slots).toEqual(slots);
    });

    it('will change value of hasLoaded property', () => {
      const state = {};
      mutations.INIT(state);
      callLoadMutation(state, { slots: [] });

      expect(state.hasLoaded).toEqual(true);
    });

    describe('filtersOptions', () => {
      const additionalSixteenWeeksSlotsEnabledOptions = [
        { value: 'next_eight_weeks', name: 'appointments.book.next_eight_weeks', translate: true },
        { value: 'all', name: 'appointments.book.all', translate: true },
      ];

      const additionalSixteenWeeksSlotsDisabledOptions = [
        { value: 'all', name: 'appointments.book.next_eight_weeks', translate: true },
      ];

      it.each([
        ['without 16 weeks option when toggle is disabled', false, additionalSixteenWeeksSlotsDisabledOptions],
        ['with 16 weeks option when toggle is enabled', true, additionalSixteenWeeksSlotsEnabledOptions],
      ])('will set filtersOptions %s', (_, toggle, dateOptions) => {
        const state = {
          filtersOptions: {
            types: [],
            locations: [],
            clinicians: [],
            dates: [],
          },
          selectedOptions: {
            type: '',
            location: '',
            clinician: '',
            date: 'this_week',
          },
        };

        const slots = [{
          id: 1,
          type: 'Baby immunisations',
          startTime: '2018-04-21T17:11:59.084865+01:00',
          endTime: '2018-04-21T17:11:59.084865+01:00',
          location: 'Leeds',
          clinicians: ['Dr Who', 'Dr House'],
        }];

        const expectedFiltersOptions = {
          types: [
            { value: '', name: 'appointments.book.selectType', translate: true },
            { value: 'Baby immunisations', name: 'Baby immunisations', translate: false },
          ],
          locations: [
            { value: '', name: 'appointments.book.selectLocation', translate: true },
            { value: 'Leeds', name: 'Leeds', translate: false },
          ],
          clinicians: [
            { value: '', name: 'appointments.book.noPreference', translate: true },
            { value: 'Dr House', name: 'Dr House', translate: false },
            { value: 'Dr Who', name: 'Dr Who', translate: false },
          ],
          dates: [
            { value: 'today', name: 'appointments.book.today', translate: true },
            { value: 'tomorrow', name: 'appointments.book.tomorrow', translate: true },
            { value: 'this_week', name: 'appointments.book.this_week', translate: true },
            { value: 'next_week', name: 'appointments.book.next_week', translate: true },
            ...dateOptions,
          ],
        };

        callLoadMutation(state, { slots }, toggle);

        expect(state.filtersOptions).toEqual(expectedFiltersOptions);
      });
    });

    it('will be defaulted to location if only one location has been returned', () => {
      const state = {
        filtersOptions: {
          types: [],
          locations: [],
          clinicians: [],
          dates: [],
        },
        selectedOptions: {
          type: '',
          location: '',
          clinician: '',
          date: DateFilterValues.THIS_WEEK,
        },
      };

      const slots = [
        {
          id: 1,
          type: 'Baby immunisations',
          startTime: '2018-04-21T17:11:59.084865+01:00',
          endTime: '2018-04-21T17:11:59.084865+01:00',
          location: 'Leeds',
          clinicians: ['Dr Who', 'Dr House'],
        },
        {
          id: 1,
          type: 'Baby immunisations',
          startTime: '2018-04-22T17:11:59.084865+01:00',
          endTime: '2018-04-22T17:11:59.084865+01:00',
          location: 'Leeds',
          clinicians: ['Dr Who', 'Dr Drake Ramoray'],
        },
      ];

      callLoadMutation(state, { slots });
      expect(state.selectedOptions.location).toEqual('Leeds');
    });
  });

  describe('SET_BOOKING_REASON_NECESSITY', () => {
    it('will set the `bookingReasonNecessity` to the received value', () => {
      const state = {};

      mutations.SET_BOOKING_REASON_NECESSITY(state, 'boo');

      expect(state.bookingReasonNecessity).toEqual('boo');
    });
  });

  describe('SELECT', () => {
    it('will set the selected slot to the slot', () => {
      const state = {
        selectedSlot: null,
      };
      mutations.SELECT(state, { id: 1 });
      expect(state.selectedSlot).toEqual({ id: 1 });
    });
  });

  describe('SET_SELECTED_OPTIONS', () => {
    it('will set the selected options', () => {
      const selectedOptions = {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Who',
        date: DateFilterValues.ALL,
      };
      const state = {
        selectedOptions: {
          type: '',
          location: '',
          clinician: '',
          date: DateFilterValues.THIS_WEEK,
        },
      };
      mutations.SET_SELECTED_OPTIONS(state, selectedOptions);
      expect(state.selectedOptions).toEqual(selectedOptions);
    });
  });
});
