/* eslint-disable import/extensions */
import mutations from '../../../../../src/store/modules/repeatPrescriptionCourses/mutations';
import { REPEAT_PRESCRIPTION_COURSES_LOADED } from '../../../../../src/store/modules/repeatPrescriptionCourses/mutation-types';


describe('REPEAT_PRESCRIPTION_COURSES_LOADED', () => {
  it('will set the courses on the state from the received courses data', () => {
    const state = {};
    const receivedData = {
      repeatPrescriptionCourses: [{
        response: {
          prescriptions: [
            {
              courses: [
                {
                  courseId: '2aae6aec-8326-4b46-95d3-75192109b3b5',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '4344c646-6e1a-4278-835b-98b5adec268c',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '4c937561-a865-46fa-8499-ad2f22b9dbc8',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '9774f74a-98b5-44eb-ac4e-7581ab1e0676',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '6d2bd499-03c4-46ea-b142-ce23f14acd20',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: 'f02f5e89-8870-4c71-97da-9843c667a702',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: 'c0174fbc-22d3-4b0b-86de-893c9ef17711',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '1aa482a2-5125-4cfa-a424-c232c3462e86',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '5bbc3783-c7d8-4a7a-9e0e-d224befdf166',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.77+00:00',
            },
            {
              courses: [
                {
                  courseId: '214c5b98-0724-4326-93ec-9553db985163',
                  status: 0,
                },
              ],
              orderDate: '2018-06-01T09:46:52.765+00:00',
            },
          ],
          courses: [
            {
              id: '5bbc3783-c7d8-4a7a-9e0e-d224befdf166',
              name: 'Choline salicylate 8.7% oromucosal gel sugar free',
              dosage: 'One To Be Taken Three Times A Day',
              quantity: '49 tablet',
            },
            {
              id: '1aa482a2-5125-4cfa-a424-c232c3462e86',
              name: 'Codine 200mg tablets',
              dosage: 'One To Be Taken Three Times A Day',
              quantity: '82  ml',
            },
            {
              id: 'c0174fbc-22d3-4b0b-86de-893c9ef17711',
              name: 'Choline salicylate 8.7% oromucosal gel sugar free',
              dosage: 'Two To Be Taken Four Times A Day',
              quantity: '7  ml',
            },
            {
              id: 'f02f5e89-8870-4c71-97da-9843c667a702',
              name: 'Paracetamol 150mg oral tablets',
              dosage: 'One To Be Taken Weekly',
              quantity: '85 tablet',
            },
            {
              id: '6d2bd499-03c4-46ea-b142-ce23f14acd20',
              name: 'Penicillin 150mg oral tablets',
              dosage: 'One To Be Taken Weekly',
              quantity: '32  ml',
            },
            {
              id: '9774f74a-98b5-44eb-ac4e-7581ab1e0676',
              name: 'Choline salicylate 8.7% oromucosal gel sugar free',
              dosage: 'One To Be Taken Weekly',
              quantity: '17  ml',
            },
            {
              id: '4c937561-a865-46fa-8499-ad2f22b9dbc8',
              name: 'Codine 200mg tablets',
              dosage: 'One To Be Take Every Evening',
              quantity: '79  ml',
            },
            {
              id: '4344c646-6e1a-4278-835b-98b5adec268c',
              name: 'Codine 200mg tablets',
              dosage: 'One To Be Taken Three Times A Day',
              quantity: '48 tablet',
            },
            {
              id: '2aae6aec-8326-4b46-95d3-75192109b3b5',
              name: 'Penicillin 150mg oral tablets',
              dosage: 'Two To Be Taken Four Times A Day',
              quantity: '26  ml',
            },
            {
              id: '214c5b98-0724-4326-93ec-9553db985163',
              name: 'Paracetamol 150mg oral tablets',
              dosage: 'One To Be Taken Weekly',
              quantity: '97  ml',
            },
          ],
        },
      },
      ],
    };

    mutations[REPEAT_PRESCRIPTION_COURSES_LOADED](state, receivedData);

    expect(state.repeatPrescriptionCourses).toEqual(receivedData.repeatPrescriptionCourses);
  });
});
