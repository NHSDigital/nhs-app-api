import { assign, mapKeys, map } from 'lodash/fp';

export const REPEAT_PRESCRIPTION_COURSES_LOADED =
  'REPEAT_PRESCRIPTION_COURSES_LOADED';

const INIT_REPEAT_PRESCRIPTIONS = 'INIT_REPEAT_PRESCRIPTIONS';

const initialState = {
  repeatPrescriptionCourses: [],
  hasLoaded: false,
  hasErrored: false,
};

export const state = () => initialState;

export const actions = {
  init({ commit }) {
    commit(INIT_REPEAT_PRESCRIPTIONS);
  },
  load({ commit }) {
    return this.app.$http.getV1PatientCourses().then((data) => {
      commit(REPEAT_PRESCRIPTION_COURSES_LOADED, data);
    });
  },
};

export const mutations = {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [REPEAT_PRESCRIPTION_COURSES_LOADED](state, data) {
    mapKeys((key) => {
      state[key] = data[key];
    })(data);

    state.repeatPrescriptionCourses = map((course) => {
      const result = assign({}, course);
      return result;
    })(state.repeatPrescriptionCourses);

    state.REPEAT_PRESCRIPTION_COURSES_LOADED = true;
  },
  [INIT_REPEAT_PRESCRIPTIONS](state) {
    state = initialState;
  },
};

export const getters = {
  /* eslint-disable no-shadow */
  repeatPrescriptionCourses(state) {
    state.repeatPrescriptionCourses.map((course) => {
      const result = assign({}, course);
      return result;
    });
  },
};
