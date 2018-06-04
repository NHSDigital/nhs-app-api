<template>
  <div id="mainDiv">
    <h5 :class="[$style.recordTitle, getCollapseStatePatientDetails]"
        @click="patientDetailsHeaderClick">{{ $t('myRecord.patientInfo.sectionHeader') }}</h5>
    <patient-details :is-collapsed="isPatientDetailsCollapsed"/>
    <h5 :class="[$style.recordTitle, getCollapseStateAllergies]"
        @click="allergiesAndAdverseReactionsClick">
      {{ $t('myRecord.allergiesAndAdverseReactions.sectionHeader') }}
    </h5>
    <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"/>
    <main :class="$style.content">
      <error-warning-dialog error-or-warning="warning">
        <p>
          {{ $t('myRecord.viewRestOfHealthRecordWarning') }}
        </p>
      </error-warning-dialog>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import PatientDetails from '@/components/my-record/PatientDetails';
import AllergiesAndAdverseReactions from '@/components/my-record/AllergiesAndAdverseReactions';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  middleware: ['auth', 'meta', 'patientDemographicsHandler'],
  components: {
    ErrorWarningDialog,
    PatientDetails,
    AllergiesAndAdverseReactions,
  },
  data() {
    return {
      isPatientDetailsCollapsed: false,
      isAllergiesAndAdverseReactionsCollapsed: true,
    };
  },
  computed: {
    getCollapseStatePatientDetails() {
      return this.isPatientDetailsCollapsed ? this.$style.closed : this.$style.opened;
    },
    getCollapseStateAllergies() {
      return this.isAllergiesAndAdverseReactionsCollapsed ? this.$style.closed : this.$style.opened;
    },
  },
  methods: {
    patientDetailsHeaderClick() {
      this.isPatientDetailsCollapsed = !this.isPatientDetailsCollapsed;
    },
    allergiesAndAdverseReactionsClick() {
      this.isAllergiesAndAdverseReactionsCollapsed = !this.isAllergiesAndAdverseReactionsCollapsed;
    },
  },
  async fetch({ store }) {
    await store.dispatch('myRecord/loadAllergiesAndAdverseReactions');
  },
};

</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';

  .recordTitle {
    font-family: "FrutigerLTW01-65Bold", Arial, sans-serif;
    font-weight: 700;
    font-size: 18px;
    line-height: 23px;
    color: #005EB8;
    padding: 16px;
    padding-right: 30px;
    box-sizing: border-box;
    background: transparent url('~/assets/icon_arrow_left.svg') no-repeat center right;
    background-position: right 16px center;
    transition: ease 0.5s;
    border-bottom: 2px solid $white;
    margin-bottom: 0px !important;

    &.opened {
      background: transparent url('~/assets/icon_arrow_down.svg') no-repeat center right;
      background-position: right 16px center;
    }
    &.closed {
      background: transparent url('~/assets/icon_arrow_left.svg') no-repeat center right;
      background-position: right 16px center;
    }
  }

</style>
