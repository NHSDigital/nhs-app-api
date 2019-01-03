<template>
  <p :class="[$style['validation-text'], isDesktopWeb ? $style.desktopWeb : $style.web]">
    <error-marker-icon/>
    <span data-purpose="error">
      <slot/>
    </span>
  </p>
</template>


<script>
import ErrorMarkerIcon from '@/components/icons/ErrorMarkerIcon';

export default {
  components: {
    ErrorMarkerIcon,
  },
  data() {
    return {
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/errorvalidation";
</style>
