
import get from 'lodash/fp/get';
import { redirectTo } from '@/lib/utils';

export default function showShutterPage(currentRoute, self) {
  if (self.$store.getters['session/isProxying']
        && self.$store.getters['errors/showApiError']
        && get('$store.state.errors.apiErrors[0].status')(self) === 403) {
    if (currentRoute && currentRoute.proxyShutterPath) {
      self.$store.dispatch('errors/clearAllApiErrors');
      redirectTo(self, currentRoute.proxyShutterPath);
    }
  }
}
