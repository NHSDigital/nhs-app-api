export const ALLERGIES_LOADED = 'ALLERGIES_LOADED';
export const DEMOGRAPHICS_LOADED = 'DEMOGRAPHICS_LOADED';
/* eslint-disable no-shadow */

export const state = () => ({
  allergies: [],
  allergiesHasLoaded: false,
  demographicsHasLoaded: false,
  patientDemographics: null,
});

function getTestPatientAllergies() {
  return {
    allergies:
    [
      { name: 'Test Name', symptom: 'Test Symptom 1', date: '2009-09-15T13:45:30.0000000Z' },
      { name: 'Test Name 2', symptom: 'Test Symptom 2', date: '2009-06-15T13:45:30.0000000Z' },
    ],
  };
}

export const actions = {
  loadAllergiesAndAdverseReactions({ commit }) {
    try {
      return this.app.$http
        .getV1PatientMyRecordAllergies({})
        .then((data) => {
          commit(ALLERGIES_LOADED, data);
        });
    } catch (err) {
      return commit(ALLERGIES_LOADED, getTestPatientAllergies());
    }
  },
  loadPatientDemographics({ commit }) {
    return this.app.$http
      .getV1PatientMyRecordDemographics({})
      .then((data) => {
        commit(DEMOGRAPHICS_LOADED, data);
      });
  },
};

export const mutations = {
  [ALLERGIES_LOADED](state, data) {
    state.allergies = data.allergies;
    state.allergiesHasLoaded = true;
  },
  [DEMOGRAPHICS_LOADED](state, data) {
    if (data != null) {
      const patientDemoResponse = data.response;
      const address = {
        line1: patientDemoResponse.address.line1,
        line2: patientDemoResponse.address.line2,
        line3: patientDemoResponse.address.line3,
        town: patientDemoResponse.address.town,
        county: patientDemoResponse.address.county,
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
    } else {
      state.patientDemographics = null;
    }
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
