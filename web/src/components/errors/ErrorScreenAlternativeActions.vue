<template>
  <div>
    <h2 id="listHeader"
        class="nhsuk-u-margin-top-0 nhsuk-u-margin-bottom-3 nhsuk-u-margin-bottom-0">
      {{ $t(`${alternativeActionsHeader}`) }}
    </h2>
    <menu-item-list>
      <slot name="items"/>
    </menu-item-list>
  </div>
</template>
<script>
import MenuItemList from '@/components/MenuItemList';

export default {
  name: 'ErrorScreenAlternativeActions',
  components: {
    MenuItemList,
  },
  props: {
    alternativeActionsHeader: {
      type: String,
      default: undefined,
    },
  },
};
</script>
<style module lang="scss" scoped>
 hr {
  margin: 0.5em auto 0.5em;
 }
</style>
