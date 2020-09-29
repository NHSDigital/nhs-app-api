<template>
  <div :class="[hasBorder && $style.border, bannerStyle, !shouldBeFloating || $style.floating]">
    <div class="nhsuk-width-container">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-two-thirds">
          <slot/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>

export default {
  name: 'WarningBanner',
  props: {
    routes: {
      type: Array,
      default: () => [],
    },
    shouldBeFloating: {
      type: Boolean,
      default: false,
    },
    color: {
      type: String,
      default: 'yellow',
    },
    hasBorder: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    bannerStyle() {
      return this.$style[this.color];
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/colours';

  .yellow {
    background: $warning_title;
  }

  .border {
    border-bottom: 1px $color_nhsuk-grey-4 solid;
  }

  .silver {
    background: $background;
  }

  .floating{
    position: sticky;
    top: 0px;
    z-index: 999;
  }

</style>
