import actions from '@/store/modules/loginSettings/actions';
import { LOGIN_SETTINGS_ERROR_PATH } from '@/router/paths';
import { LOGIN_SETTINGS_NAME } from '@/router/names';
import NativeCallbacks from '@/services/native-app';
import { SET_WAITING,
  CLEAR_ERROR_CODE,
  UPDATE_REGISTRATION_STATUS,
  UPDATE_BIOMETRIC_TYPE,
  ADD_ERROR_CODE } from '@/store/modules/loginSettings/mutation-types';
import { redirectTo } from '@/lib/utils';
import { createRouter } from '../../../helpers';

jest.mock('@/services/native-app');
jest.mock('@/lib/utils');

const mockAccessToken = 'MockAccessToken';

describe('loginSettings actions', () => {
  describe('updateRegistration', () => {
    let commit;
    beforeEach(async () => {
      commit = jest.fn();
      actions.dispatch = jest.fn();
      actions.app = {
        $cookies: {
          get: (cookieName) => {
            switch (cookieName) {
              case 'nhso.session':
                return {
                  accessToken: mockAccessToken,
                };
              default:
                return undefined;
            }
          },
        },
      };
      await actions.updateRegistration({ commit });
    });
    it('will call commit with SET_WAITING to true', () => {
      expect(commit).toBeCalledWith(SET_WAITING, true);
    });

    it('will dispatch auth/ensureAccessToken', () => {
      expect(actions.dispatch).toBeCalledWith('auth/ensureAccessToken');
    });

    it('will call NativeCallbacks.updateBiometricRegistrationWithToken with the access token', () => {
      expect(NativeCallbacks.updateBiometricRegistrationWithToken).toBeCalledWith(mockAccessToken);
    });
  });

  describe('clearErrorCode', () => {
    let commit;
    beforeEach(() => {
      commit = jest.fn();
      actions.clearErrorCode({ commit });
    });
    it('will call commit to clear error code', () => {
      expect(commit).toHaveBeenCalledWith(CLEAR_ERROR_CODE);
    });
  });

  describe('biometricSpec', () => {
    describe('enabled', () => {
      let commit;
      const deviceResponse =
        { biometricTypeReference: 'loginSettings.biometrics.biometricType.fingerPrint',
          enabled: true,
        };
      const biometricTypeReference = 'loginSettings.biometrics.biometricType.fingerPrint';
      beforeEach(() => {
        commit = jest.fn();
        actions.biometricSpec({ commit }, deviceResponse);
      });
      it('will call commit to update the biometric registration status as enabled', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_REGISTRATION_STATUS, true);
      });
      it('will call commit to update the biometric type', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_BIOMETRIC_TYPE, biometricTypeReference);
      });
    });
    describe('disabled', () => {
      let commit;
      const deviceResponse =
        { biometricTypeReference: 'loginSettings.biometrics.biometricType.fingerPrint',
          enabled: false,
        };
      const biometricTypeReference = 'loginSettings.biometrics.biometricType.fingerPrint';
      beforeEach(() => {
        commit = jest.fn();
        actions.biometricSpec({ commit }, deviceResponse);
      });
      it('will call commit to update the biometric registration status as disabled', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_REGISTRATION_STATUS, false);
      });
      it('will call commit to update the biometric type', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_BIOMETRIC_TYPE, biometricTypeReference);
      });
    });
  });

  describe('biometricCompletion', () => {
    describe('register', () => {
      let commit;
      const deviceResponse =
        { action: 'Register', outcome: 'Success', errorCode: '' };
      beforeEach(() => {
        commit = jest.fn();
        actions.biometricCompletion({ commit }, deviceResponse);
      });
      it('will call commit to set waiting to false', () => {
        expect(commit).toHaveBeenCalledWith(SET_WAITING, false);
      });

      it('will call commit to update registration status to true', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_REGISTRATION_STATUS, true);
      });
    });

    describe('deregister', () => {
      let commit;
      const deviceResponse =
        { action: 'Deregister', outcome: 'Success', errorCode: '' };
      beforeEach(() => {
        commit = jest.fn();
        actions.biometricCompletion({ commit }, deviceResponse);
      });
      it('will call commit to set waiting to false', () => {
        expect(commit).toHaveBeenCalledWith(SET_WAITING, false);
      });

      it('will call commit to update registration status to false', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_REGISTRATION_STATUS, false);
      });
    });

    describe('error', () => {
      let $router;

      beforeEach(() => {
        redirectTo.mockClear();
      });

      describe('10004', () => {
        let commit;
        const deviceResponse = { action: 'Deregister', outcome: 'Failed', errorCode: '10004' };

        beforeEach(() => {
          commit = jest.fn();
          $router = createRouter(LOGIN_SETTINGS_NAME);
          actions.app = { $router };
          actions.biometricCompletion({ commit }, deviceResponse);
        });

        it('will call commit to set waiting to false', () => {
          expect(commit).toHaveBeenCalledWith(SET_WAITING, false);
        });

        it('will redirect user to the login setting error page', () => {
          expect(redirectTo).toHaveBeenCalledWith({
            $router, $store: actions,
          }, LOGIN_SETTINGS_ERROR_PATH);
        });

        it('will add the error code that was received', () => {
          expect(commit).toHaveBeenCalledWith(ADD_ERROR_CODE, '10004');
        });
      });

      describe('10005', () => {
        let commit;
        const deviceResponse =
          { action: 'Deregister', outcome: 'Failed', errorCode: '10005' };
        beforeEach(() => {
          commit = jest.fn();
          $router = createRouter(LOGIN_SETTINGS_NAME);
          actions.app = { $router };
          actions.biometricCompletion({ commit }, deviceResponse);
        });

        it('will call commit to set waiting to false', () => {
          expect(commit).toHaveBeenCalledWith(SET_WAITING, false);
        });

        it('will redirect user to the login setting error page', () => {
          expect(redirectTo).toHaveBeenCalledWith({
            $router, $store: actions,
          }, LOGIN_SETTINGS_ERROR_PATH);
        });

        it('will add the error code that was received', () => {
          expect(commit).toHaveBeenCalledWith(ADD_ERROR_CODE, '10005');
        });
      });

      describe('unknown error code', () => {
        let commit;
        const deviceResponse =
          { action: 'Deregister', outcome: 'Failed', errorCode: '10006' };
        beforeEach(() => {
          commit = jest.fn();
          actions.dispatch = jest.fn();
          actions.biometricCompletion({ commit }, deviceResponse);
        });

        it('will call commit to set waiting to false', () => {
          expect(commit).toHaveBeenCalledWith(SET_WAITING, false);
        });

        it('will add the API error', () => {
          expect(actions.dispatch).toBeCalledWith('errors/addApiError', {
            message: 'Unknown error occurred while updating biometric registration status',
            response: {
              data: {
                errorCode: '10006',
              },
              status: 500,
            },
          });
        });
      });
    });
  });
});
