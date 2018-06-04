import {
  mutations,
  PRESCRIPTIONS_LOADED,
  PRESCRIPTIONS_CLEAR,
} from '../../../../../src/store/prescriptions';

describe('PRESCRIPTIONS_LOADED', () => {
  it('will set the prescription courses on the state to the received data', () => {
    const state = {};
    const receivedData = {
      response: {
        prescriptions: [
          {
            courses: [
              {
                courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
                status: 0,
              },
            ],
            orderDate: '0001-01-01T00:00:00+00:00',
          },
        ],
        courses: [
          {
            id: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
            name: 'Co-codamol 8mg/500mg capsules',
            dosage: 'One To Be Taken Four Times A Day',
            quantity: '20 capsule',
          },
        ],
      },
    };

    const expectedData = [
      {
        courseId: '1e5483ce-2e5c-4e57-ad2e-235a08b4f7f8',
        orderDate: '0001-01-01T00:00:00+00:00',
        name: 'Co-codamol 8mg/500mg capsules',
        dosage: 'One To Be Taken Four Times A Day',
        quantity: '20 capsule',
        selected: false,
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

    expect(state.prescriptionCourses).toEqual([]);
    expect(state.hasLoaded).toEqual(false);
  });
});
