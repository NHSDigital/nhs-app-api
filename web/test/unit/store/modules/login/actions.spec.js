import actions from '@/store/modules/login/actions';
import { LOGIN_NAME, APPOINTMENTS_NAME } from '@/router/names';
import { LOGIN_BIOMETRIC_ERROR_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createRouter } from '../../../helpers';

jest.mock('@/services/native-app');
jest.mock('@/lib/utils');

describe('handleBiometricLoginFailure', () => {
  let $router;

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('Login route', () => {
    beforeEach(() => {
      $router = createRouter(LOGIN_NAME);
      actions.app = { $router };
      actions.handleBiometricLoginFailure();
    });

    it('will redirectTo biometric error path', () => {
      expect(redirectTo).toHaveBeenCalledWith({
        $router, $store: actions,
      }, LOGIN_BIOMETRIC_ERROR_PATH);
    });
  });

  describe('appointments route', () => {
    beforeEach(() => {
      $router = createRouter(APPOINTMENTS_NAME);
      actions.app = { $router };
      actions.handleBiometricLoginFailure();
    });

    it('will not call redirectTo', () => {
      expect(redirectTo).not.toHaveBeenCalled();
    });
  });
});
