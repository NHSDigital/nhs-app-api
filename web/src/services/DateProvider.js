/* eslint-disable import/extensions */
import moment from 'moment-timezone';

export const TIMEZONE = 'Europe/London';

export default {
  create(date = new Date()) {
    return moment.tz(date, TIMEZONE);
  },
};
