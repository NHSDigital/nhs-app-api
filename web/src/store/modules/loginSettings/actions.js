import { MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_ERROR_PATH } from '@/router/paths';
import NativeApp from '@/services/native-app';
import biometricErrorCodes from '@/lib/biometrics/biometricErrorCodes';
import biometricActions from '@/lib/biometrics/biometricActions';
import biometricRegistrationOutcomes from '@/lib/biometrics/biometricRegistrationOutcomes';
import { redirectTo } from '@/lib/utils';
import {
  SET_WAITING,
  UPDATE_BIOMETRIC_TYPE,
  UPDATE_REGISTRATION_STATUS,
  ADD_ERROR_CODE,
  CLEAR_ERROR_CODE,
} from './mutation-types';

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
  fetchBiometricStatus({ state }) {
    if (state.biometricType === undefined) {
      if (NativeApp.supportsBiometricStatus()) {
        const { accessToken } = this.app.$cookies.get('nhso.session');
        NativeApp.fetchBiometricStatus(accessToken);
      } else {
        NativeApp.fetchBiometricSpec();
      }
    }
  },

  missingBiometricState({ state }) {
    if (NativeApp.supportsBiometricStatus()) {
      return state.biometricType === undefined;
    }

    // Legacy native apps do not call back if biometrics are not supported so
    // there is no way to determine if the state is initialised.
    return false;
  },

  biometricStatus({ commit }, biometricStatus) {
    const { biometricType, enabled } = biometricStatus;

    commit(UPDATE_REGISTRATION_STATUS, enabled);
    commit(UPDATE_BIOMETRIC_TYPE, biometricType);
  },

  async updateRegistration({ commit }) {
    commit(SET_WAITING, true);
    await this.dispatch('auth/ensureAccessToken');
    const { accessToken } = this.app.$cookies.get('nhso.session');
    NativeApp.updateBiometricRegistrationWithToken(accessToken);
  },

  biometricCompletion({ commit }, deviceResponse) {
    commit(SET_WAITING, false);

    let deviceResponseParam = deviceResponse;

    if (typeof deviceResponseParam !== 'object') {
      deviceResponseParam = JSON.parse(deviceResponse);
    }

    const { action, outcome, errorCode } = deviceResponseParam;

    if (outcome === biometricRegistrationOutcomes.Failure) {
      if (errorCode === biometricErrorCodes.CannotFindBiometrics
        || errorCode === biometricErrorCodes.CannotChangeBiometrics) {
        commit(ADD_ERROR_CODE, errorCode);
        redirectTo({ $router: this.app.$router, $store: this },
          MORE_ACCOUNTANDSETTINGS_LOGIN_SETTINGS_ERROR_PATH);
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
    let deviceResponseParam = deviceResponse;

    if (typeof deviceResponseParam !== 'object') {
      deviceResponseParam = JSON.parse(deviceResponse);
    }

    const { biometricTypeReference, enabled } = deviceResponseParam;
    commit(UPDATE_REGISTRATION_STATUS, enabled);
    commit(UPDATE_BIOMETRIC_TYPE, biometricTypeReference);
  },

  clearErrorCode({ commit }) {
    commit(CLEAR_ERROR_CODE);
  },
};
