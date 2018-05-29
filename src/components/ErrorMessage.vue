<template>
  <div>
    <p :id="$style.errorMessage">
      <error-marker-icon/>
      <slot/>
    </p>
  </div>
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
  @import '../style/fonts';
  @import '../style/colours';

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
