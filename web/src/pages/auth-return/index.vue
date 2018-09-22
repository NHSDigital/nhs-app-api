<template>
  <div />
</template>

<script>
import { isEmpty } from 'lodash/fp';

export default {
  name: '',
  async fetch(context) {
    if (process.server) {
      await context.store.dispatch('auth/handleAuthResponse', context.route.query.code);
      if (isEmpty(context.store.state.errors.apiErrors)) {
        return context.redirect('/terms-and-conditions');
      }
    }
    return undefined;
  },
  created() {
  },
};
</script>
