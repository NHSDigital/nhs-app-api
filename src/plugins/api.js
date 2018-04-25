import Vue from 'vue';
import Axios from 'axios';
import store from '@/store';
import NHSOnlineApi from '../services/nhsonlineapi';


Axios.defaults.baseUrl = process.env.API_HOST || 'http://localhost:8082';
Axios.defaults.headers.common.Accept = 'application/json';


Axios.get('/config').then(({ data: { API_HOST } }) => {
  const api = new NHSOnlineApi({
    domain: API_HOST,
    store,
  });
  // Bind nhsonline to vue
  Vue.$http = api;
  Object.defineProperty(Vue.prototype, '$http', {
    get() {
      return api;
    },
  });
});
