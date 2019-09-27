<template>
  <div :class="$style['page-divider']">
    <hr >
    <span>{{ text }}</span>
  </div>
</template>

<script>
export default {
  name: 'PageDivider',
  props: {
    text: {
      type: String,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/colours';

  .page-divider {
    text-align: center;
    margin-top: 0.75em;

    span {
      position: relative;
      display: inline-block;
      background: $color_nhsuk-grey-5;
      padding: 0 0.75em;
      top: -0.75em;
      max-width: 80%;
    }
  }
</style>
