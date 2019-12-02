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

    <analytics-tracked-tag :id="`${TESTRESULTS}Header`"
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

    <analytics-tracked-tag :id="`${DIAGNOSIS}Header`"
                           :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isDiagnosisCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="DIAGNOSIS"
                           :text="$t('my_record.diagnosis.sectionHeader.default')"
                           :aria-expanded="!isDiagnosisCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.diagnosis.sectionHeader.default') }}</h2>
    </analytics-tracked-tag>
    <diagnosis :is-collapsed="isDiagnosisCollapsed"
               :results="record.diagnosis"
               :supplier="record.supplier" />

    <analytics-tracked-tag :id="`${EXAMINATIONS}Header`"
                           :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isExaminationCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="EXAMINATIONS"
                           :text="$t('my_record.examinations.sectionHeader.default')"
                           :aria-expanded="!isExaminationCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.examinations.sectionHeader.default') }}</h2>
    </analytics-tracked-tag>

    <examinations :is-collapsed="isExaminationCollapsed"
                  :results="record.examinations"
                  :supplier="record.supplier" />

    <analytics-tracked-tag :id="`${PROCEDURES}Header`"
                           :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isProceduresCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PROCEDURES"
                           :text="$t('my_record.procedures.sectionHeader.default')"
                           :aria-expanded="!isProceduresCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.procedures.sectionHeader.default') }}</h2>
    </analytics-tracked-tag>
    <procedures :is-collapsed="isProceduresCollapsed"
                :results="record.procedures"
                :supplier="record.supplier" />
  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import Problems from '@/components/my-record/SharedComponents/Problems';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import Diagnosis from '@/components/my-record/SharedComponents/Diagnosis';
import Examinations from '@/components/my-record/SharedComponents/Examinations';
import Procedures from '@/components/my-record/SharedComponents/Procedures';
import VueScrollTo from 'vue-scrollto';

const IMMUNISATIONS = 'immunisations';
const PROBLEMS = 'problems';
const TESTRESULTS = 'testResults';
const DIAGNOSIS = 'diagnosis';
const EXAMINATIONS = 'examination';
const PROCEDURES = 'procedures';

export default {
  name: 'DcrVISION',
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
    TestResults,
    Diagnosis,
    Examinations,
    Procedures,
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
      PROBLEMS,
      TESTRESULTS,
      DIAGNOSIS,
      EXAMINATIONS,
      PROCEDURES,
      isProblemsCollapsed: process.client,
      isImmunisationsCollapsed: process.client,
      isTestResultsCollapsed: process.client,
      isDiagnosisCollapsed: process.client,
      isExaminationCollapsed: process.client,
      isProceduresCollapsed: process.client,
    };
  },
  mounted() {
    if (this.$route.hash) {
      setTimeout((route) => {
        const { hash } = route;
        if (hash) {
          const sectionName = hash.replace('Header', '').replace('#', '');
          VueScrollTo.scrollTo(hash, 500, { easing: VueScrollTo['ease-in'] });
          this.myRecordSectionClick(sectionName);
        }
      }, 500, this.$route);
    }
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
        case PROBLEMS:
          this.isProblemsCollapsed =
            !this.isProblemsCollapsed;
          break;
        case TESTRESULTS:
          this.isTestResultsCollapsed =
            !this.isTestResultsCollapsed;
          break;
        case DIAGNOSIS:
          this.isDiagnosisCollapsed =
            !this.isDiagnosisCollapsed;
          break;
        case EXAMINATIONS:
          this.isExaminationCollapsed =
            !this.isExaminationCollapsed;
          break;
        case PROCEDURES:
          this.isProceduresCollapsed =
            !this.isProceduresCollapsed;
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
  @import '../../../style/desktopWeb/accessibility';

  div {
   &.desktopWeb {
    .record-title {
    cursor: pointer;
    &:focus {
      @include outlineStyle
     }
    }
   }
  }
</style>
