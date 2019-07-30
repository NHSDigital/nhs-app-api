/* eslint-disable no-unused-vars */
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  INIT_REPEAT_PRESCRIPTIONS,
  REPEAT_PRESCRIPTION_VALIDATED,
  SELECT_REPEAT_PRESCRIPTION,
  REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO,
  PARTIAL_ORDER_RESULT,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT_REPEAT_PRESCRIPTIONS);
  },
  load({ commit }) {
    return this.app.$http.getV1PatientCourses().then((data) => {
      commit(REPEAT_PRESCRIPTION_COURSES_LOADED, data);
    });
  },
  validate({ commit }, validationObject) {
    commit(REPEAT_PRESCRIPTION_VALIDATED, validationObject);
  },
  select({ commit }, id) {
    commit(SELECT_REPEAT_PRESCRIPTION, id);
  },
  orderRepeatPrescription({ commit }, repeatPrescriptionOrder) {
    const param = {
      repeatPrescriptionRequest: repeatPrescriptionOrder,
    };
    return this.app.$http
      .postV1PatientPrescriptions(param).then((data) => {
        commit(PARTIAL_ORDER_RESULT, data);
        if (process.client) {
          this.dispatch('analytics/satelliteTrack', 'prescription_ordered');
        }
      });
  },
  updateAdditionalInfo({ commit }, repeatPrescriptionAdditionalInfo) {
    commit(REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO, repeatPrescriptionAdditionalInfo);
  },
};
