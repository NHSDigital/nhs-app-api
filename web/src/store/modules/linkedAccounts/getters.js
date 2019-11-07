import get from 'lodash/fp/get';

export default {
  hasLinkedAccounts(state) {
    return get('config.hasLinkedAccounts')(state);
  },
  mainPatientId(state) {
    return get('config.patientId')(state);
  },
};
