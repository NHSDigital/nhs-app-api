<template>
  <div data-purpose="error-container"
       :class="[this.$style.msg, {[this.$style.plain]: isPlain}, 'nhsuk-width-container--full']"
       :aria-live="ariaLive">
    <h2 v-if="!isPlain" :class="['nhsuk-heading-m', $style.icon]">
      {{ $t('generic.error') }}
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
  @import "@/style/custom/error-container";
</style>
