import moment from 'moment';
import { assign, find } from 'lodash/fp';

export const PRESCRIPTIONS_LOADED = 'PRESCRIPTIONS_LOADED';
export const PRESCRIPTIONS_CLEAR = 'PRESCRIPTIONS_CLEAR';
const INIT_PRESCRIPTIONS = 'INIT_PRESCRIPTIONS';

const initialState = {
  prescriptionCourses: [],
  hasLoaded: false,
  hasErrored: false,
};
export const state = () => initialState;

function getFromDate() {
  return moment()
    .subtract(6, 'months')
    .toISOString();
}

function getPrescriptionsParameters() {
  return {
    fromDate: getFromDate(),
  };
}

export const actions = {
  load({ commit }) {
    return this.app.$http
      .getV1PatientPrescriptions(getPrescriptionsParameters())
      .then((data) => {
        commit(PRESCRIPTIONS_LOADED, data);
      });
  },
  init({ commit }) {
    commit(INIT_PRESCRIPTIONS);
  },
  clear({ commit }) {
    commit(PRESCRIPTIONS_CLEAR);
  },
};

const findById = (id, collection) => find(item => item.id === id)(collection);

export const mutations = {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [PRESCRIPTIONS_LOADED](state, data) {
    const prescriptionsResponse = assign({}, data.response);
    const prescriptionCourses = [];
    for (let i = 0; i < prescriptionsResponse.prescriptions.length; i += 1) {
      const prescription = prescriptionsResponse.prescriptions[i];
      for (let j = 0; j < prescription.courses.length; j += 1) {
        const prescriptionCourse = prescription.courses[j];
        const course = findById(
          prescriptionCourse.courseId,
          prescriptionsResponse.courses,
        );
        prescriptionCourses.push({
          courseId: course.id,
          orderDate: prescription.orderDate,
          name: course.name,
          dosage: course.dosage,
          quantity: course.quantity,
          selected: false,
        });
      }
    }
    state.prescriptionCourses = prescriptionCourses;
    state.hasLoaded = true;
  },
  [INIT_PRESCRIPTIONS](state) {
    state = initialState;
  },
  [PRESCRIPTIONS_CLEAR](state) {
    state.prescriptionCourses = [];
    state.hasLoaded = false;
  },
};

export const getters = {
  prescriptionCourses(state) {
    return state.prescriptionCourses.map((prescription) => {
      const result = assign({}, prescription);
      return result;
    });
  },
};
