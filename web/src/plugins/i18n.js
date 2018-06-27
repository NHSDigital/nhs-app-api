/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import VueI18n from 'vue-i18n';
import messages from '../locale';

Vue.use(VueI18n);

/* eslint-disable */
export default ({ app }) => {
  app.i18n = new VueI18n({
    locale: 'en',
    fallbackLocale: 'en',
    messages,
    silentTranslationWarn: false,
  });
};
