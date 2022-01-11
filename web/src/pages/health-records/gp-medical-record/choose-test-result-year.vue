<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <menu-item-list>
          <menu-item v-for="year in historicYears"
                     :id="`view-older-results-${year}`"
                     :key="year"
                     :text="`${year}`"
                     :click-func="getTestResultsForYear"
                     :click-param="`${year}`"
                     header-tag="h2"
                     href="#"
                     data-sid="view-older-results"/>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { GP_MEDICAL_RECORD_PATH, TEST_RESULTS_FOR_YEAR_PATH } from '@/router/paths';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    MenuItem,
    MenuItemList,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      results: null,
      startYear: this.getStartYear(),
      pageYearCount: 5,
    };
  },
  computed: {
    showError() {
      return this.results && (
        this.results.hasErrored ||
        this.results.data.length === 0 ||
        !this.results.hasAccess);
    },
    historicYears() {
      const previousYears = [];
      for (let i = 0; i < this.pageYearCount; i += 1) {
        previousYears[i] = this.startYear - i;
      }
      return previousYears;
    },
  },
  async mounted() {
    const endYear = this.startYear - this.pageYearCount + 1;
    if (!this.$store.state.myRecord.record.testResults) {
      await this.$store.dispatch('myRecord/load');
    }
    this.results = this.$store.state.myRecord.record.testResults;

    const headerText = this.$t('navigation.pages.headers.testResultsChooseYear',
      { startYear: this.startYear, endYear });

    EventBus.$emit(UPDATE_HEADER, headerText, true);
  },
  methods: {
    getStartYear() {
      return new Date().getFullYear() - 1;
    },
    getTestResultsForYear(year) {
      redirectTo(this, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${year}`);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/linked-profiles-index";
</style>
