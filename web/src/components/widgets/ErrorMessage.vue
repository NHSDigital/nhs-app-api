<template>
  <p :class="[$style['validation-text'], !$store.state.device.isNativeApp && $style.desktopWeb]">
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
};
</script>

<style module lang="scss" scoped>
  @import "../../style/errorvalidation";

  .validation-text {
   &.desktopWeb{
    font-family: $default-web;
    font-weight: normal;
    color: red;
    margin-bottom: 1em;
   }
  }
</style>
