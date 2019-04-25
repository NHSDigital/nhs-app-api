<template>
  <svg :class="[$style.menu, selected?$style.selected:undefined]" xmlns="http://www.w3.org/2000/svg"
       viewBox="0 0 110.39 110.39">
    <defs/>
    <title>Prescriptions</title>
    <g>
      <rect :class="$style.canvas" width="110.39" height="110.39" />
      <g>
        <path :class="$style['menu_stroke']"
              d="M85.19,81.44,75.17,91a11.22,11.22,0,0,1-15.8-.36h0a11.21,
              11.21,0,0,1,.37-15.8l10-9.56" />
        <path :class="[$style['menu_stroke'], $style.active]"
              d="M69.82,65.22,85.25,81.38l10.09-9.64A11.21,11.21,0,0,0,95.7,
              56h0a11.21,11.21,0,0,0-15.8-.36Z" />
        <rect :class="[$style['menu_stroke'], $style.active]" x="22.47" y="16.31"
              width="41.94" height="19.57" rx="7" ry="7" />
        <path :class="$style['menu_stroke']"
              d="M29.79,35.88v5.56L16.51,49.16c-2.7,2-4.89,2.9-4.89,5.59v28a4.9,
              4.9,0,0,0,4.89,4.89H56.92" />
        <path :class="$style['menu_stroke']"
              d="M74.53,60.46V54.75c0-2.69-2.66-4-4.9-5.59L56,41.44V35.88" />
        <polygon :class="[$style['menu_stroke'], $style.active]"
                 points="48.1 68.25 48.1 76.01 37.31 76.01 37.31 68.25 29.55 68.25
                 29.55 57.46 37.31 57.46 37.31 49.7 48.1 49.7 48.1 57.46 55.86 57.46
                 55.86 68.25 48.1 68.25"/>
      </g>
    </g>
  </svg>
</template>

<script>
export default {
  name: 'PrescriptionsIcon',
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
  }
}
</style>
