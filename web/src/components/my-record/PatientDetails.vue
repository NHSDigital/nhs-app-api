<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <label>{{ $t('myRecord.patientInfo.fieldLabelName') }}</label>
    <p v-if="patientDetails">{{ patientDetails.patientName }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelDOB') }}</label>
    <p v-if="patientDetails">{{ patientDetails.dateOfBirth | longDate }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelSex') }}</label>
    <p v-if="patientDetails">{{ patientDetails.sex }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelAddress') }}</label>
    <p v-if="patientDetails && patientDetails.address">
      {{ patientDetails.address }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelNHS') }}</label>
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

<style lang="scss" module>
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/elements';

  .recordContent { @include record-content };
</style>
