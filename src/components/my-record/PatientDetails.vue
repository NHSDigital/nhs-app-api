<template>
  <div :class="[$style.recordContent, getCollapseState]">
    <label>{{ $t('myRecord.patientInfo.fieldLabelName') }}</label>
    <p v-if="patientInfo">{{ `${patientInfo.firstName} ${patientInfo.surname}` }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelDOB') }}</label>
    <p v-if="patientInfo">{{ patientInfo.dateOfBirth | longDate }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelSex') }}</label>
    <p v-if="patientInfo">{{ patientInfo.sex }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelAddress') }}</label>
    <p v-if="patientInfo && patientInfo.address">{{ `${patientInfo.address.line1},
    ${patientInfo.address.line2}, ${patientInfo.address.line3}, ${patientInfo.address.town},
    ${patientInfo.address.country}, ${patientInfo.address.postcode}` }}</p>
    <hr>
    <label>{{ $t('myRecord.patientInfo.fieldLabelNHS') }}</label>
    <p v-if="patientInfo">{{ patientInfo.nhsNumber }}</p>
    <hr>
  </div>
</template>

<script>

export default {
  components: {
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      patientInfo: this.$store.state.myRecord.patientDemographics,
      items: [],
    };
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
  },
  methods: {
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
