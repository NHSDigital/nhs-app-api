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

  const startOfNextWeek = createDate();
  startOfNextWeek.add(8 - today.day(), 'days');
  startOfNextWeek.set({ h: 0, m: 0, s: 0 });

  const endOfNextWeek = createDate();
  endOfNextWeek.add(14 - today.day(), 'days');
  endOfNextWeek.set({ h: 23, m: 59, s: 59 });

  const slot1 = {
    id: 1,
    type: 'Baby immunisations',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr Who'],
    isSelected: false,
  };

  const slot2 = {
    id: 3,
    type: 'Emergency',
    startTime: today.toISOString(),
    endTime: today.toISOString(),
    location: 'Bristol',
    clinicians: ['Dr House', 'Dr Drake Ramoray'],
    isSelected: false,
  };

  const slot3 = {
    id: 2,
    type: 'Emergency',
    startTime: startOfTomorrow.toISOString(),
    endTime: startOfTomorrow.toISOString(),
    location: 'London',
    clinicians: ['Dr House', 'Dr Drake Ramoray'],
    isSelected: false,
  };

  const slot4 = {
    id: 4,
    type: 'Baby immunisations',
    startTime: endOfNextWeek.toISOString(),
    endTime: endOfNextWeek.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr Who'],
    isSelected: false,
  };

  const slot5 = {
    id: 5,
    type: 'Emergency',
    startTime: endOfTomorrow.toISOString(),
    endTime: endOfTomorrow.toISOString(),
    location: 'Bristol',
    clinicians: ['Dr House', 'Dr Drake Ramoray'],
    isSelected: false,
  };

  const slot6 = {
    id: 6,
    type: 'Baby immunisations',
    startTime: startOfNextWeek.toISOString(),
    endTime: startOfNextWeek.toISOString(),
    location: 'Leeds',
    clinicians: ['Dr Who'],
    isSelected: false,
  };

  beforeEach(() => {
    const slots = [slot1, slot2, slot3, slot4, slot5, slot6];

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
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: 'next_week',
      },
      [[startOfNextWeek.format('YYYY-MM-DD'), [slot6]], [endOfNextWeek.format('YYYY-MM-DD'), [slot4]]],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: 'today',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: 'tomorrow',
      },
      [],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: 'this_week',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: 'all',
      },
      [
        [today.format('YYYY-MM-DD'), [slot1]],
        [startOfNextWeek.format('YYYY-MM-DD'), [slot6]],
        [endOfNextWeek.format('YYYY-MM-DD'), [slot4]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'London',
        clinician: 'Dr House, Dr Drake Ramoray',
        date: '2',
      },
      [[endOfTomorrow.format('YYYY-MM-DD'), [slot3]]],
    ],
  ]).it('will return correct data', (selectedOptions, expectedSlots) => {
    state.selectedOptions = selectedOptions;

    FILTER(state);

    expect(state.filteredSlots).toEqual(expectedSlots);
  });
});
