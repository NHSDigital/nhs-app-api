<template>
  <div v-if="showTemplate">
    <test-results
      v-if="results"
      :results="results"
      :year="year" />
    <pagination v-if="!showPastYearNavigation"
                :previous-link="futureYearPath"
                :previous-title="futureYear"/>
    <pagination v-else-if="!showFutureYearNavigation"
                :next-link="pastYearPath"
                :next-title="pastYear" />
    <pagination v-else
                :previous-link="futureYearPath"
                :previous-title="futureYear"
                :next-link="pastYearPath"
                :next-title="pastYear" />
  </div>
</template>

<script>

import TestResults from '@/components/gp-medical-record/SharedComponents/TestResults';
import Pagination from '@/components/Pagination';
import { redirectTo, getYearOfBirth } from '@/lib/utils';
import { GP_MEDICAL_RECORD_PATH, TEST_RESULTS_FOR_YEAR_PATH } from '@/router/paths';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';

export default {
  components: {
    TestResults,
    Pagination,
  },
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      results: null,
      yearOfBirth: getYearOfBirth(this.$store),
    };
  },
  computed: {
    year() {
      if (this.$route.query.year) {
        return this.$route.query.year;
      }

      if (this.$store.state.myRecord.lastViewedTestResultYear) {
        return this.$store.state.myRecord.lastViewedTestResultYear;
      }

      return this.getCurrentYear();
    },
    pastYear() {
      return `${Number(this.year) - 1}`;
    },
    futureYear() {
      return `${Number(this.year) + 1}`;
    },
    pastYearPath() {
      return `${TEST_RESULTS_FOR_YEAR_PATH}?year=${this.pastYear}`;
    },
    futureYearPath() {
      return `${TEST_RESULTS_FOR_YEAR_PATH}?year=${this.futureYear}`;
    },
    showFutureYearNavigation() {
      return this.year !== this.getCurrentYear().toString();
    },
    showPastYearNavigation() {
      return this.year !== this.yearOfBirth.toString();
    },
  },
  watch: {
    async year() {
      await this.loadTestResults();
    },
  },
  async mounted() {
    if (this.isInvalidYear()) {
      redirectTo(this, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${this.getCurrentYear()}`);
    }
    await this.loadTestResults();
  },
  methods: {
    async loadTestResults() {
      this.results = null;

      this.$store.dispatch('myRecord/setLastViewedTestResultYear', this.year);

      if (!this.$store.state.myRecord.hasLoaded) {
        await this.$store.dispatch('myRecord/load');
      }

      if (!this.$store.state.myRecord.record.historicTestResults || this.$store.state.myRecord.record.historicTestResults[`_${this.year}`] === undefined) {
        await this.$store.dispatch('myRecord/loadHistoricTestResult', this.year);
      }

      this.results = this.$store.state.myRecord.record.historicTestResults[`_${this.year}`];

      const headerText = this.$t('navigation.pages.headers.testResultsForYear', { year: this.year });
      EventBus.$emit(UPDATE_HEADER, headerText, true);
    },
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getCurrentYear() {
      const currentYear = new Date().getFullYear();
      return currentYear;
    },
    isInvalidYear() {
      return ((this.year > this.getCurrentYear()) || this.year < this.yearOfBirth);
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/inline-block-pointer";
</style>
