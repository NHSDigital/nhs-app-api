/* eslint-disable import/extensions */
/* eslint-disable prefer-destructuring */
import DateProvider from '@/services/DateProvider';
import Mapper from '@/store/modules/availableAppointments/dateFilter/Mapper';
import LoadMutation from '@/store/modules/availableAppointments/mutation/LoadMutation';
import FilterMutation from '@/store/modules/availableAppointments/mutation/FilterMutation';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import each from 'jest-each';
import DataProvider from './DataProvider';

describe('FilterMutation [Sunday]', () => {
  const fakeDateProvider = {
    create: (date = new Date('2018-08-19 17:23:18')) => DateProvider.create(date),
  };
  const dataProvider = new DataProvider(fakeDateProvider);
  const filterMutation = new FilterMutation(fakeDateProvider, new Mapper(fakeDateProvider));
  const loadMutation = new LoadMutation(fakeDateProvider);
  const data = dataProvider.generate();

  const dataCollection = [
    data.slot1, data.slot2, data.slot3,
    data.slot4, data.slot5, data.slot6,
    data.slot7, data.slot8, data.slot9,
    data.slot10, data.slot11,
  ];

  const slots = loadMutation.execute(dataCollection).slots;

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.TODAY,
      },
      [
        ['2018-08-19', [data.slot1, data.slot2, data.slot9, data.slot11]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [
        ['2018-08-19', [data.slot1, data.slot2, data.slot11]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: DateFilterValues.TODAY,
      },
      [
        ['2018-08-19', [data.slot2]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'London',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [
        ['2018-08-19', [data.slot8]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [
        ['2018-08-19', [data.slot7]],
      ],
    ],
    [
      {
        type: '',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [],
    ],
    [
      {
        type: 'Baby immunisations',
        location: '',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [],
    ],
    [
      {
        type: 'Not Existing Type',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [],
    ],
  ]).it('will return correct data for "today"', (selectedOptions, expectedSlots) => {
    const actualSlots = filterMutation.execute(slots, selectedOptions);

    expect(actualSlots).toEqual(expectedSlots);
  });

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.TOMORROW,
      },
      [
        ['2018-08-20', [data.slot3, data.slot5, data.slot4]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.TOMORROW,
      },
      [
        ['2018-08-20', [data.slot3, data.slot5, data.slot4]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: DateFilterValues.TOMORROW,
      },
      [
        ['2018-08-20', [data.slot3, data.slot4]],
      ],
    ],
    [
      {
        type: 'Not Existing Type',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: DateFilterValues.TOMORROW,
      },
      [],
    ],
  ]).it('will return correct data for "tomorrow"', (selectedOptions, expectedSlots) => {
    const actualSlots = filterMutation.execute(slots, selectedOptions);

    expect(actualSlots).toEqual(expectedSlots);
  });

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.THIS_WEEK,
      },
      [
        ['2018-08-19', [data.slot1, data.slot2, data.slot9, data.slot11]],
      ],
    ],
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: 'Dr Drake Ramoray',
        date: DateFilterValues.THIS_WEEK,
      },
      [
        ['2018-08-19', [data.slot2]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.THIS_WEEK,
      },
      [
        ['2018-08-19', [data.slot7]],
      ],
    ],
    [
      {
        type: 'Not Existing Type',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.THIS_WEEK,
      },
      [],
    ],
  ]).it('will return correct data for "this_week"', (selectedOptions, expectedSlots) => {
    const actualSlots = filterMutation.execute(slots, selectedOptions);

    expect(actualSlots).toEqual(expectedSlots);
  });

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.NEXT_WEEK,
      },
      [
        ['2018-08-20', [data.slot3, data.slot5, data.slot4]],
        ['2018-08-21', []],
        ['2018-08-22', []],
        ['2018-08-23', []],
        ['2018-08-24', []],
        ['2018-08-25', []],
        ['2018-08-26', [data.slot6]],
      ],
    ],
    [
      {
        type: 'Not Existing',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.NEXT_WEEK,
      },
      [],
    ],
  ]).it('will return correct data for "next_week"', (selectedOptions, expectedSlots) => {
    const actualSlots = filterMutation.execute(slots, selectedOptions);

    expect(actualSlots).toEqual(expectedSlots);
  });

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.ALL,
      },
      [
        ['2018-08-19', [data.slot1, data.slot2, data.slot9, data.slot11]],
        ['2018-08-20', [data.slot3, data.slot5, data.slot4]],
        ['2018-08-26', [data.slot6]],
        ['2018-08-27', [data.slot10]],
      ],
    ],
    [
      {
        type: 'Baby immunisations',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.ALL,
      },
      [
        ['2018-08-19', [data.slot7]],
      ],
    ],
    [
      {
        type: 'Not Existing Type',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.ALL,
      },
      [],
    ],
  ]).it('will return correct data for "all"', (selectedOptions, expectedSlots) => {
    const actualSlots = filterMutation.execute(slots, selectedOptions);

    expect(actualSlots).toEqual(expectedSlots);
  });
});
