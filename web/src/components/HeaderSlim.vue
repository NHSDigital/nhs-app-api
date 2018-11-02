<template>
  <header v-if="showHeader" :class="[$style.slim]">
    <h1 :class="[$style.h1]"><slot/></h1>
    <a tabindex="0" @click="performLogout()" @keypress="keyPress($event)">
      <back-icon/>
    </a>
  </header>
</template>


<script>
/* eslint-disable import/extensions */
import BackIcon from '@/components/icons/BackIcon';

const ENTER_KEY_CODE = 13;

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
    keyPress(event) {
      if (event.keyCode === ENTER_KEY_CODE) {
        event.preventDefault();
        this.performLogout();
      }
    },

  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";

</style>
