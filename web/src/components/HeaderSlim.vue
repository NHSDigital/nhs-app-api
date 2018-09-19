<template>
  <header v-if="showHeader" :class="[$style.slim]">
    <h1 :class="[$style.h1]"><slot/></h1>
    <a :class="$style['focus-child-svg']" tabindex="0" @click="performLogout()"
       @focus="focus" @blur="blur">
      <back-icon :class="backButtonFocus"/>
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
  data() {
    return {
      backButtonFocused: false,
    };
  },
  computed: {
    showHeader() {
      if (this.showInNative) {
        return true;
      }
      return !this.$store.state.device.isNativeApp;
    },
    backButtonFocus() {
      return { addFocus: this.backButtonFocused, altFocus: true };
    },
  },
  methods: {
    performLogout() {
      this.$store.dispatch('auth/logout');
    },
    focus() {
      this.backButtonFocused = true;
    },
    blur() {
      this.backButtonFocused = false;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";
@import "../style/accessibility";

</style>
