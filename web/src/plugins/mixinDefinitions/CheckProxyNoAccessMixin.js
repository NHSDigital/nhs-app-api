import get from 'lodash/fp/get';
import { findByName } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'CheckProxyNoAccessMixin',
  mounted() {
    if (this.$store.getters['session/isProxying']
       && this.$store.getters['errors/showApiError']
       && get('$store.state.errors.apiErrors[0].status')(this) === 403) {
      const route = findByName(this.$router.currentRoute.name);
      if (route && route.proxyShutterPath) {
        redirectTo(this, route.proxyShutterPath);
      }
    }
  },
};
