import get from 'lodash/fp/get';

export default {
  hasLinkedAccounts(state) {
    return get('config.hasLinkedAccounts')(state);
  },
  mainPatientId(state) {
    return get('config.patientId')(state);
  },
  isRecoveringFromProxyLoss(state) {
    return get('recoverFromProxyLoss')(state);
  },
  getSelectedLinkedAccount(state) {
    return get('selectedLinkedAccount')(state);
  },
};
