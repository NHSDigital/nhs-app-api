import get from 'lodash/fp/get';
import { GP_SESSION_ERROR_STATUS } from '@/lib/utils';
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  INIT_REPEAT_PRESCRIPTIONS,
  REPEAT_PRESCRIPTION_VALIDATED,
  SELECT_REPEAT_PRESCRIPTION,
  REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO,
  PARTIAL_ORDER_RESULT,
  PRESCRIPTIONS_JOURNEY_COMPLETE,
  PRESCRIPTIONS_JOURNEY_START,
  ADD_ERROR,
} from './mutation-types';


const createError = ({ response }) => ({
  status: response.status || '',
  serviceDeskReference: get('serviceDeskReference')(response.data) || '',
});

export default {
  init({ commit }) {
    commit(INIT_REPEAT_PRESCRIPTIONS);
    this.dispatch('device/unlockNavBar');
  },
  async load({ commit, rootState }) {
    try {
      const data = await this.app.$http.getV1PatientCourses({
        ignoreError: true,
      });
      commit(REPEAT_PRESCRIPTION_COURSES_LOADED, data);

      if (rootState.device.isNativeApp) {
        sessionStorage.removeItem('hasRetried');
      }
      this.dispatch('session/setRetry', false);
    } catch (error) {
      if (error.response.status !== GP_SESSION_ERROR_STATUS) {
        this.dispatch('errors/addApiError', error);
      } else {
        commit(ADD_ERROR, createError(error));
      }
    } finally {
      this.dispatch('device/unlockNavBar');
    }
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
