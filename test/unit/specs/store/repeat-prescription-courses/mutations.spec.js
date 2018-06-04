/* eslint-disable import/extensions */
import mutations from '../../../../../src/store/modules/repeatPrescriptionCourses/mutations';
import { REPEAT_PRESCRIPTION_COURSES_LOADED } from '../../../../../src/store/modules/repeatPrescriptionCourses/mutation-types';


describe('REPEAT_PRESCRIPTION_COURSES_LOADED', () => {
  it('will set the courses on the state from the received courses data', () => {
    const state = {};
    const receivedData = {
      repeatPrescriptionCourses: [],
    };

    mutations[REPEAT_PRESCRIPTION_COURSES_LOADED](state, receivedData);

    expect(state.repeatPrescriptionCourses).toEqual(receivedData.repeatPrescriptionCourses);
  });
});
