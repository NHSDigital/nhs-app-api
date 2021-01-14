<template>
  <ul :class="[$style['list-menu'], 'nhsuk-u-padding-left-0']" role="list">
    <slot/>
  </ul>
</template>

<script>
export default {
  name: 'MenuItemList',
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/menu-item-list";
</style>
