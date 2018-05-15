import { assign, find } from 'lodash/fp';
import { PRESCRIPTIONS_LOADED, PRESCRIPTIONS_CLEAR } from './mutation-types';

const findById = (id, collection) => find(item => item.id === id)(collection);

export default {
  [PRESCRIPTIONS_LOADED](state, data) {
    const prescriptionsResponse = assign({}, data.response);
    const prescriptionCourses = [];
    for (let i = 0; i < prescriptionsResponse.prescriptions.length; i += 1) {
      const prescription = prescriptionsResponse.prescriptions[i];
      for (let j = 0; j < prescription.courses.length; j += 1) {
        const prescriptionCourse = prescription.courses[j];
        const course = findById(prescriptionCourse.courseId, prescriptionsResponse.courses);
        prescriptionCourses.push({
          courseId: course.id,
          orderDate: prescription.orderDate,
          name: course.name,
          dosage: course.dosage,
          quantity: course.quantity,
        });
      }
    }
    state.prescriptionCourses = prescriptionCourses;
    state.hasLoaded = true;
  },
  [PRESCRIPTIONS_CLEAR](state) {
    state.prescriptionCourses = [];
    state.hasLoaded = false;
  },
};
