<template>
  <div data-purpose="error-container"
       :class="[this.$style.msg, {[this.$style.plain]: isPlain}, 'nhsuk-width-container--full']"
       :aria-live="ariaLive">
    <h2 v-if="!isPlain" :class="['nhsuk-heading-m', $style.icon]">
      {{ $t('messageIconText.error') }}
    </h2>
    <div :class="$style['msg-content']" data-purpose="error">
      <slot/>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ErrorContainer',
  props: {
    ariaLive: {
      type: String,
      default: 'polite',
    },
    overrideStyle: {
      type: String,
      default: 'none',
      validator: value => ['none', 'plain'].indexOf(value) !== -1,
    },
  },
  computed: {
    mType() {
      return this.showIcon ? [this.$style.msg] : [];
    },
    isPlain() {
      return this.overrideStyle === 'plain';
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '~nhsuk-frontend/packages/core/tools/mixins';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/ifff';

.msg{
  &:not(.plain) {
    @include nhsuk-responsive-margin(3, "bottom");
    @include nhsuk-responsive-margin(5, "top");
    @include nhsuk-responsive-padding(3, "bottom");
    width: 100%;
    height: auto;
    background-color: $color_nhsuk-white;
    border-top: 0.250em $nhsuk-error-color solid;

    .msg-content {
      @include nhsuk-responsive-padding(6, "top");
      @include nhsuk-responsive-padding(4, "left");
      @include nhsuk-responsive-padding(4, "right");
    }
    .icon {
      @include nhsuk-responsive-padding(4, "left");
      @include nhsuk-responsive-padding(4, "right");
      position: absolute;
      height: 2.125em;
      padding-top: 0.5em;
      padding-bottom: 0.5em;
      box-sizing: border-box;
      background-color: $nhsuk-error-color;
      color: $color_nhsuk-white;
      text-align: center;
      margin-top: -1.125em;
      display: inline-block;
    }
  }
  &.plain {
    .msg-content {
      @include nhsuk-responsive-padding(3, "top");
    }
  }
}
</style>
