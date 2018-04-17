import Vue from 'vue';
import Axios from 'axios';
import store from '@/store';


Axios.defaults.baseUrl = process.env.API_HOST || 'http://localhost:8080';
Axios.defaults.headers.common.Accept = 'application/json';

Axios.interceptors.request.use((config) => {
  store.dispatch('http/isLoading');
  return config;
}, error => Promise.reject(error));

Axios.interceptors.response.use(
  (response) => {
    store.dispatch('http/loadingCompleted');
    return response;
  },
  (error) => {
    if (error.response.status === 401) {
      store.dispatch('http/loadingCompleted');
      store.dispatch('auth/unauthorised');
    }

    return Promise.reject(error);
  },
);

// Bind Axios to vue
Vue.$http = Axios;
Object.defineProperty(Vue.prototype, '$http', {
  get() {
    return Axios;
  },
});
