<template>
  <div :class="[$style['record-content'], getCollapseState]">
    <b>{{ $t('my_record.patientInfo.fieldLabelName') }}</b>
    <p v-if="patientDetails">{{ patientDetails.patientName }}</p>
    <hr>
    <b>{{ $t('my_record.patientInfo.fieldLabelDOB') }}</b>
    <p v-if="patientDetails">{{ patientDetails.dateOfBirth | longDate }}</p>
    <hr>
    <b>{{ $t('my_record.patientInfo.fieldLabelSex') }}</b>
    <p v-if="patientDetails">{{ patientDetails.sex }}</p>
    <hr>
    <b>{{ $t('my_record.patientInfo.fieldLabelAddress') }}</b>
    <p v-if="patientDetails">
      {{ patientDetails.address }}</p>
    <hr>
    <b>{{ $t('my_record.patientInfo.fieldLabelNHS') }}</b>
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
