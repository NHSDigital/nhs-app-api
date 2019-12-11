<template>
  <div class="nhsuk-u-padding-bottom-3"
       data-sid="proxy-patient-details"
       :aria-hidden="isCollapsed">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelName') }}</p>
    <p v-if="proxyPatientDetails" data-purpose="record-item-detail" data-hj-suppress
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ proxyPatientDetails.name }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelAge') }}</p>
    <p v-if="proxyPatientDetails" data-purpose="record-item-detail"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ getDisplayedAgeText(proxyPatientDetails) }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3">
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
