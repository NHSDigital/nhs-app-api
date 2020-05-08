import { UPDATE_REGISTRATION_STATUS } from './mutation-types';

export default {
  [UPDATE_REGISTRATION_STATUS](state) {
    state.biometricsRegistrationStatus = !state.biometricsRegistrationStatus;
  },
};
