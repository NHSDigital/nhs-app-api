<template>
  <div :class="$style['page-divider']">
    <hr class="nhsuk-section-break nhsuk-section-break--m" >
    <div :class="$style['page-divider__content']">
      <span :class="$style['page-divider__inner']">{{ text }}</span>
    </div>
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
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import "~nhsuk-frontend/packages/core/settings/colours";
@import "~nhsuk-frontend/packages/core/settings/spacing";
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import "~nhsuk-frontend/packages/core/tools/spacing";

.page-divider {
  text-align: center;
  .page-divider__content {
    position: relative;
    bottom: nhsuk-spacing(6);
    margin: 0 auto;
    margin-bottom: -(nhsuk-spacing(6));
    max-width: 80%;
    @include govuk-media-query($until: tablet) {
      bottom: nhsuk-spacing(5);
      margin-bottom: -(nhsuk-spacing(5));
    }
    .page-divider__inner {
      background: $color_nhsuk-grey-5;
      display: inline-block;
      padding: nhsuk-spacing(1) nhsuk-spacing(2);
    }
  }
}
</style>
