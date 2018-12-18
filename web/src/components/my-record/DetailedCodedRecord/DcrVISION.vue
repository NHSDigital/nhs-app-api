<template>
  <div>
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isImmunisationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="IMMUNISATIONS"
                           :text="$t('my_record.immunisations.sectionHeader')"
                           :aria-expanded="!isImmunisationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.immunisations.sectionHeader') }}
    </analytics-tracked-tag>

    <immunisations :is-collapsed="isImmunisationsCollapsed" :immunisations="record.immunisations" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isProblemsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PROBLEMS"
                           :text="$t('my_record.problems.sectionHeader')"
                           :aria-expanded="!isProblemsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.problems.sectionHeader') }}
    </analytics-tracked-tag>
    <problems :is-collapsed="isProblemsCollapsed" :problems="record.problems" />

    <analytics-tracked-tag id="testResultsHeader"
                           :class="[$style['record-title'],
                                    getCollapsedState(isTestResultsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="TESTRESULTS"
                           :text="$t('my_record.testResults.sectionHeader.default')"
                           :aria-expanded="!isTestResultsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.testResults.sectionHeader.default') }}
    </analytics-tracked-tag>
    <test-results :is-collapsed="isTestResultsCollapsed" :results="record.testResults"
                  :supplier="record.supplier" />
  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import Problems from '@/components/my-record/SharedComponents/Problems';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import VueScrollTo from 'vue-scrollto';

const IMMUNISATIONS = 'immunisations';
const PROBLEMS = 'problems';
const TESTRESULTS = 'testresults';

export default {
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
    TestResults,
    VueScrollTo,
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
      isProblemsCollapsed: process.client,
      isImmunisationsCollapsed: process.client,
      isTestResultsCollapsed: process.client,
    };
  },
  mounted() {
    if (this.$route.hash) {
      setTimeout((route) => {
        VueScrollTo.scrollTo(route.hash, 500, { easing: VueScrollTo['ease-in'] });
      }, 500, this.$route);
      this.isTestResultsCollapsed = false;
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
