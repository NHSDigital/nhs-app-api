<template>
  <div />
</template>

<script>
import { isEmpty } from 'lodash/fp';
import { TERMSANDCONDITIONS } from '@/lib/routes';

export default {
  name: '',
  layout: 'authReturn',
  async fetch(context) {
    if (process.server) {
      await context.store.dispatch('appVersion/init');
      const appVersion = context.store.app.$env.VERSION_TAG;
      if (appVersion) {
        context.store.dispatch('appVersion/updateWebVersion', appVersion);
      }

      await context.store.dispatch('auth/handleAuthResponse', context.route.query.code);
      if (isEmpty(context.store.state.errors.apiErrors)) {
        return context.redirect(TERMSANDCONDITIONS.path);
      }
    }
    return undefined;
  },
  mounted() {
  },
  created() {
  },
};
</script>
