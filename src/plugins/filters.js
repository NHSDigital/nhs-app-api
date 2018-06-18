/* eslint-disable */
import Vue from 'vue';
import moment from 'moment';

Vue.filter(
  'truncate',
  (value, length = 24) => (value ? value.substr(0, length) : undefined),
);

Vue.filter(
  'shortDate',
  value => (value ? moment(value).format('D MMM YYYY') : ''),
);

Vue.filter('longDate',
  value => (value ? moment(value).format('D MMMM YYYY') : ''));

Vue.filter('datePart',
  function(value, datePart) {
      switch (datePart) {
        case "Unknown":
        case "YearMonthDay":
          return value ? moment(value).format('D MMMM YYYY') : ''
          break;
        case "Year":
          return value ? moment(value).format('YYYY') : ''
          break;
        case "YearMonth":
          return value ? moment(value).format('MMMM YYYY') : ''
          break;
        case "YearMonthDayTime":
          return value ? moment(value).utc().format("D MMMM YYYY hh:mm") : ''
          break;
        default:
          return value ? moment(value).format('D MMMM YYYY'): ''
          break;
      }
  }
);

