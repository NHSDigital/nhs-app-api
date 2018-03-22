// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue';
import VueI18n from 'vue-i18n';
import axios from 'axios';
import App from './App';
import router from './router';
import en from './locales/en';
// import NHSOnlineApi from './services/nhsonlineapi';


Vue.config.productionTip = false;
Vue.use(VueI18n);

const messages = {
  en,
};

const i18n = new VueI18n({
  locale: 'en',
  messages,
});

axios.get('/config')
  .then((response) => {
    // JSON responses are automatically parsed.
    Vue.prototype.$config = response.data;
    /* eslint-disable no-new */
    new Vue({
      i18n,
      router,
      el: '#app',
      components: { App },
      template: '<App/>',
      // provide: {
      //  nhsOnlineApi: new NHSOnlineApi(response.data.API_HOST),
      // },
    });
  });
