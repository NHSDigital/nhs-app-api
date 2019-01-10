<template>
  <div :class="$style.info">
    <h3>{{ $t('organDonation.reviewYourDecision.additionalInformation.subheader') }}</h3>
    <h4>{{ $t('organDonation.reviewYourDecision.additionalInformation.ethnicityheader') }}</h4>
    <p>{{ ethnicity }}</p>
    <h4>{{ $t('organDonation.reviewYourDecision.additionalInformation.religionheader') }}</h4>
    <p>{{ religion }}</p>
  </div>
</template>

<script>
import find from 'lodash/fp/find';

// eslint-disable-next-line eqeqeq
const findById = id => find(x => x.id == id);

export default {
  name: 'AdditionalInformation',
  props: {
    ethnicityId: {
      type: String,
      default: '',
    },
    religionId: {
      type: String,
      default: '',
    },
    referenceData: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    ethnicity() {
      return this.display(this.ethnicityId, this.referenceData.ethnicities);
    },
    religion() {
      return this.display(this.religionId, this.referenceData.religions);
    },
  },
  methods: {
    display(value, data) {
      const { displayName } = findById(value)(data) || {};
      return displayName;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/info";
  @import '../../style/fonts';
  @import "../../style/spacings";

  h3 {
    margin: 0;
    padding: 0;
  }

  h4 {
    margin: 0;
    padding: $one 0 0 0;
  }
</style>
