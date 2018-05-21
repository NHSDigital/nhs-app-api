<template>
  <svg xmlns="http://www.w3.org/2000/svg" width="25" height="30" viewBox="0 0 94.96 112.5">
    <defs/>
    <title>{{ $t('icons.myRecordIcon.title') }}</title>
    <rect class="menu_stroke active" x="2" y="2" width="86.3" height="107.81" />
    <polyline class="menu_stroke" points="88.3 78.34 102 78.34 102 2 88.3 2" />
    <line class="menu_stroke white" x1="20.26" y1="80.45" x2="70.04" y2="80.45" />
    <line class="menu_stroke white" x1="20.26" y1="92.62" x2="70.04" y2="92.62" />
    <polygon class="menu_stroke white" points="52.48 49.25 52.48 59.79 37.82 59.79
          37.82 49.25 27.27 49.25 27.27 34.58 37.82 34.58 37.82 24.04 52.48 24.04 52.48
          34.58 63.02 34.58 63.02 49.25 52.48 49.25" />
    <line class="menu_stroke" x1="89.3" y1="52.89" x2="101.05" y2="52.89" />
    <line class="menu_stroke" x1="89.3" y1="27.89" x2="101.05" y2="27.89" />
  </svg>
</template>

<style lang="scss" scoped>
@import "../../style/icons";
@import "../../style/colours";
.active {
  svg {
    .active {
      fill: $nhs_blue;
    }
    .white {
      stroke: $white;
    }
  }
}
</style>
