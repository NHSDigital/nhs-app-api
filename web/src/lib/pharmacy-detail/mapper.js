// eslint-disable-next-line import/prefer-default-export
export const mapPharmacyDetail = (data) => {
  const daysInWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  const openingTimeDetailArray = [];
  daysInWeek.forEach((dayInWeek) => {
    const openingTimeDetails = data.filter(dayDetail => dayDetail.day === dayInWeek);

    if (openingTimeDetails.length > 0) {
      openingTimeDetailArray.push({ day: dayInWeek, times: openingTimeDetails.map(v => v.time) });
    } else {
      openingTimeDetailArray.push({ day: dayInWeek, times: [] });
    }
  });

  return openingTimeDetailArray;
};
