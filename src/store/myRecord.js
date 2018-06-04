export const ALLERGIES_LOADED = 'ALLERGIES_LOADED';
export const DEMOGRAPHICS_LOADED = 'DEMOGRAPHICS_LOADED';

export const state = () => ({
  allergies: [],
  allergiesHasLoaded: false,
  demographicsHasLoaded: false,
  patientDemographics: null,
});

function getTestPatientAllergies() {
  return {
    allergies: [
      { name: 'Test Name', symptom: 'Test Symptom 1', date: '2009-09-15T13:45:30.0000000Z' },
      { name: 'Test Name 2', symptom: 'Test Symptom 2', date: '2009-06-15T13:45:30.0000000Z' },
    ],
  };
}

function getTestPatientDemographics() {
  return {
    nhsNumber: '123456789',
    firstName: 'Billy',
    surname: 'Bob',
    dateOfBirth: '1983-06-15T13:45:30.0000000Z',
    sex: 'male',
    address: {
      line1: '12 Fake Street ffffffffffffffx',
      line2: 'Apartment 24',
      line3: 'Fake Apartment Name',
      town: 'Belfast',
      country: 'Northern Ireland',
      postcode: 'BT19 345',
    },
  };
}

export const actions = {
  loadAllergiesAndAdverseReactions({ commit }) {
    const data = getTestPatientAllergies();
    commit(ALLERGIES_LOADED, data);
  },
  loadPatientDemographics({ commit }) {
    commit(DEMOGRAPHICS_LOADED, getTestPatientDemographics());
  },
};

/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [ALLERGIES_LOADED](state, data) {
    state.allergies = data.allergies;
    state.allergiesHasLoaded = true;
  },
  [DEMOGRAPHICS_LOADED](state, data) {
    const patientDemoResponse = data;
    const address = {
      line1: patientDemoResponse.address.line1,
      line2: patientDemoResponse.address.line2,
      line3: patientDemoResponse.address.line3,
      town: patientDemoResponse.address.town,
      country: patientDemoResponse.address.country,
      postcode: patientDemoResponse.address.postcode,
    };
    state.patientDemographics = {
      nhsNumber: patientDemoResponse.nhsNumber,
      firstName: patientDemoResponse.firstName,
      surname: patientDemoResponse.surname,
      dateOfBirth: patientDemoResponse.dateOfBirth,
      sex: patientDemoResponse.sex,
      address,
    };
    state.demographicsHasLoaded = true;
  },
};

export const getters = {
  allergies(state) {
    return state.allergies;
  },
  patientDemographics(state) {
    return state.patientDemographics;
  },
};
