<template>
  <nuxt-link ref="homeLogoEl"
             :to="indexPath">
    <home-icon/>
  </nuxt-link>
</template>
<script>
import HomeIcon from '@/components/icons/HomeIcon';
import { INDEX } from '@/lib/routes';

export default {
  name: 'HomeLink',
  components: { HomeIcon },
  props: {
    indexPath: {
      default: INDEX.path,
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
