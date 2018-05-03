import state from '@/store/modules/prescriptions/state';

describe('state', () => {
  it('will set the courses and repeat prescriptions to an empty array', () => {
    expect(state.coursesAndRepeatPrescriptions).toEqual([]);
  });
});
