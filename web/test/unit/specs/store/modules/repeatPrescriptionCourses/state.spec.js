import repeatPrescriptions from '@/store/modules/repeatPrescriptionCourses/index';

const { state } = repeatPrescriptions;

describe('state', () => {
  it('will set the repeat prescription courses to an empty array', () => {
    expect(state().repeatPrescriptionCourses).toEqual([]);
  });

  it('will set specialRequest to null', () => {
    expect(state().specialRequest).toEqual(null);
  });
});
