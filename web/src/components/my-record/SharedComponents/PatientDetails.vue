<template>
  <div :class="[$style['record-content'], getCollapseState]" :aria-hidden="isCollapsed">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelName') }}</span>
    <p v-if="patientDetails">{{ patientDetails.patientName }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelDOB') }}</span>
    <p v-if="patientDetails">{{ patientDetails.dateOfBirth | longDate }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelSex') }}</span>
    <p v-if="patientDetails">{{ patientDetails.sex }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelAddress') }}</span>
    <p v-if="patientDetails">
      {{ patientDetails.address }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelNHS') }}</span>
    <p v-if="patientDetails">{{ patientDetails.nhsNumber }}</p>
    <hr aria-hidden="true">
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
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
@import '../../../style/medrecordtitle';

  .fieldName {
    padding-left: 1.3em;
    padding-right: 1.3em;
    padding-bottom: 0.250rem;
    color: #425563;
    font-size: 0.813em;
    font-weight: 700;
  }

</style>
