<script>
import get from 'lodash/fp/get';
import {
  REDIRECT_PARAMETER,
  isNhsAppRouteName,
} from '@/router/names';
import {
  INTERSTITIAL_REDIRECTOR_PATH,
  INDEX_PATH,
} from '@/router/paths';
import { redirectTo, redirectByName } from '@/lib/utils';
import NativeApp from '@/services/native-app';

export default {
  methods: {
    conditionalRedirect() {
      if (NativeApp.goToLoggedInHomeScreen()) {
        return;
      }
      const redirectName = get(REDIRECT_PARAMETER)(this.$route.query);
      if (redirectName) {
        if (isNhsAppRouteName(redirectName)) {
          redirectByName(this, redirectName);
          return;
        }
        redirectTo(this, INTERSTITIAL_REDIRECTOR_PATH, this.$route.query);
        return;
      }
      redirectTo(this, INDEX_PATH);
    },
  },
};
</script>
