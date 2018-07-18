/* eslint-disable import/extensions */
import each from 'jest-each';
import moment from 'moment-timezone';
import mutations from '@/store/modules/availableAppointments/mutations';

const {
  LOAD,
  FILTER,
} = mutations;

const createDate = (date = new Date()) => moment.tz(date, 'Europe/London');

describe('FILTER', () => {
  const state = {
    filteredSlots: [],
    selectedOptions: {
      type: '',
      location: '',
      clinician: '',
      date: 'this_week',
    },
  };

  const today = createDate();
  today.set({ h: 23, m: 59, s: 59 });

  const startOfTomorrow = createDate();
  startOfTomorrow.add(1, 'days');
  startOfTomorrow.set({ h: 0, m: 0, s: 0 });

  const endOfTomorrow = createDate();
  endOfTomorrow.add(1, 'days');
  endOfTomorrow.set({ h: 23, m: 59, s: 59 });

  const endOfThisWeek = createDate();
  endOfThisWeek.add(7 - today.day(), 'days');
  endOfThisWeek.set({ h: 23, m: 59, s: 59 });

  const startOfNextWeek = createDate();
  startOfNextWeek.add(8 - today.day(), 'days');
  startOfNextWeek.set({ h: 0, m: 0, s: 0 });

  const endOfNextWeek = createDate();
  endOfNextWeek.add(14 - today.day(), 'days');
  endOfNextWeek.set({ h: 23, m: 59, s: 59 });

  const startOfNext2Week = createDate();
  startOfNext2Week.add(15 - today.day(), 'days');
  startOfNext2Week.set({ h: 0, m: 0, s: 0 });

  const slot1 = {
    id: 1,
    type: 'Emergency',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House'],
    isSelected: false,
  };

  const slot2 = {
    id: 2,
    type: 'Emergency',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House', 'Dr Drake Ramoray'],
    isSelected: false,
  };

  const slot3 = {
    id: 3,
    type: 'Emergency',
    startTime: startOfTomorrow.toISOString(),
    endTime: startOfTomorrow.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House', 'Dr Drake Ramoray'],
    isSelected: false,
  };

  const slot4 = {
    id: 4,
    type: 'Emergency',
    startTime: endOfTomorrow.toISOString(),
    endTime: endOfTomorrow.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House', 'Dr Drake Ramoray'],
    isSelected: false,
  };

  const slot5 = {
    id: 5,
    type: 'Emergency',
    startTime: startOfNextWeek.toISOString(),
    endTime: startOfNextWeek.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House'],
    isSelected: false,
  };

  const slot6 = {
    id: 6,
    type: 'Emergency',
    startTime: endOfNextWeek.toISOString(),
    endTime: endOfNextWeek.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House'],
    isSelected: false,
  };


  const slot7 = {
    id: 7,
    type: 'Baby immunisations',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House'],
    isSelected: false,
  };

  const slot8 = {
    id: 8,
    type: 'Emergency',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'London',
    clinicians: ['Dr House'],
    isSelected: false,
  };

  const slot9 = {
    id: 9,
    type: 'Emergency',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr Who'],
    isSelected: false,
  };

  const slot10 = {
    id: 10,
    type: 'Emergency',
    startTime: startOfNext2Week.toISOString(),
    endTime: startOfNext2Week.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House'],
    isSelected: false,
  };

  const slot11 = {
    id: 11,
    type: 'Emergency',
    startTime: endOfThisWeek.toISOString(),
    endTime: endOfThisWeek.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr House'],
    isSelected: false,
  };

  beforeEach(() => {
    const slots = [slot1, slot2, slot3, slot4, slot5, slot6, slot7, slot8, slot9, slot10, slot11];

    LOAD(state, { slots });
  });

  each([
    [
      {
        type: '',
        location: 'Leeds',
        clinician: 'Dr Who',
        date: 'today',
      },
      [],
    ],
    [
      {
        type: 'Baby immunisations',
        location: '',
        clinician: 'Dr Who',
        date: 'today',
      },
      [],
    ],
    [
      {
        type: '',
        location: '',
        clinician: 'Dr Who',
        date: 'today',
      },
      [],
    ],
  ]).it('will return empty array if mandatory field are not selected', (selectedOptions, expectedSlots) => {
    state.selectedOptions = selectedOptions;

    FILTER(state);

    expect(state.filteredSlots).toEqual(expectedSlots);
  });

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: 'today',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1, slot2, slot9]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr House',
        date: 'today',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1, slot2]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: 'today',
      },
      [
        [today.format('YYYY-MM-DD'), [slot2]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'London',
        clinician: 'Dr House',
        date: 'today',
      },
      [
        [today.format('YYYY-MM-DD'), [slot8]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: 'Dr House',
        date: 'today',
      },
      [
        [today.format('YYYY-MM-DD'), [slot7]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: 'tomorrow',
      },
      [
        [startOfTomorrow.format('YYYY-MM-DD'), [slot3, slot4]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr House',
        date: 'tomorrow',
      },
      [
        [startOfTomorrow.format('YYYY-MM-DD'), [slot3, slot4]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: 'tomorrow',
      },
      [
        [startOfTomorrow.format('YYYY-MM-DD'), [slot3, slot4]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: 'this_week',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1, slot2, slot9]],
        [startOfTomorrow.format('YYYY-MM-DD'), [slot3, slot4]],
        [endOfThisWeek.format('YYYY-MM-DD'), [slot11]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: 'this_week',
      },
      [
        [today.format('YYYY-MM-DD'), [slot2]],
        [startOfTomorrow.format('YYYY-MM-DD'), [slot3, slot4]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: 'this_week',
      },
      [
        [today.format('YYYY-MM-DD'), [slot7]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: 'next_week',
      },
      [
        [startOfNextWeek.format('YYYY-MM-DD'), [slot5]],
        [endOfNextWeek.format('YYYY-MM-DD'), [slot6]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: 'all',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1, slot2, slot9]],
        [startOfTomorrow.format('YYYY-MM-DD'), [slot3, slot4]],
        [endOfThisWeek.format('YYYY-MM-DD'), [slot11]],
        [startOfNextWeek.format('YYYY-MM-DD'), [slot5]],
        [endOfNextWeek.format('YYYY-MM-DD'), [slot6]],
        [startOfNext2Week.format('YYYY-MM-DD'), [slot10]],

      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: 'Dr House',
        date: 'all',
      },
      [
        [today.format('YYYY-MM-DD'), [slot7]],

      ],
    ],
  ]).it('will return correct data', (selectedOptions, expectedSlots) => {
    state.selectedOptions = selectedOptions;

    FILTER(state);

    expect(state.filteredSlots).toEqual(expectedSlots);
  });
});
