import { state } from '../../../../../src/store/repeatPrescriptionCourses';

describe('state', () => {
  it('will set the repeat prescription courses to an empty array', () => {
    expect(state().repeatPrescriptionCourses).toEqual([]);
  });
});
