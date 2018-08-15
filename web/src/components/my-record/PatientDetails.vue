<template>
  <div :class="[$style['record-content'], getCollapseState]">
    <label>{{ $t('my_record.patientInfo.fieldLabelName') }}</label>
    <p v-if="patientDetails">{{ patientDetails.patientName }}</p>
    <hr>
    <label>{{ $t('my_record.patientInfo.fieldLabelDOB') }}</label>
    <p v-if="patientDetails">{{ patientDetails.dateOfBirth | longDate }}</p>
    <hr>
    <label>{{ $t('my_record.patientInfo.fieldLabelSex') }}</label>
    <p v-if="patientDetails">{{ patientDetails.sex }}</p>
    <hr>
    <label>{{ $t('my_record.patientInfo.fieldLabelAddress') }}</label>
    <p v-if="patientDetails">
      {{ patientDetails.address }}</p>
    <hr>
    <label>{{ $t('my_record.patientInfo.fieldLabelNHS') }}</label>
    <p v-if="patientDetails">{{ patientDetails.nhsNumber }}</p>
    <hr>
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
@import '../../style/medrecordcontent';

</style>
