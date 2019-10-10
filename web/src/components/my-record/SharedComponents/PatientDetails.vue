<template>
  <div v-if="!isCollapsed" :class="[$style['record-content'],
                                    'nhsuk-u-padding-bottom-3',
                                    getCollapseState,
                                    !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelName') }}</p>
    <p v-if="patientDetails" data-purpose="record-item-detail" data-hj-suppress
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ patientDetails.patientName }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelDOB') }}</p>
    <p v-if="patientDetails" data-purpose="record-item-detail"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ patientDetails.dateOfBirth | longDate }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelSex') }}</p>
    <p v-if="patientDetails" data-purpose="record-item-detail" data-hj-suppress
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ patientDetails.sex }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelAddress') }}</p>
    <p v-if="patientDetails" data-purpose="record-item-detail" data-hj-suppress
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ patientDetails.address }}</p>
    <hr aria-hidden="true" class="nhsuk-u-margin-top-3">
    <p data-purpose="record-item-header"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3
       nhsuk-body-s">
      {{ $t('my_record.patientInfo.fieldLabelNHS') }}</p>
    <p v-if="patientDetails" data-purpose="record-item-detail"
       class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
      {{ patientDetails.nhsNumber }}</p>
    <hr aria-hidden="true">
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'PatientDetails',
  props: {
    patientDetails: {
      type: Object,
      default: null,
    },
    isCollapsed: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    ...mapGetters({
      patientInfo: 'myRecord/patientDemographics',
    }),
  },
};

</script>

<style module lang="scss" scoped>
@import '../../../style/medrecordcontent';
</style>
