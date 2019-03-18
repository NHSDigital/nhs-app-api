<template>
  <!--eslint-disable-next-line  vue/this-in-template -->
  <svg :id="id"
       v-tabbing="iconClasses"
       :class="getStyleClasses"
       :viewBox="viewBox"
       :preserveAspectRatio="preserveAspectRatio"
       :tabindex="disableTabFocus ? undefined : '-1'"
       xmlns="http://www.w3.org/2000/svg">
    <slot/>
  </svg>
</template>

<script>

import TabFocusMixin from './TabFocusMixin';

export default {
  components: {
    TabFocusMixin,
  },
  mixins: [TabFocusMixin.tabMixin],
  props: {
    iconClasses: {
      type: Array,
      default: () => [],
    },
    id: {
      type: String,
      default: '',
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
@import "../../style/colours";
svg.tabFocus {
  outline-color: $focus_highlight !important;
  outline-style: solid !important;
  outline-width: thick;
}

svg.defaultStyle {
  margin: 0 0;
  display: inline-block;
  height: 0;
  width: auto;
  position: fixed;
  right: 0;
  top: 0;
  cursor: pointer;
  outline-style: none;
}
</style>
