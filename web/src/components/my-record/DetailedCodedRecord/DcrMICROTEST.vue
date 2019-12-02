<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isImmunisationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="IMMUNISATIONS"
                           :text="$t('my_record.immunisations.sectionHeader')"
                           :aria-expanded="!isImmunisationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.immunisations.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <immunisations :is-collapsed="isImmunisationsCollapsed" :immunisations="record.immunisations" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isProblemsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PROBLEMS"
                           :text="$t('my_record.healthConditions.sectionHeader')"
                           :aria-expanded="!isProblemsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.healthConditions.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <problems :is-collapsed="isProblemsCollapsed" :problems="record.problems" />

    <analytics-tracked-tag id="testResultsHeader"
                           :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isTestResultsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="TESTRESULTS"
                           :text="$t('my_record.testResults.sectionHeader.default')"
                           :aria-expanded="!isTestResultsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.testResults.sectionHeader.default') }}</h2>
    </analytics-tracked-tag>
    <test-results :is-collapsed="isTestResultsCollapsed" :results="record.testResults"
                  :supplier="record.supplier" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isMedicalHistoryCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="MEDICAL_HISTORY"
                           :text="$t('my_record.medicalHistory.sectionHeader')"
                           :aria-expanded="!isMedicalHistoryCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.medicalHistory.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <medicalHistory :is-collapsed="isMedicalHistoryCollapsed"
                    :medical-history="record.medicalHistories" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isRecallsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="RECALLS"
                           :text="$t('my_record.recalls.sectionHeader')"
                           :aria-expanded="!isRecallsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.recalls.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <recalls :is-collapsed="isRecallsCollapsed" :recalls="record.recalls" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isEncountersCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ENCOUNTERS"
                           :text="$t('my_record.encounters.sectionHeader')"
                           :aria-expanded="!isEncountersCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.encounters.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <encounters :is-collapsed="isEncountersCollapsed" :encounters="record.encounters" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isReferralsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="REFERRALS"
                           :text="$t('my_record.referral.sectionHeader')"
                           :aria-expanded="!isReferralsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.referrals.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <referrals :is-collapsed="isReferralsCollapsed" :referrals="record.referrals" />

  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import Problems from '@/components/my-record/SharedComponents/Problems';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import MedicalHistory from '@/components/my-record/SharedComponents/MedicalHistory';
import Recalls from '@/components/my-record/SharedComponents/Recalls';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import Encounters from '@/components/my-record/SharedComponents/Encounters';
import Referrals from '@/components/my-record/SharedComponents/Referrals';

const IMMUNISATIONS = 'immunisations';
const TESTRESULTS = 'testresults';
const PROBLEMS = 'problems';
const MEDICAL_HISTORY = 'medicalHistory';
const RECALLS = 'recalls';
const ENCOUNTERS = 'encounters';
const REFERRALS = 'referrals';

export default {
  name: 'DcrMICROTEST',
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
    TestResults,
    MedicalHistory,
    Recalls,
    Encounters,
    Referrals,
  },
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      IMMUNISATIONS,
      TESTRESULTS,
      PROBLEMS,
      MEDICAL_HISTORY,
      RECALLS,
      ENCOUNTERS,
      REFERRALS,
      isImmunisationsCollapsed: process.client,
      isTestResultsCollapsed: process.client,
      isProblemsCollapsed: process.client,
      isMedicalHistoryCollapsed: process.client,
      isRecallsCollapsed: process.client,
      isEncountersCollapsed: process.client,
      isReferralsCollapsed: process.client,
    };
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
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
        case MEDICAL_HISTORY:
          this.isMedicalHistoryCollapsed =
            !this.isMedicalHistoryCollapsed;
          break;
        case RECALLS:
          this.isRecallsCollapsed =
            !this.isRecallsCollapsed;
          break;
        case ENCOUNTERS:
          this.isEncountersCollapsed =
            !this.isEncountersCollapsed;
          break;
        case REFERRALS:
          this.isReferralsCollapsed =
            !this.isReferralsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
</style>
