<template>
  <svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" viewBox="0 0 114 112">
    <defs>
    </defs>
    <title>{{ $t('icons.appointmentsIcon.title') }}</title>
    <rect class="menu_stroke active" x="15.5" y="46" width="19" height="19" />
    <rect class="menu_stroke active" x="47.5" y="46" width="19" height="19" />
    <rect class="menu_stroke active" x="79.5" y="46" width="19" height="19" />
    <rect class="menu_stroke active" x="15.5" y="77" width="19" height="19" />
    <rect class="menu_stroke active" x="47.5" y="77" width="19" height="19" />
    <rect class="menu_stroke active" x="79.5" y="77" width="19" height="19" />
    <polyline class="menu_stroke" points="2 32 2 110 112 110 112 32" />
    <polyline class="menu_stroke" points="15.5 13.43 15.5 2 34.5 2 34.5 13.55" />
    <polyline class="menu_stroke" points="79.5 13.51 79.5 2 98.5 2 98.5 13.47" />
    <polygon class="menu_stroke active" points="98.5 14 98.5 21 79.5 21 79.5 14 34.5
          14 34.5 21 15.5 21 15.5 14 2 14 2 32 112 32 112 14 98.5 14" />
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
    }
  }

</style>
