<template>
  <p :class="[
    'nhsuk-error-message',
    !$store.state.device.isNativeApp && $style.desktopWeb
  ]">
    <span data-purpose="error">
      <slot/>
    </span>
  </p>
</template>

<script>
export default {
  name: 'ErrorMessage',
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/error-message";
</style>
