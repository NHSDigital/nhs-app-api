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
  @import "@/style/custom/message-list";
</style>
