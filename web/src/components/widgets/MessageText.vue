<template>
  <p :class="[this.$style.msgText,
              indentStyle,
              extendedStyle,
              !$store.state.device.isNativeApp && $style.desktopWeb]"
     :aria-label="ariaLabel">
    <slot/>
  </p>
</template>

<script>

export default {
  name: 'MessageText',
  props: {
    ariaLabel: {
      type: String,
      default: undefined,
    },
    isHeader: {
      type: Boolean,
      default: false,
    },
    overrideStyle: {
      type: String,
      default: 'none',
      validator: value => ['none', 'plain'].indexOf(value) !== -1,
    },
    isBeforeFooter: {
      type: Boolean,
      default: false,
    },
    unindent: {
      type: Boolean,
      default: false,
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
      if (this.isBeforeFooter) {
        style = this.$style.footerMargin;
      }
      return `${style} nhsuk-u-margin-bottom-2`;
    },
    indentStyle() {
      return this.unindent ? this.$style.unindented : 'nhsuk-u-margin-left-2';
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/textstyles";
.msgText {
  padding: 1em 1em 0 1em;
  @include message;
  &.header {
    @include default_label;
    }
  &.plainHeader {
    @include default_label;
    }
  &.footerMargin {
   margin-bottom: 1em;
  }
}
.unindented {
  padding: 1em 1em 0 0;
}
</style>
