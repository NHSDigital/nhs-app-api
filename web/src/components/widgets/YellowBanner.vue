<template>
  <div :class="[$style.bannerContainer, !shouldBeFloating || $style.floating]">
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
  name: 'YellowBanner',
  props: {
    routes: {
      type: Array,
      default: () => [],
    },
    shouldBeFloating: {
      type: Boolean,
      default: false,
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/colours';

  .bannerContainer{
    background: $warning_title;
  }

  .floating{
    position: sticky;
    top: 0px;
    z-index: 999;
  }

</style>
