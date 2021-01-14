<template>
  <!--eslint-disable-next-line  vue/this-in-template -->
  <svg v-if="disableTabFocus"
       :class="iconClasses"
       :viewBox="viewBox"
       :preserveAspectRatio="preserveAspectRatio"
       xmlns="http://www.w3.org/2000/svg">
    <slot/>
  </svg>
  <svg v-else
       v-tabbing="iconClasses"
       :class="getStyleClasses"
       :viewBox="viewBox"
       :preserveAspectRatio="preserveAspectRatio"
       tabindex="-1"
       xmlns="http://www.w3.org/2000/svg">
    <slot/>
  </svg>
</template>

<script>
import TabFocusMixin from './TabFocusMixin';

export default {
  name: 'TabFocusedIcon',
  mixins: [TabFocusMixin.tabMixin],
  props: {
    iconClasses: {
      type: Array,
      default: () => [],
    },
    viewBox: {
      type: String,
      default: '0 0 0 0',
    },
    preserveAspectRatio: {
      type: String,
      default: 'none',
    },
    disableTabFocus: {
      type: Boolean,
      default: false,
    },
  },
  created() {
    this.stylingBinding = this.iconClasses;
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/tab-focused-icon";
</style>
