/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import { ALLERGIES_LOADED, DEMOGRAPHICS_LOADED } from './mutation-types';

export default {
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
