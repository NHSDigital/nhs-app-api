<template>
  <div :class="$style['nhsuk-card-group']">
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'CardGroup',
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/widgets-card-group";
</style>
