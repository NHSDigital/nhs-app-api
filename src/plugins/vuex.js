import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);
Vue.filter('truncate', (value, length = 24) => (value ? value.substr(0, length) : undefined));
