<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <h2>Confirmation</h2>
    <h3>Ethnicity: {{ ethnicity }}</h3>
    <h3>Religion: {{ religion }}</h3>
  </div>
</template>

<script>
import get from 'lodash/fp/get';

export default {
  computed: {
    ethnicity() {
      return get('$store.state.organDonation.additionalDetails.ethnicityId')(this);
    },
    religion() {
      return get('$store.state.organDonation.additionalDetails.religionId')(this);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/info";
  @import "../../style/accessibility";
</style>
