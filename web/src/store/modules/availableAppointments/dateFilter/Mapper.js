import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';

export default class Mapper {
  constructor(DateProvider) {
    this.dateProvider = DateProvider;
  }

  createDate(rangeInDays = 0) {
    const date = this.dateProvider.create();
    date.add(rangeInDays, 'days');

    return date;
  }

  mapToStartDate(value) {
    const today = this.dateProvider.create();
    let rangeInDays;
    switch (value) {
      case DateFilterValues.TOMORROW:
        rangeInDays = 1;
        break;
      case DateFilterValues.NEXT_WEEK:
        rangeInDays = (today.day() === 0) ? 1 : 8 - today.day();
        break;
      default:
        rangeInDays = 0;
        break;
    }

    return this.createDate(rangeInDays);
  }

  mapToEndDate(value) {
    const today = this.dateProvider.create();
    let rangeInDays;
    switch (value) {
      case DateFilterValues.TODAY:
        rangeInDays = 0;
        break;
      case DateFilterValues.TOMORROW:
        rangeInDays = 1;
        break;
      case DateFilterValues.NEXT_WEEK:
        rangeInDays = (today.day() === 0) ? 7 : 14 - today.day();
        break;
      default:
        rangeInDays = (today.day() === 0) ? 0 : 7 - today.day();
        break;
    }

    const date = this.createDate(rangeInDays);

    return date;
  }
}
