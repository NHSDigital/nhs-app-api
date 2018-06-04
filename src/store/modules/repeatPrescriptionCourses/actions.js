import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  INIT_REPEAT_PRESCRIPTIONS,
  REPEAT_PRESCRIPTION_VALIDATED,
  SELECT_REPEAT_PRESCRIPTION,
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
};
