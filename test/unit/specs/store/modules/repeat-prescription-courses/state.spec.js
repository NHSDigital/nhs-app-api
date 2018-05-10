import state from '@/store/modules/repeat-prescription-courses/state';

describe('state', () => {
  it('will set the repeat prescription courses to an empty array', () => {
    expect(state.repeatPrescriptionCourses).toEqual([]);
  });
});
