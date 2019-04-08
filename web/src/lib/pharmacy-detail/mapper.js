import moment from 'moment-timezone';

function convertTime(timeRangeIn24h) {
  let timeFormatted = '';
  const timeIn24h = timeRangeIn24h.split('-');
  timeIn24h.forEach((timeSt) => {
    const time = moment(timeSt, 'HH:mm');
    if (time.minutes() === 0) {
      timeFormatted += time.format('ha');
    } else {
      timeFormatted += time.format('h:mma');
    }
    timeFormatted += ' to ';
  });
  return timeFormatted.slice(0, -4);
}

// eslint-disable-next-line import/prefer-default-export
export const mapPharmacyDetail = (data) => {
  const daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  const openingTimeDetailArray = [];
  daysInWeek.forEach((dayInWeek) => {
    const openingTimeDetails = data.filter(dayDetail => dayDetail.day === dayInWeek);
    if (openingTimeDetails.length > 0) {
      openingTimeDetailArray.push({ day: dayInWeek,
        times: openingTimeDetails.map(v => convertTime(v.time)) });
    } else {
      openingTimeDetailArray.push({ day: dayInWeek, times: [] });
    }
  });

  return openingTimeDetailArray;
};
