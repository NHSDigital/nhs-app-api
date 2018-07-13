<template>
  <div>
    <p :class="$style.errorMessage">
      <error-marker-icon/>
      <span data-purpose="error">
        <slot/>
      </span>
    </p>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import ErrorMarkerIcon from '@/components/icons/ErrorMarkerIcon';

export default {
  components: {
    ErrorMarkerIcon,
  },
};
</script>

<style module lang="scss">
  @import '../../style/fonts';
  @import '../../style/colours';

  #errorMessage {
    display: block;
    font-family: $default;
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
    color: $error;
    padding-bottom: 10px;
  }
</style>
