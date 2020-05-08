import { LOGIN_BIOMETRIC_ERROR, LOGIN } from '@/lib/routes';

export default {
  handleBiometricLoginFailure() {
    if (this.$router.history.current.name === LOGIN.name) {
      this.$router.push(LOGIN_BIOMETRIC_ERROR.path);
    }
  },
};
