import actions from '@/store/modules/login/actions';
import { createRouter } from '../../../helpers';
import { LOGIN, LOGIN_BIOMETRIC_ERROR, APPOINTMENTS } from '@/lib/routes';

jest.mock('@/services/native-app');

describe('handleBiometricLoginFailure', () => {
  describe('Login route', () => {
    beforeEach(async () => {
      actions.$router = createRouter(LOGIN.name);
      actions.$router.push(LOGIN.path);
      actions.handleBiometricLoginFailure();
    });
    it('will redirect to biometric login error page', () => {
      expect(actions.$router.push).toHaveBeenCalledWith(LOGIN_BIOMETRIC_ERROR.path);
    });
  });
  describe('appointments route', () => {
    beforeEach(async () => {
      actions.$router = createRouter(APPOINTMENTS.name);
      actions.$router.push(APPOINTMENTS.path);
      actions.handleBiometricLoginFailure();
    });
    it('will not redirect to biometric login error page', () => {
      expect(actions.$router.push).toHaveBeenLastCalledWith(APPOINTMENTS.path);
    });
  });
});
