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

(async () => {
  Vue.use(VueCookies);
  Vue.use(ApiPlugin);
  Vue.use(VueMeta);
  Vue.use(RouterExtensions, { router });
  Vue.config.productionTip = false;
  store.$env = (await Axios.get('/config.json')).data;
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
  store.app.isNhsAppPath = (path) => {
    const matches = router.getMatchedComponents(path);

    if (matches.length === 0) {
      return false;
    }

    const matchesWithNotFound = matches.filter(m => m !== undefined && m.name === 'NotFoundPage');
    return matchesWithNotFound.length === 0;
  };

  app.$mount('#app');



  // TODO: [REMOVE] Added for debugging purposes
  window.vue = app;

  // global functions for native app callback
  window.appEvent = ({ event, payload }) => store.dispatch(event, payload);
  window.appRoute = path => redirectTo(app, path);

  // Backwards compatability for native apps to call dispatch (pre-nuxt removal).
  window.$nuxt = Object.assign({}, {
    $store: {
      dispatch: (event, payload) => {
        store.dispatch.call(app, event, payload);
      },
    },
  });
})();
