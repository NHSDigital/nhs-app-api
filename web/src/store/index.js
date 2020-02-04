import analytics from './modules/analytics';
import appVersion from './modules/appVersion';
import auth from './modules/auth';
import availableAppointments from './modules/availableAppointments';
import biometricBanner from './modules/biometricBanner';
import cookieBanner from './modules/cookieBanner';
import device from './modules/device';
import errors from './modules/errors';
import flashMessage from './modules/flashMessage';
import header from './modules/header';
import http from './modules/http';
import knownServices from './modules/knownServices';
import linkedAccounts from './modules/linkedAccounts';
import modal from './modules/modal';
import messaging from './modules/messaging';
import myAppointments from './modules/myAppointments';
import myRecord from './modules/myRecord';
import navigation from './modules/navigation';
import nominatedPharmacy from './modules/nominatedPharmacy';
import notifications from './modules/notifications';
import onlineConsultations from './modules/onlineConsultations';
import organDonation from './modules/organDonation';
import pageTitle from './modules/pageTitle';
import patientPracticeMessaging from './modules/patientPracticeMessaging';
import practiceSettings from './modules/practiceSettings';
import prescriptions from './modules/prescriptions';
import repeatPrescriptionCourses from './modules/repeatPrescriptionCourses';
import serviceJourneyRules from './modules/serviceJourneyRules';
import session from './modules/session';
import spinner from './modules/spinner';
import termsAndConditions from './modules/termsAndConditions';
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
  knownServices,
  linkedAccounts,
  messaging,
  modal,
  myAppointments,
  myRecord,
  navigation,
  nominatedPharmacy,
  notifications,
  onlineConsultations,
  organDonation,
  pageTitle,
  patientPracticeMessaging,
  practiceSettings,
  prescriptions,
  repeatPrescriptionCourses,
  serviceJourneyRules,
  session,
  spinner,
  termsAndConditions,
  throttling,
};

export const actions = {
  async nuxtServerInit({ dispatch }, { req, res }) {
    const authCookie = this.$cookies.get('nhso.auth');
    if (process.server) {
      /*
      disabled the eslint global-require as consola should only be imported when running on server
      babel doesn't transpile the library code to es5 (for ie 11) when imports on client
      */
      const consola = require('consola'); // eslint-disable-line global-require
      const { redirectUri, codeVerifier } = authCookie || {};
      const { nhsoRequestId } = res.locals;

      consola.info(`Auth Cookie values for request: ${req.url}, redirectUri=${redirectUri}, codeVerifier=${codeVerifier}, CorrelationId=${nhsoRequestId}`);
    }
    await dispatch('auth/updateConfig', authCookie);
    await dispatch('session/setInfo', this.$cookies.get('nhso.session'));
  },
};
