import mutations from '@/store/modules/repeat-prescription-courses/mutations';
import { REPEAT_PRESCRIPTION_COURSES_LOADED } from '@/store/modules/repeat-prescription-courses/mutation-types';

describe('REPEAT_PRESCRIPTION_COURSES_LOADED', () => {
  it('will set the courses on the state from the received courses data', () => {
    const state = {};
    const receivedData = {
      repeatPrescriptionCourses: [{}],
    };

    mutations[REPEAT_PRESCRIPTION_COURSES_LOADED](state, receivedData);

    expect(state.repeatPrescriptionCourses).toEqual(receivedData.repeatPrescriptionCourses);
  });
});
