/* eslint-disable import/extensions */
/* eslint-disable prefer-destructuring */
import DateProvider from '@/services/DateProvider';
import Mapper from '@/store/modules/availableAppointments/dateFilter/Mapper';
import FilterMutation from '@/store/modules/availableAppointments/mutation/FilterMutation';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import each from 'jest-each';
import FilterDataProvider from './FilterDataProvider';

describe('FilterMutation [Weekday]', () => {
  const fakeDateProvider = {
    create: (date = new Date('2018-08-13')) => DateProvider.create(date),
  };
  const dataProvider = new FilterDataProvider(fakeDateProvider);
  const filterMutation = new FilterMutation(fakeDateProvider, new Mapper(fakeDateProvider));
  const data = dataProvider.generate();

  const slots = [
    data.slot17, data.slot5, data.slot8, data.slot4, data.slot12,
    data.slot6, data.slot7, data.slot14, data.slot9, data.slot16,
    data.slot11, data.slot2, data.slot13, data.slot3, data.slot15,
    data.slot10, data.slot1, data.slot18,
  ];

  each([
    [
      {
        type: 'Emergency',
        location: 'Leeds',
        clinician: '',
        date: DateFilterValues.TODAY,
      },
      [
        ['2018-08-13', [data.slot12, data.slot14]],
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
        ['2018-08-13', [data.slot14, data.slot2, data.slot13, data.slot1]],
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
        ['2018-08-13', [data.slot12, data.slot14, data.slot2]],
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
        ['2018-08-13', [data.slot8]],
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
        ['2018-08-13', [data.slot7]],
      ],
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
        type: '',
        location: 'Leeds',
        clinician: 'Dr House',
        date: DateFilterValues.TODAY,
      },
      [],
    ],
    [
      {
        type: 'Not Existing Appointment',
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

  describe('FilterMutation De-dupe appointment slot by session name', () => {
    let slotA;
    let slotB;

    beforeEach(() => {
      const today = new Date();
      const now = today.toISOString();

      slotA = {
        id: 1,
        ref: 'slot_A',
        type: 'Emergency',
        startTime: now,
        endTime: now,
        location: 'Manchester',
        sessionName: 'Emergency Appointment',
        clinicians: ['Dr Dre'],
      };

      slotB = {
        id: 2,
        ref: 'slot_B',
        type: 'Emergency',
        startTime: now,
        endTime: now,
        location: 'Manchester',
        sessionName: 'Another Appointment',
        clinicians: ['Dr Martin'],
      };
    });

    const selectedOptions = {
      type: 'Emergency',
      location: 'Manchester',
      clinician: '',
      date: DateFilterValues.ALL,
    };

    it('with multiple slots where the session name differs', () => {
      const actualSlots = filterMutation.execute([slotA, slotB], selectedOptions);

      expect(actualSlots[0][1]).toEqual([slotA, slotB]);
    });

    it('with multiple slots where the session name is the same', () => {
      slotB.sessionName = 'Emergency Appointment';
      const actualSlots = filterMutation.execute([slotA, slotB], selectedOptions);

      expect(actualSlots[0][1]).toEqual([slotA]);
    });
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
        ['2018-08-14', [data.slot3, data.slot4]],
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
        ['2018-08-14', [data.slot3, data.slot4, data.slot15]],
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
        ['2018-08-14', [data.slot3, data.slot4, data.slot15]],
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
        ['2018-08-13', [data.slot12, data.slot14]],
        ['2018-08-14', [data.slot3, data.slot4]],
        ['2018-08-15', []],
        ['2018-08-16', []],
        ['2018-08-17', []],
        ['2018-08-18', []],
        ['2018-08-19', [data.slot11]],
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
        ['2018-08-13', [data.slot12, data.slot14, data.slot2]],
        ['2018-08-14', [data.slot3, data.slot4, data.slot15]],
        ['2018-08-15', []],
        ['2018-08-16', []],
        ['2018-08-17', []],
        ['2018-08-18', []],
        ['2018-08-19', []],
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
        ['2018-08-13', [data.slot7]],
        ['2018-08-14', []],
        ['2018-08-15', []],
        ['2018-08-16', []],
        ['2018-08-17', []],
        ['2018-08-18', []],
        ['2018-08-19', []],
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
        ['2018-08-20', [data.slot5]],
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
        type: 'Not Existing Type',
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
        ['2018-08-13', [data.slot12, data.slot14]],
        ['2018-08-14', [data.slot3, data.slot4]],
        ['2018-08-19', [data.slot11]],
        ['2018-08-20', [data.slot5]],
        ['2018-08-26', [data.slot6]],
        ['2018-08-27', [data.slot17]],
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
        ['2018-08-13', [data.slot7]],
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
