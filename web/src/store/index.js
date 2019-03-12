/* eslint-disable global-require */
import Vuex from 'vuex';

import cookieBanner from './modules/cookieBanner';
import header from './modules/header';
import pageTitle from './modules/pageTitle';
import availableAppointments from './modules/availableAppointments';
import auth from './modules/auth';
import device from './modules/device';
import http from './modules/http';
import navigation from './modules/navigation';
import prescriptions from './modules/prescriptions';
import repeatPrescriptionCourses from './modules/repeatPrescriptionCourses';
import session from './modules/session';
import errors from './modules/errors';
import myAppointments from './modules/myAppointments';
import flashMessage from './modules/flashMessage';
import analytics from './modules/analytics';
import termsAndConditions from './modules/termsAndConditions';
import appVersion from './modules/appVersion';
import myRecord from './modules/myRecord';
import organDonation from './modules/organDonation';
import throttling from './modules/throttling';

const createStore = () => new Vuex.Store({
  modules: {
    cookieBanner,
    header,
    pageTitle,
    availableAppointments,
    auth,
    device,
    http,
    navigation,
    prescriptions,
    repeatPrescriptionCourses,
    session,
    errors,
    myAppointments,
    myRecord,
    flashMessage,
    analytics,
    termsAndConditions,
    appVersion,
    organDonation,
    throttling,
  },
  actions: {
    async nuxtServerInit({ dispatch }, { req }) {
      const authCookie = this.$cookies.get('nhso.auth');
      if (process.server) {
        const consola = require('consola');
        const { redirectUri, codeVerifier } = authCookie || {};
        consola.info(`Auth Cookie values for request: ${req.url}: redirectUri: ${redirectUri}, codeVerifier: ${codeVerifier}`);
      }
      await dispatch('auth/updateConfig', this.$cookies.get('nhso.auth'));
      await dispatch('session/setInfo', this.$cookies.get('nhso.session'));
    },
  },
});

export default createStore;
