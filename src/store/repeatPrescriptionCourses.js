import { assign, mapKeys, map } from 'lodash/fp';

export const REPEAT_PRESCRIPTION_COURSES_LOADED =
  'REPEAT_PRESCRIPTION_COURSES_LOADED';

const INIT_REPEAT_PRESCRIPTIONS = 'INIT_REPEAT_PRESCRIPTIONS';
const SELECT_REPEAT_PRESCRIPTION = 'SELECT_REPEAT_PRESCRIPTION';
const REPEAT_PRESCRIPTION_VALIDATED = 'REPEAT_PRESCRIPTION_VALIDATED';

const initialState = {
  repeatPrescriptionCourses: [],
  hasLoaded: false,
  hasErrored: false,
  validated: false,
  isValid: false,
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
  validate({ commit }, validationObject) {
    commit(REPEAT_PRESCRIPTION_VALIDATED, validationObject);
  },
  select({ commit }, id) {
    commit(SELECT_REPEAT_PRESCRIPTION, id);
  },
};

export const mutations = {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [REPEAT_PRESCRIPTION_COURSES_LOADED](state, data) {
    const { response: courses } = data;
    mapKeys((key) => {
      state[key] = courses[key];
    })(courses);

    state.repeatPrescriptionCourses = map((course) => {
      const result = assign({}, course);
      result.selected = false;
      return result;
    })(state.courses);
    state.loaded = true;
  },
  [INIT_REPEAT_PRESCRIPTIONS](state) {
    state = initialState;
  },
  [REPEAT_PRESCRIPTION_VALIDATED](state, validationObject) {
    if (validationObject.submitted) {
      state.validated = true;
    }
    state.isValid = validationObject.isValid;
  },
  [SELECT_REPEAT_PRESCRIPTION](state, id) {
    state.repeatPrescriptionCourses = state.repeatPrescriptionCourses.map((course) => {
      if (course.id === id) {
        course.selected = !course.selected;
      }
      return course;
    });
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
  selectedPrescriptions(state) {
    const selectedCourses = [];
    state.repeatPrescriptionCourses.forEach((course) => {
      if (course.selected) {
        selectedCourses.push(course);
      }
    });
    return selectedCourses;
  },
  isValid(state) {
    const selectedCourses = [];
    state.repeatPrescriptionCourses.forEach((course) => {
      if (course.selected) {
        selectedCourses.push(course);
      }
    });
    return selectedCourses.length > 0;
  },
};
