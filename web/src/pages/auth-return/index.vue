<template>
  <div />
</template>

<script>
import { isEmpty } from 'lodash/fp';
import NativeCallbacks from '@/services/native-app';
import { TERMSANDCONDITIONS } from '@/lib/routes';

export default {
  name: '',
  layout: 'authReturn',
  async fetch(context) {
    if (process.server) {
      await context.store.dispatch('auth/handleAuthResponse', context.route.query.code);
      if (isEmpty(context.store.state.errors.apiErrors)) {
        return context.redirect(TERMSANDCONDITIONS.path);
      }
    }
    return undefined;
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
      NativeCallbacks.hideWhiteScreen();
    }
  },
  created() {
  },
};
</script>
