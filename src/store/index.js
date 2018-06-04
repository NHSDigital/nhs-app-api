import Vuex from 'vuex';

import header from './modules/header';
import appointmentSlots from './modules/appointmentSlots';
import auth from './modules/auth';
import device from './modules/device';
import http from './modules/http';
import navigation from './modules/navigation';
import prescriptions from './modules/prescriptions';
import repeatPrescriptionCourses from './modules/repeatPrescriptionCourses';
import myRecord from './modules/myRecord';

const createStore = () => new Vuex.Store({
  modules: {
    header,
    appointmentSlots,
    auth,
    device,
    http,
    navigation,
    prescriptions,
    repeatPrescriptionCourses,
    myRecord,
  },
});

export default createStore;
