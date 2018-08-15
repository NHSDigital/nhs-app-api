<template>
  <svg :class="[$style.menu, selected?$style.selected:undefined]" xmlns="http://www.w3.org/2000/svg"
       viewBox="0 0 110.39 110.39">
    <defs/>
    <title>More</title>
    <g>
      <rect :class="$style.canvas" width="110.39" height="110.39"/>
      <g>
        <circle :class="$style['menu_stroke']" cx="33.85" cy="55.2" r="5.89"/>
        <circle :class="$style['menu_stroke']" cx="55.2" cy="55.2" r="5.89"/>
        <circle :class="$style['menu_stroke']" cx="76.54" cy="55.2" r="5.89"/>
      </g>
    </g>
  </svg>
</template>

<script>
export default {
  props: {
    selected: {
      type: Boolean,
      default: false,
    },
  },
};
</script>

<style lang="scss" module scoped>
@import "../../style/colours";
@import "../../style/navmenuicons";
svg.menu {
  &.selected {
    .menu_stroke {
      fill: $nhs_blue;
    }
  }
}
</style>
