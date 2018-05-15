import state from '@/store/modules/prescriptions/state';

describe('state', () => {
  it('will initialise state correctly', () => {
    expect(state.prescriptionCourses).toEqual([]);
    expect(state.hasLoaded).toEqual(false);
  });
});
