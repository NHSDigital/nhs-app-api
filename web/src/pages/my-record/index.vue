<template>
  <div v-if="showTemplate" id="mainDiv">
    <h5 :class="[$style.recordTitle, getCollapseState(isPatientDetailsCollapsed)]"
        @click="myRecordSectionClick(PATIENTDETAILS)">
      {{ $t('myRecord.patientInfo.sectionHeader') }}</h5>
    <patient-details :is-collapsed="isPatientDetailsCollapsed"
                     :patient-details="patientDetails"/>

    <div v-if="myRecord.hasSummaryRecordAccess">
      <h5 :class="[$style.recordTitle, getCollapseState(isAllergiesAndAdverseReactionsCollapsed)]"
          @click="myRecordSectionClick(ALLERGIESANDADVERSEREACTIONS)">
        {{ $t('myRecord.allergiesAndAdverseReactions.sectionHeader') }}
      </h5>
      <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                       :data="myRecord.allergies"/>

      <h5 :class="[$style.recordTitle, getCollapseState(isAcuteMedicationsCollapsed)]"
          @click="myRecordSectionClick(ACUTEMEDICATIONS)">
        {{ $t('myRecord.acuteMedications.sectionHeader') }}
      </h5>
      <medications :is-collapsed="isAcuteMedicationsCollapsed"
                   :data="myRecord.medications.data.acuteMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <h5 :class="[$style.recordTitle, getCollapseState(isCurrentRepeatMedicationsCollapsed)]"
          @click="myRecordSectionClick(CURRENTREPEATMEDICATIONS)">
        {{ $t('myRecord.currentRepeatMedications.sectionHeader') }}
      </h5>
      <medications :is-collapsed="isCurrentRepeatMedicationsCollapsed"
                   :data="myRecord.medications.data.currentRepeatMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <h5 :class="[$style.recordTitle, getCollapseState(isDiscontinuedRepeatMedicationsCollapsed)]"
          @click="myRecordSectionClick(DISCONTINUEDREPEATMEDICATIONS)">
        {{ $t('myRecord.discontinuedRepeatMedications.sectionHeader') }}
      </h5>
      <medications :is-collapsed="isDiscontinuedRepeatMedicationsCollapsed"
                   :data="myRecord.medications.data.discontinuedRepeatMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <div v-if="myRecord.hasDetailedRecordAccess">
        <h5 :class="[$style.recordTitle, getCollapseState(isImmunisationsCollapsed)]"
            @click="myRecordSectionClick(IMMUNISATIONS)">
          {{ $t('myRecord.immunisations.sectionHeader') }}
        </h5>
        <immunisations :is-collapsed="isImmunisationsCollapsed"
                       :data="myRecord.immunisations"/>

        <h5 :class="[$style.recordTitle, getCollapseState(isTestResultsCollapsed)]"
            @click="myRecordSectionClick(TESTRESULTS)">
          {{ $t('myRecord.testResults.sectionHeader') }}
        </h5>
        <test-results :is-collapsed="isTestResultsCollapsed"
                      :data="myRecord.testResults"/>

        <h5 :class="[$style.recordTitle, getCollapseState(isProblemsCollapsed)]"
            @click="myRecordSectionClick(PROBLEMS)">
          {{ $t('myRecord.problems.sectionHeader') }}
        </h5>
        <problems :is-collapsed="isProblemsCollapsed"
                  :data="myRecord.problems"/>
      </div>
      <div v-else>
        <main :class="$style.content">
          <p>
            {{ $t('myRecord.viewRestOfHealthRecordWarning') }}
          </p>
        </main>
      </div>
    </div>
    <div v-else >
      <div v-if="hasLoaded">
        <main :class="$style.content">
          <error-warning-dialog error-or-warning="error">
            <p>
              <b>{{ $t('myRecord.noRecordAccess.warningHeader') }}</b>
              <br>
              {{ $t('myRecord.noRecordAccess.warningBody') }}
            </p>
          </error-warning-dialog>
        </main>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import PatientDetails from '@/components/my-record/PatientDetails';
import AllergiesAndAdverseReactions from '@/components/my-record/AllergiesAndAdverseReactions';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';
import Medications from '@/components/my-record/Medications';
import Immunisations from '@/components/my-record/Immunisations';
import TestResults from '@/components/my-record/TestResults';
import Problems from '@/components/my-record/Problems';

const PATIENTDETAILS = 'patientdetails';
const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';
const ACUTEMEDICATIONS = 'acutemedications';
const CURRENTREPEATMEDICATIONS = 'currentrepeatmedications';
const DISCONTINUEDREPEATMEDICATIONS = 'discontinuedrepeatmedications';
const IMMUNISATIONS = 'immunisations';
const TESTRESULTS = 'testresults';
const PROBLEMS = 'problems';

export default {
  components: {
    ErrorWarningDialog,
    PatientDetails,
    AllergiesAndAdverseReactions,
    Medications,
    Immunisations,
    TestResults,
    Problems,
  },
  beforeRouteEnter(to, from, next) {
    if (from.path === '/my-record/myrecordwarning') {
      next();
    } else {
      next('/my-record/myrecordwarning');
    }
  },
  data() {
    return {
      PATIENTDETAILS,
      ALLERGIESANDADVERSEREACTIONS,
      ACUTEMEDICATIONS,
      CURRENTREPEATMEDICATIONS,
      DISCONTINUEDREPEATMEDICATIONS,
      IMMUNISATIONS,
      TESTRESULTS,
      PROBLEMS,
      isPatientDetailsCollapsed: true,
      hasLoaded: false,
      isAllergiesAndAdverseReactionsCollapsed: true,
      isAcuteMedicationsCollapsed: true,
      isCurrentRepeatMedicationsCollapsed: true,
      isDiscontinuedRepeatMedicationsCollapsed: true,
      isImmunisationsCollapsed: true,
      isTestResultsCollapsed: true,
      isProblemsCollapsed: true,
      myRecord: {},
      patientDetails: {},
    };
  },
  mounted() {
    this.$store.app.$http
      .getV1PatientDemographics({})
      .then((data) => {
        this.patientDetails = data.response;
        this.isPatientDetailsCollapsed = false;
      }).then(() => {
        this.$store.app.$http
          .getV1PatientMyRecord({})
          .then((data) => {
            this.myRecord = data.response;
            this.hasLoaded = true;
          });
      });
  },
  methods: {
    getCollapseState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case PATIENTDETAILS:
          this.isPatientDetailsCollapsed = !this.isPatientDetailsCollapsed;
          break;
        case ALLERGIESANDADVERSEREACTIONS:
          this.isAllergiesAndAdverseReactionsCollapsed =
          !this.isAllergiesAndAdverseReactionsCollapsed;
          break;
        case ACUTEMEDICATIONS:
          this.isAcuteMedicationsCollapsed =
          !this.isAcuteMedicationsCollapsed;
          break;
        case CURRENTREPEATMEDICATIONS:
          this.isCurrentRepeatMedicationsCollapsed =
          !this.isCurrentRepeatMedicationsCollapsed;
          break;
        case DISCONTINUEDREPEATMEDICATIONS:
          this.isDiscontinuedRepeatMedicationsCollapsed =
          !this.isDiscontinuedRepeatMedicationsCollapsed;
          break;
        case IMMUNISATIONS:
          this.isImmunisationsCollapsed =
            !this.isImmunisationsCollapsed;
          break;
        case TESTRESULTS:
          this.isTestResultsCollapsed =
            !this.isTestResultsCollapsed;
          break;
        case PROBLEMS:
          this.isProblemsCollapsed =
            !this.isProblemsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};

</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';

  .content {
    @include space(padding, all, $three);
  }
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
