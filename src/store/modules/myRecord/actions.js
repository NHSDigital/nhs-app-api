import { ALLERGIES_LOADED, DEMOGRAPHICS_LOADED } from './mutation-types';

function getTestPatientAllergies() {
  return {
    allergies:
      [
        { name: 'Test Name', symptom: 'Test Symptom 1', date: '2009-09-15T13:45:30.0000000Z' },
        { name: 'Test Name 2', symptom: 'Test Symptom 2', date: '2009-06-15T13:45:30.0000000Z' },
      ],
  };
}

export default {
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
