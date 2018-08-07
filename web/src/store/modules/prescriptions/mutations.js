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

    for (let i = 0; i < prescriptionsResponse.prescriptions.length; i += 1) {
      const prescription = prescriptionsResponse.prescriptions[i];

      if (!prescriptionCourses[prescription.status]) {
        prescriptionCourses[prescription.status] = [];
      }

      for (let j = 0; j < prescription.courses.length; j += 1) {
        const prescriptionCourse = prescription.courses[j];
        const course = findById(
          prescriptionCourse.courseId,
          prescriptionsResponse.courses,
        );
        prescriptionCourses[prescription.status].push({
          courseId: prescriptionCourse.courseId,
          orderDate: prescription.orderDate,
          name: course.name,
          status: prescription.status,
          details: course.details,
        });
      }
    }

    state.prescriptionCourses = prescriptionCourses;
    state.hasLoaded = true;
  },
  [INIT_PRESCRIPTIONS](state) {
    state = initialState();
  },
  [PRESCRIPTIONS_CLEAR](state) {
    state.prescriptionCourses = [];
    state.hasLoaded = false;
  },
};
