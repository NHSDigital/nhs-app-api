<template>
  <p :class="[$style.msgText,
              extendedStyle,
              !$store.state.device.isNativeApp && $style.desktopWeb]"
     :aria-label="ariaLabel"
     :role="role">
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
    role: {
      type: String,
      default: 'text',
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
  &.footerMargin {
   margin-bottom: 1em;
  }
}
</style>
