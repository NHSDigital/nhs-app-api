<template>
  <header v-if="showHeader" :class="[$style.slim]">
    <h1 :class="[$style.h1]"><slot/></h1>
    <a @click="performLogout()">
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
  props: {
    showInNative: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    showHeader() {
      if (this.showInNative) {
        return true;
      }
      return !this.$store.state.device.isNativeApp;
    },
  },
  methods: {
    performLogout() {
      this.$store.dispatch('auth/logout');
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";

</style>
