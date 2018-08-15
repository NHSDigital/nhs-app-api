<template>
  <header v-if="showHeader" :class="[$style.slim]">
    <h1 :class="[$style.h1]"> {{ $t('sy01.pageHeader') }}</h1>
    <a href="/login">
      <back-icon/>
    </a>
  </header>
</template>


<script>
/* eslint-disable import/extensions */
import BackIcon from '@/components/icons/BackIcon';

export default {
  components: {
    BackIcon,
  },
  computed: {
    showHeader() {
      return (
        !this.$store.state.device.isNativeApp
      );
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";

</style>
