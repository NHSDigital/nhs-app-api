<template>
  <svg :class="[$style.menu, selected?$style.selected:undefined]" xmlns="http://www.w3.org/2000/svg"
       viewBox="0 0 110.39 110.39">
    <defs/>
    <title>Appointments</title>
    <g>
      <rect :class="$style.canvas" width="110.39" height="110.39" />
      <g>
        <g>
          <rect :class="[$style['menu_stroke'], $style.active]" x="24.65" y="47.84"
                width="13.98" height="13.98" />
          <rect :class="[$style['menu_stroke'], $style.active]" x="48.2" y="47.84"
                width="13.98" height="13.98" />
          <rect :class="[$style['menu_stroke'], $style.active]" x="71.76" y="47.84"
                width="13.98" height="13.98" />
          <rect :class="[$style['menu_stroke'], $style.active]" x="24.65" y="70.65"
                width="13.98" height="13.98" />
          <rect :class="[$style['menu_stroke'], $style.active]" x="48.2" y="70.65"
                width="13.98" height="13.98" />
          <rect :class="[$style['menu_stroke'], $style.active]" active="" x="71.76" y="70.65"
                width="13.98" height="13.98" />
        </g>
        <polyline :class="$style['menu_stroke']"
                  points="14.72 37.53 14.72 94.94 95.67 94.94 95.67 37.53" />
        <polyline :class="$style['menu_stroke']"
                  points="24.65 23.86 24.65 15.46 38.64 15.46 38.64 23.96" />
        <polyline :class="$style['menu_stroke']"
                  points="71.75 23.93 71.75 15.46 85.74 15.46 85.74 23.89" />
        <polygon :class="[$style['menu_stroke'], $style.active]"
                 points="85.74 24.29 85.74 29.44 71.75 29.44 71.75 24.29 38.64
                 24.29 38.64 29.44 24.65 29.44 24.65 24.29 14.72 24.29 14.72 37.53
                 95.67 37.53 95.67 24.29 85.74 24.29"
        />
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
    .active {
      fill: $nhs_blue;
    }
  }
}

</style>
