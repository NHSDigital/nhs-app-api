/* eslint-disable import/extensions */
import mutations from '../../../../../src/store/modules/repeatPrescriptionCourses/mutations';
import { REPEAT_PRESCRIPTION_COURSES_LOADED, REPEAT_PRESCRIPTION_ORDER_SUCCESS, REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO } from '../../../../../src/store/modules/repeatPrescriptionCourses/mutation-types';


describe('REPEAT_PRESCRIPTION_COURSES_LOADED', () => {
  it('will set the courses on the state from the received courses data', () => {
    const state = {};
    const receivedData = {
      courses: [
        {
          id: '5bbc3783-c7d8-4a7a-9e0e-d224befdf166',
          name: 'Choline salicylate 8.7% oromucosal gel sugar free',
          details: '49 tablet - One To Be Taken Three Times A Day',
          selected: false,
        },
        {
          id: '1aa482a2-5125-4cfa-a424-c232c3462e86',
          name: 'Codine 200mg tablets',
          details: '82  ml - One To Be Taken Three Times A Day',
          selected: false,
        },
        {
          id: 'c0174fbc-22d3-4b0b-86de-893c9ef17711',
          name: 'Choline salicylate 8.7% oromucosal gel sugar free',
          details: '7  ml - Two To Be Taken Four Times A Day',
          selected: false,
        },
        {
          id: 'f02f5e89-8870-4c71-97da-9843c667a702',
          name: 'Paracetamol 150mg oral tablets',
          details: '85 tablet - One To Be Taken Weekly',
          selected: false,
        },
        {
          id: '6d2bd499-03c4-46ea-b142-ce23f14acd20',
          name: 'Penicillin 150mg oral tablets',
          details: '32  ml - One To Be Taken Weekly',
          selected: false,
        },
        {
          id: '9774f74a-98b5-44eb-ac4e-7581ab1e0676',
          name: 'Choline salicylate 8.7% oromucosal gel sugar free',
          details: '17  ml - One To Be Taken Weekly',
          selected: false,
        },
        {
          id: '4c937561-a865-46fa-8499-ad2f22b9dbc8',
          name: 'Codine 200mg tablets',
          details: '79  ml - One To Be Take Every Evening',
          selected: false,
        },
        {
          id: '4344c646-6e1a-4278-835b-98b5adec268c',
          name: 'Codine 200mg tablets',
          details: '48 tablet - One To Be Taken Three Times A Day',
          selected: false,
        },
        {
          id: '2aae6aec-8326-4b46-95d3-75192109b3b5',
          name: 'Penicillin 150mg oral tablets',
          details: '26  ml - Two To Be Taken Four Times A Day',
          selected: false,
        },
        {
          id: '214c5b98-0724-4326-93ec-9553db985163',
          name: 'Paracetamol 150mg oral tablets',
          details: '97  ml - One To Be Taken Weekly',
          selected: false,
        },
      ],
    };

    mutations[REPEAT_PRESCRIPTION_COURSES_LOADED](state, receivedData);

    expect(state.repeatPrescriptionCourses).toEqual(receivedData.courses);
  });
});

describe('REPEAT_PRESCRIPTION_ORDER_SUCCESS', () => {
  it('will set the justOrderedARepeatPrescription on the state to true', () => {
    const state = {};

    mutations[REPEAT_PRESCRIPTION_ORDER_SUCCESS](state);

    expect(state.justOrderedARepeatPrescription).toEqual(true);
  });
});

describe('REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO', () => {
  it('will set the appropriate data on the state from the source', () => {
    const state = {};

    const additionalInfo = {
      specialRequest: 'Please call me when prescription is ready',
    };

    mutations[REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO](state, additionalInfo);

    expect(state.specialRequest).toEqual(additionalInfo.specialRequest);
  });
});
