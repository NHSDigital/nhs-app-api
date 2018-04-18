// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.

import Vue from 'vue';
import Axios from 'axios';
import './plugins/api';
import './plugins/vuex';
import { i18n } from './plugins/vue-i18n';
import { router } from './plugins/vue-router';
import './plugins/vuex-router-sync';
import App from './App';
import store from './store';

Vue.config.productionTip = false;

Axios.get('/config')
  .then((response) => {
    // JSON responses are automatically parsed.
    Vue.prototype.$config = response.data;
    /* eslint-disable no-new */
    new Vue({
      el: '#app',
      i18n,
      router,
      store,
      render: h => h(App),
    });
  });
