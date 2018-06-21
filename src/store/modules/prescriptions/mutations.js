import {
  assign,
  find,
} from 'lodash/fp';
import {
  PRESCRIPTIONS_CLEAR,
  PRESCRIPTIONS_LOADED,
  INIT_PRESCRIPTIONS,
  initialState,
} from './mutation-types';

const findById = (id, collection) => find(item => item.id === id)(collection);

export default {
  /* eslint-disable no-shadow */
  /* eslint-disable no-param-reassign */
  /* eslint-disable no-unused-vars */
  [PRESCRIPTIONS_LOADED](state, data) {
    const prescriptionsResponse = assign({}, data);
    const prescriptionCourses = [];
    let id = 0;
    for (let i = 0; i < prescriptionsResponse.prescriptions.length; i += 1) {
      const prescription = prescriptionsResponse.prescriptions[i];
      for (let j = 0; j < prescription.courses.length; j += 1) {
        const prescriptionCourse = prescription.courses[j];
        const course = findById(
          prescriptionCourse.courseId,
          prescriptionsResponse.courses,
        );
        prescriptionCourses.push({
          id,
          courseNumber: prescriptionCourse.courseId,
          orderDate: prescription.orderDate,
          name: course.name,
          status: prescriptionCourse.status,
          dosage: course.dosage,
          quantity: course.quantity,
          selected: false,
        });
        id += 1;
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
