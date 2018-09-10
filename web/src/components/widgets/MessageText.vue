<template>
  <p :class="[$style.msgText, extendedStyle]">
    <slot/>
  </p>
</template>

<script>

export default {
  props: {
    isHeader: {
      type: Boolean,
      default: false,
    },
    overrideStyle: {
      type: String,
      default: 'none',
      validator: value => ['none', 'plain'].indexOf(value) !== -1,
    },
  },
  computed: {
    extendedStyle() {
      let style;
      if (this.isHeader) {
        style = this.$style.header;
      }
      if (this.isHeader && this.overrideStyle === 'plain') {
        style = this.$style.plainHeader;
      }
      return style;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/textstyles";
.msgText {
  padding: 1em 1em 0.150em 1em;
  @include message;
  &.header {
    @include default_label;
    font-size: 1.125em;
    }
  &.plainHeader {
    @include default_label;
    font-size: 1.125em;
    }
}
</style>
