<template>
  <ul :class="$style['list-menu']" role="list">
    <slot/>
  </ul>
</template>

<script>
export default {
  name: 'MenuItemList',
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/ifff';

.list-menu {
  @include nhsuk-responsive-padding(0, "left");
  @include nhsuk-responsive-margin(1, "top");
  @include nhsuk-responsive-margin(3, "bottom");
  list-style: none;
  @include govuk-media-query($until: desktop) {
    margin-left: (-$nhsuk-gutter-half) ;
    margin-right: (-$nhsuk-gutter-half);
  }
}
</style>
