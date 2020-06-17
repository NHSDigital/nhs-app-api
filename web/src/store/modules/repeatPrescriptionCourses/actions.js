/* eslint-disable no-unused-vars */
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  INIT_REPEAT_PRESCRIPTIONS,
  REPEAT_PRESCRIPTION_VALIDATED,
  SELECT_REPEAT_PRESCRIPTION,
  REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO,
  PARTIAL_ORDER_RESULT,
  PRESCRIPTIONS_JOURNEY_COMPLETE,
  PRESCRIPTIONS_JOURNEY_START,
} from './mutation-types';

export default {
  init({ commit }) {
    commit(INIT_REPEAT_PRESCRIPTIONS);
    this.dispatch('device/unlockNavBar');
  },
  load({ commit }) {
    return this.app.$http.getV1PatientCourses()
      .then((data) => {
        commit(REPEAT_PRESCRIPTION_COURSES_LOADED, data);
      })
      .catch(() => false)
      .finally(() => {
        this.dispatch('device/unlockNavBar');
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
        commit(PRESCRIPTIONS_JOURNEY_START);
        commit(PARTIAL_ORDER_RESULT, data);
        this.dispatch('analytics/satelliteTrack', 'prescription_ordered');
      });
  },
  updateAdditionalInfo({ commit }, repeatPrescriptionAdditionalInfo) {
    commit(REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO, repeatPrescriptionAdditionalInfo);
  },
  completeOrderJourney({ commit }) {
    commit(PRESCRIPTIONS_JOURNEY_COMPLETE);
  },
  startOrderJourney({ commit }) {
    commit(PRESCRIPTIONS_JOURNEY_START);
  },
};
