import {
  mutations,
  DEMOGRAPHICS_LOADED,
  ALLERGIES_LOADED,
} from '../../../../../src/store/myRecord';

describe('DEMOGRAPHICS_LOADED', () => {
  it('will set the demographics data on the state to the received data', () => {
    const state = {};
    const recievedData = {
      response: {
        nhsNumber: '123456789',
        firstName: 'Billy',
        surname: 'Bob',
        dateOfBirth: '14/04/1995',
        sex: 'male',
        address: {
          line1: '12 Fake Street',
          line2: 'Apartment 24',
          line3: '',
          town: 'Belfast',
          county: 'Northern Ireland',
          postcode: 'BT19 345',
        },
      },
    };
    const expectedData = {
      nhsNumber: '123456789',
      firstName: 'Billy',
      surname: 'Bob',
      dateOfBirth: '14/04/1995',
      sex: 'male',
      address: {
        line1: '12 Fake Street',
        line2: 'Apartment 24',
        line3: '',
        town: 'Belfast',
        county: 'Northern Ireland',
        postcode: 'BT19 345',
      },
    };

    mutations[DEMOGRAPHICS_LOADED](state, recievedData);
    expect(state.patientDemographics).toEqual(expectedData);
    expect(state.demographicsHasLoaded).toEqual(true);
  });
});

describe('ALLERGIES_LOADED', () => {
  it('will set the allergies data on the state to the received data', () => {
    const state = {};
    const recievedData = {
      allergies:
        [
          { name: 'Test Name', symptom: 'Test Symptom 1', date: '2009-09-15T13:45:30.0000000Z' },
          { name: 'Test Name 2', symptom: 'Test Symptom 2', date: '2009-06-15T13:45:30.0000000Z' },
        ],
    };
    const expectedData = [
      { name: 'Test Name', symptom: 'Test Symptom 1', date: '2009-09-15T13:45:30.0000000Z' },
      { name: 'Test Name 2', symptom: 'Test Symptom 2', date: '2009-06-15T13:45:30.0000000Z' },
    ];

    mutations[ALLERGIES_LOADED](state, recievedData);
    expect(state.allergies).toEqual(expectedData);
    expect(state.allergiesHasLoaded).toEqual(true);
  });
});
