<template>
  <p :class="$style['validation-text']">
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
</style>
