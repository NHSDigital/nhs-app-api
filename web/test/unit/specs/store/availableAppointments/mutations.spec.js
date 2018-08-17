/* eslint-disable import/extensions */
import mutations from '@/store/modules/availableAppointments/mutations';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';

const {
  DESELECT,
  SELECT,
  CLEAR,
  LOAD,
  INIT,
  SET_SELECTED_OPTIONS,
} = mutations;

describe('SELECT', () => {
  it('will set the selected slot to the slot', () => {
    const state = {
      selectedSlot: null,
    };
    SELECT(state, { id: 1 });
    expect(state.selectedSlot).toEqual({ id: 1 });
  });
});

describe('DESELECT', () => {
  it('will deselect the selected slot', () => {
    const slot = { id: 1 };
    const state = {
      selectedSlot: slot,
    };

    DESELECT(state, slot);
    expect(state.selectedSlot).toEqual(null);
    expect(slot).toEqual({ id: 1 });
  });
});

describe('CLEAR', () => {
  it('will set default values', () => {
    const slot = { id: 1 };
    const slots = new Map();
    slots.set('2018-12-02', [slot]);
    const state = {
      slots,
      filteredSlots: [['2018-12-01', [slot]]],
      hasLoaded: true,
      selectedSlot: slot,
      booked: true,
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
      slots: new Map(),
      filteredSlots: [],
      hasLoaded: false,
      selectedSlot: slot,
      booked: true,
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

    CLEAR(state);
    expect(state).toEqual(expectedState);
  });

  it('will not deselect selected slot', () => {
    const slot = { id: 1 };
    const state = {
      selectedSlot: slot,
    };

    CLEAR(state);
    expect(state.selectedSlot).toEqual(slot);
  });

  it('will not change booked property', () => {
    const state = {
      booked: true,
    };

    CLEAR(state);
    expect(state.booked).toEqual(true);
  });
});

describe('INIT', () => {
  it('will set default values', () => {
    const slot = { id: 1 };
    const slots = new Map();
    slots.set('2018-12-02', [slot]);
    const state = {
      slots,
      filteredSlots: [['2018-12-01', [slot]]],
      hasLoaded: true,
      selectedSlot: slot,
      booked: true,
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
      slots: new Map(),
      filteredSlots: [],
      hasLoaded: false,
      selectedSlot: null,
      booked: false,
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

    INIT(state);
    expect(state).toEqual(expectedState);
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
    SET_SELECTED_OPTIONS(state, selectedOptions);
    expect(state.selectedOptions).toEqual(selectedOptions);
  });
});

describe('LOAD', () => {
  it('will sort slots', () => {
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

    const sortedSlots = new Map();
    sortedSlots.set('2018-04-21', [slots[0]]);
    sortedSlots.set('2018-04-22', [slots[3], slots[2]]);
    sortedSlots.set('2018-04-23', [slots[1]]);

    INIT(state);
    LOAD(state, data);

    expect(state.slots).toEqual(sortedSlots);
  });

  it('will change value of hasLoaded property', () => {
    const state = {};
    INIT(state);
    LOAD(state, { slots: [] });

    expect(state.hasLoaded).toEqual(true);
  });

  it('will set filtersOptions', () => {
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

    const slots = [
      {
        id: 1,
        type: 'Baby immunisations',
        startTime: '2018-04-21T17:11:59.084865+01:00',
        endTime: '2018-04-21T17:11:59.084865+01:00',
        location: 'Leeds',
        clinicians: ['Dr Who', 'Dr House'],
      },
    ];

    LOAD(state, { slots });

    const expectedFiltersOptions = {
      types: [
        { value: '', name: 'appointments.booking.filters.type.default_option', translate: true },
        { value: 'Baby immunisations', name: 'Baby immunisations', translate: false },
      ],
      locations: [
        { value: '', name: 'appointments.booking.filters.location.default_option', translate: true },
        { value: 'Leeds', name: 'Leeds', translate: false },
      ],
      clinicians: [
        { value: '', name: 'appointments.booking.filters.clinician.default_option', translate: true },
        { value: 'Dr Who', name: 'Dr Who', translate: false },
        { value: 'Dr House', name: 'Dr House', translate: false },
      ],
      dates: [
        { value: 'today', name: 'appointments.booking.filters.date.options.today', translate: true },
        { value: 'tomorrow', name: 'appointments.booking.filters.date.options.tomorrow', translate: true },
        { value: 'this_week', name: 'appointments.booking.filters.date.options.this_week', translate: true },
        { value: 'next_week', name: 'appointments.booking.filters.date.options.next_week', translate: true },
        { value: 'all', name: 'appointments.booking.filters.date.options.all', translate: true },
      ],
    };

    expect(state.filtersOptions).toEqual(expectedFiltersOptions);
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

    LOAD(state, { slots });
    expect(state.selectedOptions.location).toEqual('Leeds');
  });
});
