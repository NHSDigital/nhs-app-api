import prescriptions from '@/store/modules/prescriptions/index';

const { state } = prescriptions;
describe('state', () => {
  it('will initialise state correctly', () => {
    expect(state().prescriptionCourses).toEqual({});
    expect(state().hasLoaded).toEqual(false);
  });
});
