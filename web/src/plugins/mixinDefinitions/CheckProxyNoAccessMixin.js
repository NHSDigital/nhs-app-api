import get from 'lodash/fp/get';
import { findByPath } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'CheckProxyNoAccessMixin',
  mounted() {
    if (this.$store.getters['session/isProxying']
       && this.$store.getters['errors/showApiError']
       && get('$store.state.errors.apiErrors[0].status')(this) === 403) {
      const route = findByPath(this.$store.state.errors.routePath);
      if (route && route.proxyShutterPath) {
        redirectTo(this, route.proxyShutterPath);
      }
    }
  },
};
