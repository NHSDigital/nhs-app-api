<template>
  <svg :class="[$style.menu, selected?$style.selected:undefined]" xmlns="http://www.w3.org/2000/svg"
       viewBox="0 0 110.39 110.39">
    <defs/>
    <title>My record</title>
    <g>
      <rect :class="$style.canvas" width="110.39" height="110.39"/>
      <g>
        <rect :class="[$style['menu_stroke'], $style.active]" x="18.4" y="15.53"
              width="63.51" height="79.34"/>
        <polyline :class="$style['menu_stroke']"
                  points="81.91 71.71 91.99 71.71 91.99 15.53 81.91 15.53"/>
        <line :class="[$style['menu_stroke'], $style.white]" x1="31.83" y1="73.26"
              x2="68.47" y2="73.26"/>
        <line :class="[$style['menu_stroke'], $style.white]" x1="31.83" y1="82.22"
              x2="68.47" y2="82.22"/>
        <polygon :class="[$style['menu_stroke'], $style.white]"
                 points="55.55 50.3 55.55 58.06 44.76 58.06 44.76 50.3 37 50.3 37
                 39.51 44.76 39.51 44.76 31.75 55.55 31.75 55.55 39.51 63.31 39.51
                 63.31 50.3 55.55 50.3"/>
        <line :class="$style['menu_stroke']" x1="82.65" y1="52.98"
              x2="91.29" y2="52.98"/>
        <line :class="$style['menu_stroke']" x1="82.65" y1="34.58"
              x2="91.29" y2="34.58"/>
      </g>
    </g>
  </svg>
</template>

<script>
export default {
  name: 'MyRecordIcon',
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
    .active {
      fill: $nhs_blue;
    }
    .white {
      stroke: $white;
    }
  }
}
</style>
