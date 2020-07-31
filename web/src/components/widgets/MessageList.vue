<template>
  <ul :class="[$style.msgList, 'nhsuk-u-margin-left-2', isError ? $style.error : undefined]">
    <slot/>
  </ul>
</template>

<script>
export default {
  name: 'MessageList',
  props: {
    isError: {
      type: Boolean,
      default: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/textstyles";
.msgList {
  list-style: none;
  padding: 0em 1em 0em 1em;
  li {
    padding-top: 0.25em;
    @include message;
  }
  &.error {
    li {
      color: $error;
      p {
        color: $error;
      }
    }
  }
}

</style>
