<template>
  <div />
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import { REDIRECT_PARAMETER, TERMSANDCONDITIONS } from '@/lib/routes';

export default {
  name: '',
  layout: 'authReturn',
  async fetch({ redirect, route, store }) {
    if (process.server) {
      await store.dispatch('appVersion/init');
      const appVersion = store.app.$env.VERSION_TAG;
      if (appVersion) {
        store.dispatch('appVersion/updateWebVersion', appVersion);
      }
      await store.dispatch('auth/handleAuthResponse', route.query.code);
      if (isEmpty(store.state.errors.apiErrors)) {
        const query = route.query.state.length > 1
          ? { [REDIRECT_PARAMETER]: route.query.state }
          : {};
        redirect(TERMSANDCONDITIONS.path, query);
      }
    }
  },
};
</script>
