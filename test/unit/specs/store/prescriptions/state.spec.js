import { state } from '../../../../../src/store/prescriptions';

describe('state', () => {
  it('will initialise state correctly', () => {
    expect(state().prescriptionCourses).toEqual([]);
    expect(state().hasLoaded).toEqual(false);
  });
});
