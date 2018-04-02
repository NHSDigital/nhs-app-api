<template>
  <svg xmlns="http://www.w3.org/2000/svg" width="25" height="5" viewBox="0 0 78 20">
    <defs>
    </defs>
    <title>More</title>
    <circle class="menu_stroke" cx="10" cy="10" r="8" />
    <circle class="menu_stroke" cx="39" cy="10" r="8" />
    <circle class="menu_stroke" cx="68" cy="10" r="8" />
  </svg>
</template>

<style lang="scss" scoped>
  @import "../../style/icons";
  @import "../../style/colours";
  .active {
    svg {
      .menu_stroke {
        fill: $nhs_blue;
      }
    }
  }

</style>
