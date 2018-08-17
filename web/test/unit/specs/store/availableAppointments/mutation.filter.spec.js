/* eslint-disable import/extensions */
import each from 'jest-each';
import mutations from '@/store/modules/availableAppointments/mutations';
import DateProvider from '@/services/DateProvider';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import DataProvider from './mutation/DataProvider';

const {
  INIT,
  LOAD,
  FILTER,
} = mutations;

describe('FILTER', () => {
  const dataProvider = new DataProvider(DateProvider);
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
    INIT(state);
    LOAD(state, { slots });
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

    FILTER(state);

    expect(state.filteredSlots).toEqual(expectedSlots);
  });
});
