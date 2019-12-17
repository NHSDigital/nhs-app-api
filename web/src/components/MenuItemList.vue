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
  @import '~nhsuk-frontend/packages/core/tools/mixins';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';

.list-menu {
  list-style: none;
  margin: 5px -16px 0.5em;

  @include govuk-media-query('tablet') {
    margin-left: 0;
    margin-right: 0;
  }
}
</style>
