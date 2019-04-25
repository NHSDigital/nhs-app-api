<template>
  <div v-if="!isCollapsed" :class="[$style['record-content'],
                                    getCollapseState,
                                    !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelName') }}</span>
    <p v-if="patientDetails" data-hj-suppress>{{ patientDetails.patientName }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelDOB') }}</span>
    <p v-if="patientDetails">{{ patientDetails.dateOfBirth | longDate }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelSex') }}</span>
    <p v-if="patientDetails" data-hj-suppress>{{ patientDetails.sex }}</p>
    <hr aria-hidden="true">
    <span :class="$style.fieldName">{{ $t('my_record.patientInfo.fieldLabelAddress') }}</span>
    <p v-if="patientDetails" data-hj-suppress>
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
@import '../../../style/medrecordtitle';

  .fieldName {
    padding-left: 1.3em;
    padding-right: 1.3em;
    padding-bottom: 0.250rem;
    color: #425563;
    font-size: 0.813em;
    font-weight: 700;
  }

div {
 &.desktopWeb {
  max-width: 540px;
  cursor: default;

  span {
   font-family: $default_web;
   font-weight: normal;
  }
  p {
   font-family: $default_web;
   font-weight: normal;
  }
 }
}

</style>
