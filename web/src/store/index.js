import Vue from 'vue';
import Vuex from 'vuex';
import get from 'lodash/fp/get';
import analytics from './modules/analytics';
import appVersion from './modules/appVersion';
import auth from './modules/auth';
import availableAppointments from './modules/availableAppointments';
import biometricBanner from './modules/biometricBanner';
import cookieBanner from './modules/cookieBanner';
import device from './modules/device';
import documents from './modules/documents';
import errors from './modules/errors';
import flashMessage from './modules/flashMessage';
import header from './modules/header';
import http from './modules/http';
import knownServices from './modules/knownServices';
import linkedAccounts from './modules/linkedAccounts';
import login from './modules/login';
import loginSettings from './modules/loginSettings';
import modal from './modules/modal';
import messaging from './modules/messaging';
import myAppointments from './modules/myAppointments';
import myRecord from './modules/myRecord';
import navigation from './modules/navigation';
import nominatedPharmacy from './modules/nominatedPharmacy';
import notifications from './modules/notifications';
import onlineConsultations from './modules/onlineConsultations';
import organDonation from './modules/organDonation';
import pageLeaveWarning from './modules/pageLeaveWarning';
import gpMessages from './modules/gpMessages';
import practiceSettings from './modules/practiceSettings';
import preRegistrationInformation from './modules/preRegistrationInformation';
import prescriptions from './modules/prescriptions';
import repeatPrescriptionCourses from './modules/repeatPrescriptionCourses';
import serviceJourneyRules from './modules/serviceJourneyRules';
import session from './modules/session';
import spinner from './modules/spinner';
import termsAndConditions from './modules/termsAndConditions';

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
  },
  getters: {
    getEnvVariable: state => variable => get(variable)(state.$env),
  },
  mutations: {
  },
  actions: {
  },
  modules: {
    analytics,
    appVersion,
    auth,
    availableAppointments,
    biometricBanner,
    cookieBanner,
    device,
    documents,
    errors,
    flashMessage,
    header,
    http,
    knownServices,
    linkedAccounts,
    login,
    loginSettings,
    messaging,
    modal,
    myAppointments,
    myRecord,
    navigation,
    nominatedPharmacy,
    notifications,
    onlineConsultations,
    organDonation,
    pageLeaveWarning,
    gpMessages,
    practiceSettings,
    preRegistrationInformation,
    prescriptions,
    repeatPrescriptionCourses,
    serviceJourneyRules,
    session,
    spinner,
    termsAndConditions,
  },
});
