<template>
  <div :class="$style['nhsuk-card-group']">
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'CardGroup',
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/tools/mixins';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';
  @import '~nhsuk-frontend/packages/core/tools/typography';
  @import '~nhsuk-frontend/packages/core/settings/typography';

  /* card group
   ========================================================================== */

  /**
   * card group allows you to have a row of cards.
   *
   * 1. Flexbox is used to make each card in a row the same height.
   */

  .nhsuk-card-group {
    @include flex(); /* [1] */

    margin-bottom: nhsuk-spacing(5);

    @include mq($until: desktop) {
      margin-bottom: nhsuk-spacing(4);
    }
  }
</style>
