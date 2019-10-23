import moment from 'moment-timezone';
// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import { readableBytes, datePart } from '@/lib/utils';

moment.locale('en_GB');

export const formatDate = (value, format) => (value ? moment.tz(value, 'Europe/London').format(format) : '');
export const longDate = value => formatDate(value, 'D MMMM YYYY');

Vue.filter('datePart', datePart);

Vue.filter('formatDate', formatDate);

Vue.filter('longDate', longDate);

Vue.filter('readableBytes', readableBytes);

Vue.filter('uppercase', value => `${value}`.toLocaleUpperCase());
