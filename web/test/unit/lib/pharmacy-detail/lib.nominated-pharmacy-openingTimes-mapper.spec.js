import mapPharmacyDetail from '@/lib/pharmacy-detail/mapper';

describe('mapping for opening times of nominated pharmacy', () => {
  let data;

  beforeEach(() => {
    data =
    {
      openingTimes: [
        {
          day: 'Monday',
          time: '08:00-18:00',
        },
        {
          day: 'Monday',
          time: '19:00-20:00',
        },
        {
          day: 'Tuesday',
          time: '08:00-20:00',
        },
        {
          day: 'Wednesday',
          time: '08:00-20:00',
        },
        {
          day: 'Thursday',
          time: '08:00-20:00',
        },
        {
          day: 'Friday',
          time: '08:00-20:00',
        }],
    };
  });

  it('will throw error if opening times is null', () => {
    expect(() => {
      mapPharmacyDetail(null);
    }).toThrow('no opening times provided to map');
  });

  it('will throw error if opening times is undefined', () => {
    expect(() => {
      mapPharmacyDetail(undefined);
    }).toThrow('no opening times provided to map');
  });

  describe('map openingTimes after formatting', () => {
    let formattedData;
    beforeEach(() => {
      formattedData =
      {
        formattedOpeningTimes: [
          {
            day: 'Sunday',
            times: [],
          },
          {
            day: 'Monday',
            times: ['8am to 6pm', '7pm to 8pm'],
          },
          {
            day: 'Tuesday',
            times: ['8am to 8pm'],
          },
          {
            day: 'Wednesday',
            times: ['8am to 8pm'],
          },
          {
            day: 'Thursday',
            times: ['8am to 8pm'],
          },
          {
            day: 'Friday',
            times: ['8am to 8pm'],
          },
          {
            day: 'Saturday',
            times: [],
          }],
      };
    });
    it('will map opening times of pharmacy', () => {
      expect(mapPharmacyDetail(data.openingTimes)).toEqual(formattedData.formattedOpeningTimes);
    });
  });
});
