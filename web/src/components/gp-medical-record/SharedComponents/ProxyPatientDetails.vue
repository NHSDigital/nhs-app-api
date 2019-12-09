<template>
  <div class="nhsuk-u-padding-bottom-3"
       data-sid="proxy-patient-details">
    <p data-purpose="record-item-header"
       class="nhsuk-label nhsuk-u-margin-top-0
                nhsuk-u-padding-bottom-0 nhsuk-u-font-weight-bold">
      {{ $t('my_record.patientInfo.fieldLabelName') }}</p>
    <p v-if="proxyPatientDetails" data-purpose="record-item-detail" data-hj-suppress
       class="nhsuk-u-padding-top-0 nhsuk-u-margin-bottom-0">
      {{ proxyPatientDetails.name }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
    <p data-purpose="record-item-header"
       class="nhsuk-label nhsuk-u-margin-top-0
                nhsuk-u-padding-bottom-0 nhsuk-u-font-weight-bold">
      {{ $t('my_record.patientInfo.fieldLabelAge') }}</p>
    <p v-if="proxyPatientDetails" data-purpose="record-item-detail"
       class="nhsuk-u-padding-top-0 nhsuk-u-margin-bottom-0">
      {{ getDisplayedAgeText(proxyPatientDetails) }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import CalculateAgeInMonthsAndYears from '../../../plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';

export default {
  name: 'ProxyPatientDetails',
  mixins: [CalculateAgeInMonthsAndYears],
  props: {
    proxyPatientDetails: {
      type: Object,
      default: null,
    },
  },
  computed: {
    ...mapGetters({
      patientInfo: 'myRecord/patientDemographics',
    }),
  },
};

</script>

