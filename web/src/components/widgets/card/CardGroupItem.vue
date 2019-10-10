<template>
  <div :class="$style['nhsuk-card-group__item']">
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'CardGroupItem',
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/tools/mixins';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';

  /* card group
   ========================================================================== */

  /**
   * card group allows you to have a row of cards.
   *
   * 1. Flexbox is used to make each card in a row the same height.
   */

  .nhsuk-card-group__item {
    @include flex-item(); /* [1] */

    @include mq($until: desktop) {
      margin-bottom: nhsuk-spacing(4);

      &:last-child {
        margin-bottom: 0;
      }
    }
  }
</style>
