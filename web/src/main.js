import Vue from 'vue';
import VueCookies from 'vue-cookies';
import VueMeta from 'vue-meta';
import Axios from 'axios';
import App from '@/App';
import router from '@/router';
import store from '@/store';
import i18n from '@/plugins/i18n';
import ApiPlugin from '@/plugins/api';
import RouterExtensions from '@/plugins/routing';
import NativeAppCallbacksPlugin from '@/plugins/native-app-callbacks';
import DefaultMixin from '@/plugins/mixinDefinitions/DefaultMixin';
import '@/plugins/filters';
import '@/plugins/directives';
import { redirectTo } from '@/lib/utils';

// eslint-disable-next-line
import '@/style/_nhsukfrontend.scss';

const defineErrorHandling = () => {
  const globalErrorLogHandler = (message) => {
    store.dispatch('log/onError', message);
  };

  const vueErrorLogHandler = (err) => {
    let errorMessage = err;
    if (err && err.stack) {
      errorMessage = err.stack;
    }
    store.dispatch('log/onError', errorMessage);

    throw err;
  };

  window.onerror = globalErrorLogHandler;
  Vue.config.errorHandler = vueErrorLogHandler;
};

function convertBooleans() {
  Object.keys(store.$env).forEach((key) => {
    if (store.$env[key] === 'true') {
      store.$env[key] = true;
    }

    if (store.$env[key] === 'false') {
      store.$env[key] = false;
    }
  });
}

(async () => {
  Vue.use(VueCookies);
  Vue.use(ApiPlugin);
  Vue.use(VueMeta);
  Vue.use(RouterExtensions, { router });
  Vue.use(NativeAppCallbacksPlugin);
  Vue.config.productionTip = false;
  store.$env = (await Axios.get('CONFIG_PATH/config.json')).data; // see the web dockerfile to see how this is defined at startup

  convertBooleans();

  store.$cookies = Vue.$cookies;

  defineErrorHandling();

  Vue.mixin(DefaultMixin);

  const app = new Vue({
    i18n,
    router,
    store,
    render: h => h(App),
  });

  store.app = app;

  app.$mount('#app');

  // only adds a global reference to the vue instance running locally
  if (store.$env.VUE_WINDOW_OBJECT_ENABLED) {
    window.vue = app;
  }

  // global functions for native app callback
  window.appEvent = ({ event, payload }) => store.dispatch(event, payload);
  window.appRoute = path => redirectTo(app, path);
})();
