<template>
  <auth-return-layout>
    <div />
  </auth-return-layout>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import { TERMSANDCONDITIONS_PATH } from '@/router/paths';
import { REDIRECT_PARAMETER } from '@/router/names';
import AuthReturnLayout from '@/layouts/authReturn';

export default {
  name: 'AuthReturnPage',
  components: {
    AuthReturnLayout,
  },
  async mounted() {
    const route = this.$router.currentRoute;
    await this.$store.dispatch('appVersion/init');
    const appVersion = this.$store.$env.VERSION_TAG;
    if (appVersion) {
      this.$store.dispatch('appVersion/updateWebVersion', appVersion);
    }
    await this.$store.dispatch('auth/handleAuthResponse', route.query.code);
    if (isEmpty(this.$store.state.errors.apiErrors)) {
      const query = route.query.state.length > 1
        ? { [REDIRECT_PARAMETER]: route.query.state }
        : {};

      this.$router.push({ path: TERMSANDCONDITIONS_PATH, query });
    }
  },
};
</script>
