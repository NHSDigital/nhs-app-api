import Vue from 'vue';
import Vuex from 'vuex';
import moment from 'moment';

Vue.use(Vuex);
Vue.filter('truncate', (value, length = 24) => (value ? value.substr(0, length) : undefined));
Vue.filter('shortDate', value => (value ? moment(value).format('D MMM YYYY') : ''));
