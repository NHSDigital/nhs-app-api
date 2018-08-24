<template>
  <div v-if="showTemplate" id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <glossary-header />
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapseState(isPatientDetailsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PATIENTDETAILS"
                           :text="$t('my_record.patientInfo.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.patientInfo.sectionHeader') }}
    </analytics-tracked-tag>
    <patient-details :is-collapsed="isPatientDetailsCollapsed"
                     :patient-details="patientDetails"/>

    <div v-if="myRecord.hasSummaryRecordAccess">
      <analytics-tracked-tag :class="[$style['record-title'],
                                      getCollapseState(isAllergiesAndAdverseReactionsCollapsed)]"
                             :click-func="myRecordSectionClick"
                             :click-param="ALLERGIESANDADVERSEREACTIONS"
                             :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
                             data-purpose="accordion"
                             tag="h2">
        {{ $t('my_record.allergiesAndAdverseReactions.sectionHeader') }}
      </analytics-tracked-tag>
      <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                       :data="myRecord.allergies" />

      <analytics-tracked-tag :class="[$style['record-title'],
                                      getCollapseState(isAcuteMedicationsCollapsed)]"
                             :click-func="myRecordSectionClick"
                             :click-param="ACUTEMEDICATIONS"
                             :text="$t('my_record.acuteMedications.sectionHeader')"
                             data-purpose="accordion"
                             tag="h2">
        {{ $t('my_record.acuteMedications.sectionHeader') }}
      </analytics-tracked-tag>
      <medications :is-collapsed="isAcuteMedicationsCollapsed"
                   :data="myRecord.medications.data.acuteMedications"
                   :has-error="myRecord.medications.hasErrored"/>

      <analytics-tracked-tag :class="[$style['record-title'],
                                      getCollapseState(isCurrentRepeatMedicationsCollapsed)]"
                             :click-func="myRecordSectionClick"
                             :click-param="CURRENTREPEATMEDICATIONS"
                             :text="$t('my_record.currentRepeatMedications.sectionHeader')"
                             data-purpose="accordion"
                             tag="h2">
        {{ $t('my_record.currentRepeatMedications.sectionHeader') }}
      </analytics-tracked-tag>
      <medications :is-collapsed="isCurrentRepeatMedicationsCollapsed"
                   :data="myRecord.medications.data.currentRepeatMedications"
                   :has-error="myRecord.medications.hasErrored" />

      <analytics-tracked-tag :class="[$style['record-title'],
                                      getCollapseState(isDiscontinuedRepeatMedicationsCollapsed)]"
                             :click-func="myRecordSectionClick"
                             :click-param="DISCONTINUEDREPEATMEDICATIONS"
                             :text="$t('my_record.discontinuedRepeatMedications.sectionHeader')"
                             data-purpose="accordion"
                             tag="h2">
        {{ $t('my_record.discontinuedRepeatMedications.sectionHeader') }}
      </analytics-tracked-tag>
      <medications :is-collapsed="isDiscontinuedRepeatMedicationsCollapsed"
                   :data="myRecord.medications.data.discontinuedRepeatMedications"
                   :has-error="myRecord.medications.hasErrored" />

      <div v-if="myRecord.hasDetailedRecordAccess">
        <div v-if="myRecord.supplier === 'EMIS'">
          <analytics-tracked-tag :class="[$style['record-title'],
                                          getCollapseState(isImmunisationsCollapsed)]"
                                 :click-func="myRecordSectionClick"
                                 :click-param="IMMUNISATIONS"
                                 :text="$t('my_record.immunisations.sectionHeader')"
                                 data-purpose="accordion"
                                 tag="h2">
            {{ $t('my_record.immunisations.sectionHeader') }}
          </analytics-tracked-tag>
          <immunisations :is-collapsed="isImmunisationsCollapsed" :data="myRecord.immunisations" />

          <analytics-tracked-tag :class="[$style['record-title'],
                                          getCollapseState(isProblemsCollapsed)]"
                                 :click-func="myRecordSectionClick"
                                 :click-param="PROBLEMS"
                                 :text="$t('my_record.problems.sectionHeader')"
                                 data-purpose="accordion"
                                 tag="h2">
            {{ $t('my_record.problems.sectionHeader') }}
          </analytics-tracked-tag>
          <problems :is-collapsed="isProblemsCollapsed" :data="myRecord.problems" />

          <analytics-tracked-tag :class="[$style['record-title'],
                                          getCollapseState(isConsultationsCollapsed)]"
                                 :click-func="myRecordSectionClick"
                                 :click-param="CONSULTATIONS"
                                 :text="$t('my_record.consultations.sectionHeader')"
                                 data-purpose="accordion"
                                 tag="h2">
            {{ $t('my_record.consultations.sectionHeader') }}
          </analytics-tracked-tag>
          <consultations :is-collapsed="isConsultationsCollapsed"
                         :data="myRecord.consultations" />
        </div>

        <div v-if="myRecord.supplier === 'TPP'">
          <analytics-tracked-tag :class="[$style['record-title'],
                                          getCollapseState(isEventsCollapsed)]"
                                 :click-func="myRecordSectionClick"
                                 :click-param="EVENTS"
                                 :text="$t('my_record.events.sectionHeader')"
                                 data-purpose="accordion"
                                 tag="h2">
            {{ $t('my_record.events.sectionHeader') }}
          </analytics-tracked-tag>
          <events :is-collapsed="isEventsCollapsed" :data="myRecord.tppDcrEvents" />
        </div>
        <analytics-tracked-tag id="testResultsHeader"
                               :class="[$style['record-title'],
                                        getCollapseState(isTestResultsCollapsed)]"
                               :click-func="myRecordSectionClick"
                               :click-param="TESTRESULTS"
                               :text="getTestResultSectionHeader(myRecord.supplier)"
                               data-purpose="accordion"
                               tag="h2">
          {{ getTestResultSectionHeader(myRecord.supplier) }}
        </analytics-tracked-tag>
        <test-results :is-collapsed="isTestResultsCollapsed" :data="myRecord.testResults"
                      :supplier="myRecord.supplier" />
      </div>
      <div v-else class="pull-content">
        <p>
          {{ $t('my_record.viewRestOfHealthRecordWarning') }}
        </p>
      </div>
    </div>
    <div v-else class="pull-content">
      <div v-if="hasLoaded">
        <message-dialog message-type="error" icon-text="Warning">
          <message-text :is-header="true">
            {{ $t('my_record.noRecordAccess.warningHeader') }}
          </message-text>
          <message-text>
            {{ $t('my_record.noRecordAccess.warningBody') }}
          </message-text>
        </message-dialog>
      </div>
    </div>
  </div>
</template>


<script>
/* eslint-disable import/extensions */
import PatientDetails from '@/components/my-record/PatientDetails';
import AllergiesAndAdverseReactions from '@/components/my-record/AllergiesAndAdverseReactions';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import Medications from '@/components/my-record/Medications';
import Immunisations from '@/components/my-record/Immunisations';
import TestResults from '@/components/my-record/TestResults';
import Problems from '@/components/my-record/Problems';
import Consultations from '@/components/my-record/Consultations';
import Events from '@/components/my-record/Events';
import VueScrollTo from 'vue-scrollto';
import AbbreviationsArrowRightIcon from '@/components/icons/AbbreviationsArrowRightIcon';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GlossaryHeader from '@/components/GlossaryHeader';

const PATIENTDETAILS = 'patientdetails';
const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';
const ACUTEMEDICATIONS = 'acutemedications';
const CURRENTREPEATMEDICATIONS = 'currentrepeatmedications';
const DISCONTINUEDREPEATMEDICATIONS = 'discontinuedrepeatmedications';
const IMMUNISATIONS = 'immunisations';
const TESTRESULTS = 'testresults';
const PROBLEMS = 'problems';
const CONSULTATIONS = 'consultations';
const EVENTS = 'events';

export default {
  components: {
    MessageDialog,
    MessageText,
    PatientDetails,
    AllergiesAndAdverseReactions,
    Medications,
    Immunisations,
    TestResults,
    Problems,
    Consultations,
    Events,
    VueScrollTo,
    AbbreviationsArrowRightIcon,
    AnalyticsTrackedTag,
    GlossaryHeader,
  },
  beforeRouteEnter(to, from, next) {
    if (from.path === '/my-record/myrecordwarning' || from.path === '/my-record/testresultdetail') {
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
      CONSULTATIONS,
      EVENTS,
      isPatientDetailsCollapsed: true,
      hasLoaded: false,
      isAllergiesAndAdverseReactionsCollapsed: true,
      isAcuteMedicationsCollapsed: true,
      isCurrentRepeatMedicationsCollapsed: true,
      isDiscontinuedRepeatMedicationsCollapsed: true,
      isImmunisationsCollapsed: true,
      isTestResultsCollapsed: true,
      isProblemsCollapsed: true,
      isConsultationsCollapsed: true,
      isEventsCollapsed: true,
      myRecord: {},
      patientDetails: {},
      clinicalAbbreviationsUrl: process.env.CLINICAL_ABBREVIATIONS_URL,
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
          }).then(() => {
            if (this.$route.hash) {
              setTimeout((route) => {
                VueScrollTo.scrollTo(route.hash, 500, { easing: VueScrollTo['ease-in'] });
              }, 500, this.$route);
              this.isTestResultsCollapsed = false;
            }
          });
      });
  },
  methods: {
    getTestResultSectionHeader(supplier) {
      return supplier === 'TPP' ?
        this.$t('my_record.testResults.sectionHeader.tpp') :
        this.$t('my_record.testResults.sectionHeader.default');
    },
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
        case CONSULTATIONS:
          this.isConsultationsCollapsed =
            !this.isConsultationsCollapsed;
          break;
        case EVENTS:
          this.isEventsCollapsed =
            !this.isEventsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../style/medrecordtitle';

  .no-padding {
    margin-left: -1em;
    margin-right: -1em;
  }

  .abbreviations {
    margin-bottom: 0.5em;
    p {
      padding-left: 1em;
      padding-top: 0.5em;
      padding-bottom: 0.5em;
      padding-right: 0.5em;
    }
    a {
      line-height: 1em;
      padding-left: 1em;
      padding-top: 0.5em;
      padding-bottom: 0.5em;
    }
  }
</style>
