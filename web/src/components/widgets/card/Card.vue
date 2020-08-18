<template>
  <component :is="component"
             :class="$style['nhsuk-card']"
             @click="$event => $emit('click', $event)">
    <slot/>
  </component>
</template>

<script>
export default {
  name: 'Card',
  props: {
    component: {
      type: String,
      default: 'div',
      validator: value => ['div', 'a'].includes(value),
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/desktopWeb/accessibility';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/tools/mixins';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  /* ==========================================================================
   COMPONENTS / #card
   ========================================================================== */

  @mixin card($card-background-color, $card-text-color) {

    @include top-and-bottom();
    @include nhsuk-responsive-margin(7, 'bottom');
    @include nhsuk-responsive-margin(7, 'top');
    @include nhsuk-responsive-padding(5);

    background-color: $card-background-color;
    color: $card-text-color;

    @include mq($media-type: print) {
      border: 1px solid $nhsuk-print-text-color;
      page-break-inside: avoid;
    }
  }

  .nhsuk-card {
    @include card($color_nhsuk-white, $nhsuk-text-color);
    width: 100%;
    margin-bottom: 0;
    margin-top: 0;
    padding: 20px;
    border: 1px solid $color_nhsuk-grey-3;
    box-sizing: border-box;
  }

  /**
  The below styling is used for ie11 specifically. Can be removed once we don't support ie11.
   */
  @media all and (-ms-high-contrast: none), (-ms-high-contrast: active) {
    .nhsuk-card {
      flex-basis: 85%;
      @include govuk-media-query($until: desktop) {
        flex: 0 0 80%;
        padding-right: 0;
      }
    }
  }
</style>
