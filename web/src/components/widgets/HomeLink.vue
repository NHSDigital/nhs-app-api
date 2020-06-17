
<template>
  <router-link ref="homeLogoEl"
               :to="indexPath">
    <home-icon/>
  </router-link>
</template>
<script>
import HomeIcon from '@/components/icons/HomeIcon';
import { INDEX_PATH } from '@/router/paths';

export default {
  name: 'HomeLink',
  components: { HomeIcon },
  props: {
    indexPath: {
      default: INDEX_PATH,
      type: String,
    },
  },
  methods: {
    resetFocusToNhsLogo() {
      // No javascript handles the removing of focus fine
      // this line is for javascript side to handle it
      document.activeElement.blur();
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import '../../style/screensizes';
</style>
