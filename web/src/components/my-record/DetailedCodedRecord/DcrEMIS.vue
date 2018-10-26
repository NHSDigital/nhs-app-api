<template>
  <div>
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isImmunisationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="IMMUNISATIONS"
                           :text="$t('my_record.immunisations.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.immunisations.sectionHeader') }}
    </analytics-tracked-tag>
    <immunisations :is-collapsed="isImmunisationsCollapsed" :data="myRecord.immunisations" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isProblemsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PROBLEMS"
                           :text="$t('my_record.problems.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.problems.sectionHeader') }}
    </analytics-tracked-tag>
    <problems :is-collapsed="isProblemsCollapsed" :data="myRecord.problems" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isConsultationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="CONSULTATIONS"
                           :text="$t('my_record.consultations.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.consultations.sectionHeader') }}
    </analytics-tracked-tag>
    <consultations :is-collapsed="isConsultationsCollapsed"
                   :data="myRecord.consultations" />
    <analytics-tracked-tag id="testResultsHeader"
                           :class="[$style['record-title'],
                                    getCollapsedState(isTestResultsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="TESTRESULTS"
                           :text="$t('my_record.testResults.sectionHeader.default')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.testResults.sectionHeader.default') }}
    </analytics-tracked-tag>
    <test-results :is-collapsed="isTestResultsCollapsed" :data="myRecord.testResults"
                  :supplier="myRecord.supplier" />
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
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
    Consultations,
    TestResults,
  },
  data() {
    return {
      IMMUNISATIONS,
      TESTRESULTS,
      PROBLEMS,
      CONSULTATIONS,
      isImmunisationsCollapsed: true,
      isTestResultsCollapsed: true,
      isProblemsCollapsed: true,
      isConsultationsCollapsed: true,
      myRecord: this.$parent.myRecord,
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

</style>
