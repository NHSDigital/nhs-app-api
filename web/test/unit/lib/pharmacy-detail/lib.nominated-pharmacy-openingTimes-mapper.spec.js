import { mapPharmacyDetail } from '@/lib/pharmacy-detail/mapper';

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
            times: ['08:00-18:00'],
          },
          {
            day: 'Tuesday',
            times: ['08:00-20:00'],
          },
          {
            day: 'Wednesday',
            times: ['08:00-20:00'],
          },
          {
            day: 'Thursday',
            times: ['08:00-20:00'],
          },
          {
            day: 'Friday',
            times: ['08:00-20:00'],
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
