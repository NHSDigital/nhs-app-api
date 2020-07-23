/* eslint-disable import/extensions */
import DateProvider from '@/services/DateProvider';
import LoadMutation from '@/store/modules/availableAppointments/mutation/LoadMutation';

describe('LoadMutation', () => {
  let mutation;
  beforeEach(() => {
    mutation = new LoadMutation(DateProvider);
  });

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
    const actualResult = mutation.execute(data);

    expect(actualResult.slots).toEqual(slots);
  });

  describe('filtersOptions', () => {
    const additionalSixteenWeeksSlotsEnabledOptions = [
      { value: 'next_eight_weeks', name: 'appointments.booking.filters.date.options.next_eight_weeks', translate: true },
      { value: 'all', name: 'appointments.booking.filters.date.options.all', translate: true },
    ];

    const additionalSixteenWeeksSlotsDisabledOptions = [
      { value: 'all', name: 'appointments.booking.filters.date.options.next_eight_weeks', translate: true },
    ];

    it.each([
      ['without 16 weeks option when toggle is disabled', false, additionalSixteenWeeksSlotsDisabledOptions],
      ['with 16 weeks option when toggle is enabled', true, additionalSixteenWeeksSlotsEnabledOptions],
    ])('will set filtersOptions sorted alphabetically %s', (_, toggle, dateOptions) => {
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
          id: 2,
          type: 'General Medicine',
          startTime: '2019-04-22T12:00:00.0+01:00',
          endTime: '2019-04-22T12:10:00.0+01:00',
          location: 'Birmingham',
          clinicians: ['Dr Taylor'],
        },
      ];

      const data = { slots };
      const actualResult = mutation.execute(data, toggle);

      const expectedFiltersOptions = {
        types: [
          { value: '', name: 'appointments.booking.filters.type.default_option', translate: true },
          { value: 'Baby immunisations', name: 'Baby immunisations', translate: false },
          { value: 'General Medicine', name: 'General Medicine', translate: false },
        ],
        locations: [
          { value: '', name: 'appointments.booking.filters.location.default_option', translate: true },
          { value: 'Birmingham', name: 'Birmingham', translate: false },
          { value: 'Leeds', name: 'Leeds', translate: false },
        ],
        clinicians: [
          { value: '', name: 'appointments.booking.filters.clinician.default_option', translate: true },
          { value: 'Dr House', name: 'Dr House', translate: false },
          { value: 'Dr Taylor', name: 'Dr Taylor', translate: false },
          { value: 'Dr Who', name: 'Dr Who', translate: false },
        ],
        dates: [
          { value: 'today', name: 'appointments.booking.filters.date.options.today', translate: true },
          { value: 'tomorrow', name: 'appointments.booking.filters.date.options.tomorrow', translate: true },
          { value: 'this_week', name: 'appointments.booking.filters.date.options.this_week', translate: true },
          { value: 'next_week', name: 'appointments.booking.filters.date.options.next_week', translate: true },
          ...dateOptions,
        ],
      };

      expect(actualResult.filtersOptions).toEqual(expectedFiltersOptions);
    });
  });

  it('will be defaulted to location if only one location has been returned', () => {
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

    const data = { slots };
    const actualResult = mutation.execute(data);
    expect(actualResult.defaultLocationSelectedOption).toEqual('Leeds');
  });

  it('will return correct object', () => {
    const slots = [];
    const data = { slots };
    const actualResult = mutation.execute(data);

    expect(Array.isArray(actualResult.slots)).toBeTruthy();
    expect(Array.isArray(actualResult.filtersOptions.types)).toBeTruthy();
    expect(Array.isArray(actualResult.filtersOptions.locations)).toBeTruthy();
    expect(Array.isArray(actualResult.filtersOptions.clinicians)).toBeTruthy();
    expect(Array.isArray(actualResult.filtersOptions.dates)).toBeTruthy();
    expect(typeof actualResult.defaultLocationSelectedOption).toEqual('string');
  });
});
