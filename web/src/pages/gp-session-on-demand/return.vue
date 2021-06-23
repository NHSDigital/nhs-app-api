<template>
  <auth-return-layout>
    <div/>
  </auth-return-layout>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import { TERMSANDCONDITIONS_PATH } from '@/router/paths';
import { REDIRECT_PARAMETER } from '@/router/names';
import NativeReferrerSetup from '@/services/nativeReferrerSetup';
import AuthReturnLayout from '@/layouts/authReturn';

export default {
  name: 'OnDemandGpReturnPage',
  components: {
    AuthReturnLayout,
  },
  async mounted() {
    NativeReferrerSetup(this.$store);

    const route = this.$router.currentRoute;
    await this.$store.dispatch('appVersion/init');
    const appVersion = this.$store.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
    await this.$store.dispatch('auth/handleGpOnDemandResponse', route.query);
    if (isEmpty(this.$store.state.errors.apiErrors)) {
      this.$store.dispatch('session/setGpSession', true);

      const query = route.query.state.length > 1
        ? { [REDIRECT_PARAMETER]: route.query.state }
        : {};

      this.$router.push({ path: TERMSANDCONDITIONS_PATH, query });
    }
  },
};
</script>
