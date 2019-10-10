<template>
  <header v-if="showHeader" :class="[$style.slim]">
    <h1 :class="[$style.h1, 'nhsuk-u-margin-top-1']"><slot/></h1>
    <form :action="backUrl" method="get">
      <button tabindex="0" @click="backClick($event)" @keypress="keyPress($event)">
        <back-icon/>
      </button>
    </form>
  </header>
</template>


<script>
/* eslint-disable import/extensions */
import BackIcon from '@/components/icons/BackIcon';
import { ACCOUNT_SIGNOUT } from '@/lib/routes';

const ENTER_KEY_CODE = 13;

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
      default: ACCOUNT_SIGNOUT.path,
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
    backClick(event) {
      event.preventDefault();
      this.goToUrl(this.backUrl);
    },
    keyPress(event) {
      if (event.keyCode === ENTER_KEY_CODE) {
        this.backClick(event);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";
</style>
