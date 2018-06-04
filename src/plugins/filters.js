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
  value => (value ? moment(value).format('Do MMMM YYYY') : ''));
