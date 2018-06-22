/* eslint-disable import/extensions */
import prescriptions from '../../../../../src/store/modules/prescriptions';

const { state } = prescriptions;
describe('state', () => {
  it('will initialise state correctly', () => {
    expect(state().prescriptionCourses).toEqual(null);
    expect(state().hasLoaded).toEqual(false);
  });
});
