import Vuex from 'vuex';

import header from './modules/header';
import appointment from './modules/appointment';
import appointmentSlots from './modules/appointmentSlots';
import auth from './modules/auth';
import { UPDATE_CONFIG } from './modules/auth/mutation-types';
import device from './modules/device';
import http from './modules/http';
import navigation from './modules/navigation';
import prescriptions from './modules/prescriptions';
import repeatPrescriptionCourses from './modules/repeatPrescriptionCourses';
import session from './modules/session';
import errors from './modules/errors';
import myAppointments from './modules/myAppointments';

const createStore = () => new Vuex.Store({
  modules: {
    header,
    appointment,
    appointmentSlots,
    auth,
    device,
    http,
    navigation,
    prescriptions,
    repeatPrescriptionCourses,
    session,
    errors,
    myAppointments,
  },
  actions: {
    nuxtServerInit({ state, commit }) {
      commit(UPDATE_CONFIG, state.auth.config);
    },
  },
});

export default createStore;
