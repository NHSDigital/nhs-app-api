/* eslint-disable */
import Vue from 'vue';
import moment from 'moment-timezone';

Vue.filter('longDate',
  value => (value ? moment.tz(value, 'Europe/London').format('D MMMM YYYY') : ''));

Vue.filter('datePart',
  function(value, datePart) {
      switch (datePart) {
        case "Unknown":
        case "YearMonthDay":
          return value ? moment.utc(value).format('D MMMM YYYY') : ''
          break;
        case "Year":
          return value ? moment.utc(value).format('YYYY') : ''
          break;
        case "YearMonth":
          return value ? moment.utc(value).format('MMMM YYYY') : ''
          break;
        case "YearMonthDayTime":
          return value ? moment.utc(value).format("D MMMM YYYY h:mm a") : ''
          break;
        default:
          return value ? moment.utc(value).format('D MMMM YYYY'): ''
          break;
      }
  }
);

