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

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isConsultationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="CONSULTATIONS"
                           :text="$t('my_record.consultationsAndEvents.sectionHeader')"
                           :aria-expanded="!isConsultationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.consultationsAndEvents.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <consultations :is-collapsed="isConsultationsCollapsed"
                   :consultations="record.consultations" />
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
  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import Problems from '@/components/my-record/SharedComponents/Problems';
import Consultations from '@/components/my-record/SharedComponents/Consultations';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const IMMUNISATIONS = 'immunisations';
const TESTRESULTS = 'testresults';
const PROBLEMS = 'problems';
const CONSULTATIONS = 'consultations';

export default {
  name: 'DcrEMIS',
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
    Consultations,
    TestResults,
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
      CONSULTATIONS,
      isImmunisationsCollapsed: process.client,
      isTestResultsCollapsed: process.client,
      isProblemsCollapsed: process.client,
      isConsultationsCollapsed: process.client,
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
        case CONSULTATIONS:
          this.isConsultationsCollapsed =
            !this.isConsultationsCollapsed;
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
