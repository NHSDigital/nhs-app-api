import mutations from '@/store/modules/prescriptions/mutations';
import { PRESCRIPTIONS_LOADED, PRESCRIPTIONS_CLEAR } from '@/store/modules/prescriptions/mutation-types';
import MedicationCourseStatus from '@/lib/medication-course-status';

describe('PRESCRIPTIONS_LOADED', () => {
  it('will set the prescription courses on the state to the received data and order by date', () => {
    const state = {};
    const receivedData = {
      prescriptions: [
        {
          courses: [
            {
              courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
            },
          ],
          orderDate: '2019-01-01T00:00:00+00:00',
          status: MedicationCourseStatus.Approved,
        },
        {
          courses: [
            {
              courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f0',
            },
          ],
          orderDate: '2019-01-10T00:00:00+00:00',
          status: MedicationCourseStatus.Rejected,
        },
        {
          courses: [
            {
              courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f9',
            },
          ],
          orderDate: '2019-01-10T00:00:00+00:00',
          status: MedicationCourseStatus.Requested,
        },
      ],
      courses: [
        {
          id: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
          name: 'Co-codamol 8mg/500mg capsules',
          details: '20 capsule - One To Be Taken Four Times A Day',
        },
        {
          id: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f9',
          name: 'Paracetemol 500mg',
          details: '100 tablets - Two To Be Taken Two Times A Day',
        },
        {
          id: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f0',
          name: 'Trial drug',
          details: 'Dosage to be discussed with doctor',
        },
      ],
    };

    const expectedData = [
      {
        courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
        details: '20 capsule - One To Be Taken Four Times A Day',
        name: 'Co-codamol 8mg/500mg capsules',
        orderDate: '2019-01-01T00:00:00+00:00',
        status: 'Approved',
      },
      {
        courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f0',
        details: 'Dosage to be discussed with doctor',
        name: 'Trial drug',
        orderDate: '2019-01-10T00:00:00+00:00',
        status: 'Rejected',
      },
      {
        courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f9',
        details: '100 tablets - Two To Be Taken Two Times A Day',
        name: 'Paracetemol 500mg',
        orderDate: '2019-01-10T00:00:00+00:00',
        status: 'Requested',
      },
    ];

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
