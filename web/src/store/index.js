import biometricBanner from './modules/biometricBanner';
import nominatedPharmacy from './modules/nominatedPharmacy';
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
import serviceJourneyRules from './modules/serviceJourneyRules';
import throttling from './modules/throttling';

export const modules = {
  analytics,
  appVersion,
  auth,
  availableAppointments,
  biometricBanner,
  cookieBanner,
  device,
  errors,
  flashMessage,
  header,
  http,
  myAppointments,
  myRecord,
  navigation,
  nominatedPharmacy,
  organDonation,
  pageTitle,
  prescriptions,
  repeatPrescriptionCourses,
  serviceJourneyRules,
  session,
  termsAndConditions,
  throttling,
};

export const actions = {
  async nuxtServerInit({ dispatch }, { req }) {
    const authCookie = this.$cookies.get('nhso.auth');
    if (process.server) {
      /*
      disabled the eslint global-require as consola should only be imported when running on server
      babel doesn't transpile the library code to es5 (for ie 11) when imports on client
      */
      const consola = require('consola'); // eslint-disable-line global-require
      const { redirectUri, codeVerifier } = authCookie || {};
      consola.info(`Auth Cookie values for request: ${req.url}: redirectUri: ${redirectUri}, codeVerifier: ${codeVerifier}`);
    }
    await dispatch('auth/updateConfig', authCookie);
    await dispatch('session/setInfo', this.$cookies.get('nhso.session'));
  },
};
