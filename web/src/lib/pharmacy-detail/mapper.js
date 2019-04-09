import moment from 'moment-timezone';

const convertTime = (time) => {
  const momentTime = moment(time, 'HH:mm');
  return momentTime.minutes() === 0 ? momentTime.format('ha') : momentTime.format('h:mma');
};

const convertTimeRange = (timeRangeIn24h) => {
  const timeIn24h = timeRangeIn24h.split('-');
  return `${convertTime(timeIn24h[0])} to ${convertTime(timeIn24h[1])}`;
};

const daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
export default (data) => {
  const openingTimeDetailArray = [];
  daysInWeek.forEach((dayInWeek) => {
    const openingTimeDetails = data.filter(dayDetail => dayDetail.day === dayInWeek);
    const day = { day: dayInWeek, times: [] };
    if (openingTimeDetails.length > 0) {
      day.times = openingTimeDetails.map(v => convertTimeRange(v.time));
    }
    openingTimeDetailArray.push(day);
  });
  return openingTimeDetailArray;
};
