import mutations from '@/store/modules/prescriptions/mutations';
import { PRESCRIPTIONS_LOADED, PRESCRIPTIONS_CLEAR } from '@/store/modules/prescriptions/mutation-types';
import MedicationCourseStatus from '@/lib/medication-course-status';

describe('PRESCRIPTIONS_LOADED', () => {
  it('will set the prescription courses on the state to the received data', () => {
    const state = {};
    const receivedData = {
      prescriptions: [
        {
          courses: [
            {
              courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
            },
          ],
          orderDate: '0001-01-01T00:00:00+00:00',
          status: MedicationCourseStatus.Approved,
        },
      ],
      courses: [
        {
          id: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
          name: 'Co-codamol 8mg/500mg capsules',
          details: '20 capsule - One To Be Taken Four Times A Day',
        },
      ],
    };

    const expectedData = {
      Approved: [
        {
          courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
          orderDate: '0001-01-01T00:00:00+00:00',
          name: 'Co-codamol 8mg/500mg capsules',
          details: '20 capsule - One To Be Taken Four Times A Day',
          status: MedicationCourseStatus.Approved,
        },
      ],
    };

    mutations[PRESCRIPTIONS_LOADED](state, receivedData);

    expect(state.prescriptionCourses).toEqual(expectedData);
    expect(state.hasLoaded).toEqual(true);
  });
});

describe('PRESCRIPTIONS_CLEAR', () => {
  it('will set the prescription courses on the state to an empty array', () => {
    const state = {};

    mutations[PRESCRIPTIONS_CLEAR](state);

    expect(state.prescriptionCourses).toEqual({});
    expect(state.hasLoaded).toEqual(false);
  });
});
