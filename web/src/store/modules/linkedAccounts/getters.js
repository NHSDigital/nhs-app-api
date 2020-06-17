import get from 'lodash/fp/get';

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

const findPatientId = (state) => {
  const actingAsUser = get('actingAsUser')(state);
  if (actingAsUser) {
    return actingAsUser.id;
  }
  return get('config.patientId')(state);
};

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
  getPatientId(state) {
    return findPatientId(state);
  },
  isPatientIdNotEmpty(state) {
    const patientId = findPatientId(state);
    return patientId !== '' && patientId !== EMPTY_GUID;
  },
};
