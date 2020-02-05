import assign from 'lodash/fp/assign';
import find from 'lodash/fp/find';
import {
  PRESCRIPTIONS_CLEAR,
  PRESCRIPTIONS_LOADED,
} from './mutation-types';

const findById = (id, collection) => find(item => item.id === id)(collection);

export default {
  [PRESCRIPTIONS_LOADED](state, data) {
    const prescriptionsResponse = assign({}, data);
    const prescriptionCourses = {};

    (prescriptionsResponse.prescriptions || [])
      .forEach((prescription) => {
        prescriptionCourses[prescription.status] = prescriptionCourses[prescription.status] || [];

        (prescription.courses || [])
          .forEach((prescriptionCourse) => {
            const course = findById(
              prescriptionCourse.courseId,
              prescriptionsResponse.courses,
            );
            prescriptionCourses[prescription.status].push({
              courseId: prescriptionCourse.courseId,
              orderDate: prescription.orderDate,
              orderedBy: prescription.orderedBy,
              name: course.name,
              status: prescription.status,
              details: course.details,
            });
          });
      });

    state.prescriptionCourses = prescriptionCourses;
    state.hasLoaded = true;
  },
  [PRESCRIPTIONS_CLEAR](state) {
    state.prescriptionCourses = {};
    state.hasLoaded = false;
  },
};
