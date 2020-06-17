import { LOGIN_BIOMETRIC_ERROR_PATH } from '@/router/paths';
import { LOGIN_NAME } from '@/router/names';
import { redirectTo } from '@/lib/utils';

export default {
  handleBiometricLoginFailure() {
    if (this.app.$router.currentRoute.name === LOGIN_NAME) {
      redirectTo({ $router: this.app.$router, $store: this }, LOGIN_BIOMETRIC_ERROR_PATH);
    }
  },
};
