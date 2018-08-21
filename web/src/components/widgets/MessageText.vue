<template>
  <p :class="[$style.msgText, isHeader ? $style.header : undefined]"
     :data-purpose="isHeader ? 'msg-header' : undefined">
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
}
</style>
