import Vuex from 'vuex';

import header from './modules/header';
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

const createStore = () => new Vuex.Store({
  modules: {
    header,
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
    flashMessage,
  },
  actions: {
    nuxtServerInit({ state, dispatch }) {
      dispatch('auth/updateConfig', state.auth.config);
    },
  },
});

export default createStore;
