// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue';
import VueI18n from 'vue-i18n';
import App from './App';
import router from './router';
import en from './locales/en';

Vue.config.productionTip = false;
Vue.use(VueI18n);

const messages = {
  en,
};

const i18n = new VueI18n({
  locale: 'en',
  messages,
});

/* eslint-disable no-new */
new Vue({
  i18n,
  el: '#app',
  router,
  components: { App },
  template: '<App/>',
});
