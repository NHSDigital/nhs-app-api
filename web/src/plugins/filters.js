/* eslint-disable */
import Vue from 'vue';
import moment from 'moment-timezone';
import { readableBytes, datePart } from '@/lib/utils';

Vue.filter('readableBytes', readableBytes);

Vue.filter('uppercase', value => `${value}`.toLocaleUpperCase());

Vue.filter('longDate',
  value => (value ? moment.tz(value, 'Europe/London').format('D MMMM YYYY') : ''));

Vue.filter('datePart', datePart);
