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

export const modules = {
  analytics,
  appVersion,
  auth,
  availableAppointments,
  cookieBanner,
  device,
  errors,
  flashMessage,
  header,
  http,
  myAppointments,
  myRecord,
  navigation,
  organDonation,
  pageTitle,
  prescriptions,
  repeatPrescriptionCourses,
  session,
  termsAndConditions,
  throttling,
};

export const actions = {
  async nuxtServerInit({ dispatch }) {
    await dispatch('auth/updateConfig', this.$cookies.get('nhso.auth'));
    await dispatch('session/setInfo', this.$cookies.get('nhso.session'));
  },
};
