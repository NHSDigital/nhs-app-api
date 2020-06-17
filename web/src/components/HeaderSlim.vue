<template>
  <header v-if="showHeader" :class="[$style.slim]">
    <h1 :class="[$style.h1, 'nhsuk-u-margin-top-1']"><slot/></h1>
    <form :action="backUrl" method="get">
      <button tabindex="0" @click.prevent="goBack" @keypress.enter.prevent="goBack">
        <back-icon/>
      </button>
    </form>
  </header>
</template>


<script>
/* eslint-disable import/extensions */
import BackIcon from '@/components/icons/BackIcon';
import { LOGOUT_PATH } from '@/router/paths';

export default {
  name: 'HeaderSlim',
  components: {
    BackIcon,
  },
  props: {
    showInDesktop: {
      type: Boolean,
      default: true,
    },
    showInNative: {
      type: Boolean,
      default: false,
    },
    clickUrl: {
      type: String,
      default: LOGOUT_PATH,
    },
  },
  computed: {
    backUrl() {
      return this.correctUrl(this.clickUrl);
    },
    showHeader() {
      return (this.showInNative && this.$store.state.device.isNativeApp)
          || (this.showInDesktop && !this.$store.state.device.isNativeApp);
    },
  },
  methods: {
    goBack() {
      this.goToUrl(this.backUrl);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";
</style>
