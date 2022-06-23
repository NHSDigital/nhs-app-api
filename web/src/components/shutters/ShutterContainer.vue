<template>
  <div data-purpose="shutter-container"
       :class="[$style.msg, {[$style.plain]: isPlain}, 'nhsuk-width-container--full']"
       :aria-live="ariaLive">
    <div :class="$style['msg-content']" data-purpose="shutter">
      <slot/>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ShutterContainer',
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
@import "@/style/custom/shutter-container";
</style>
