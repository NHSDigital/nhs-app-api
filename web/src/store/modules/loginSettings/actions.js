import {
  SET_WAITING,
  UPDATE_BIOMETRIC_TYPE,
  UPDATE_REGISTRATION_STATUS,
  ADD_ERROR_CODE,
  CLEAR_ERROR_CODE,
} from './mutation-types';
import { LOGIN_SETTINGS_ERROR } from '@/lib/routes';
import NativeApp from '@/services/native-app';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import biometricActions from '@/lib/biometrics/biometricActions';
import biometricRegistrationOutcomes from '@/lib/biometrics/biometricRegistrationOutcomes';

const addApiError = ({ dispatch }, statusCode, errorCode, message) => dispatch('errors/addApiError', {
  message,
  response: {
    status: statusCode,
    data: {
      errorCode,
    },
  },
});

export default {
  updateRegistration({ commit }) {
    commit(SET_WAITING, true);
    NativeApp.updateBiometricRegistration();
  },

  biometricCompletion({ commit }, deviceResponse) {
    commit(SET_WAITING, false);
    const { action, outcome, errorCode } = deviceResponse;

    if (outcome === biometricRegistrationOutcomes.Failure) {
      if (errorCode === biometricErrorCodes.CannotFindBiometrics
        || errorCode === biometricErrorCodes.CannotChangeBiometrics) {
        this.$router.push(LOGIN_SETTINGS_ERROR.path);
        commit(ADD_ERROR_CODE, errorCode);
      } else {
        addApiError(this,
          500,
          biometricErrorCodes.Unknown,
          'Unknown error occurred while updating biometric registration status');
      }

      return;
    }

    if (outcome === biometricRegistrationOutcomes.Success) {
      if (action === biometricActions.Register) {
        commit(UPDATE_REGISTRATION_STATUS, true);
      } else if (action === biometricActions.Deregister) {
        commit(UPDATE_REGISTRATION_STATUS, false);
      }
    }
  },

  biometricSpec({ commit }, deviceResponse) {
    const { biometricTypeReference, enabled } = deviceResponse;
    commit(UPDATE_REGISTRATION_STATUS, enabled);
    commit(UPDATE_BIOMETRIC_TYPE, biometricTypeReference);
  },

  clearErrorCode({ commit }) {
    commit(CLEAR_ERROR_CODE);
  },
};
