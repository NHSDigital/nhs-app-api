<template>
  <div/>
</template>

<script>
import get from 'lodash/fp/get';
import { getRedirectRoute, INDEX, INTERSTITIAL_REDIRECTOR } from '@/lib/routes';

export default {
  created() {
    const redirectPath = get('path')(getRedirectRoute(this.$route));
    const path = redirectPath === INTERSTITIAL_REDIRECTOR.path
      ? INDEX.path
      : redirectPath || INDEX.path;
    this.$router.push(path);
  },
};
</script>
