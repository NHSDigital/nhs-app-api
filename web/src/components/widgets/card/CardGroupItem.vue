<template>
  <div :class="$style['nhsuk-card-group__item']">
    <slot/>
  </div>
</template>

<script>
export default {
  name: 'CardGroupItem',
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/widgets-card-group-item";
</style>
