<template>
  <div :class="{
    [$style['validation-inline']]: showError,
    [$style['validation-border-left']]: showError
  }">
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'ErrorGroup',
  props: {
    showError: {
      type: Boolean,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/errorvalidation";
</style>
