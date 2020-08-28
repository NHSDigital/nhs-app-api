<template>
  <div>
    <h3>{{ $t('organDonation.additionalDetails.additionalInformation') }}</h3>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.additionalDetails.ethnicity') }}
    </h4>
    <p v-if="!ethnicityId">
      {{ $t('organDonation.additionalDetails.youDidNotAnswer') }}
    </p>
    <p v-else>{{ ethnicity }}</p>
    <h4 class="nhsuk-heading-xs nhsuk-u-margin-0">
      {{ $t('organDonation.additionalDetails.religion') }}
    </h4>
    <p v-if="!religionId">
      {{ $t('organDonation.additionalDetails.youDidNotAnswer') }}
    </p>
    <p v-else>{{ religion }}</p>
    <p>
      {{ $t('organDonation.additionalDetails.optionalInformationIsOnlyUsedBy') }}
    </p>
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
