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
        <pagination v-if="!showPastYearRange"
                    :previous-link="futureYearRangePath"
                    :previous-title="futureYearRange"/>
        <pagination v-else-if="!showFutureYearRange"
                    :next-link="pastYearRangePath"
                    :next-title="pastYearRange" />
        <pagination v-else
                    :previous-link="futureYearRangePath"
                    :previous-title="futureYearRange"
                    :next-link="pastYearRangePath"
                    :next-title="pastYearRange" />
      </div>
    </div>
  </div>
</template>

<script>
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import Pagination from '@/components/Pagination';
import { GP_MEDICAL_RECORD_PATH, TEST_RESULTS_FOR_YEAR_PATH, CHOOSE_TEST_RESULT_YEAR_PATH } from '@/router/paths';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import { redirectTo, getYearOfBirth } from '@/lib/utils';

export default {
  components: {
    MenuItem,
    MenuItemList,
    Pagination,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      pageYearCount: 5,
      yearOfBirth: getYearOfBirth(this.$store),
      previousYear: new Date().getFullYear() - 1,
    };
  },
  computed: {
    page() {
      if (this.$route.query.page) {
        return Number(this.$route.query.page);
      }
      if (this.$store.state.myRecord.lastViewedTestResultYear) {
        const year = this.$store.state.myRecord.lastViewedTestResultYear;
        const yearDiff = this.previousYear - year;
        const page = Math.trunc(yearDiff / this.pageYearCount) + 1;
        return Number(page);
      }
      return 1;
    },
    pastYearRangePath() {
      return `${CHOOSE_TEST_RESULT_YEAR_PATH}?page=${this.pastPageNumber}`;
    },
    futureYearRangePath() {
      return `${CHOOSE_TEST_RESULT_YEAR_PATH}?page=${this.futurePageNumber}`;
    },
    pastPageNumber() {
      const pageNumber = this.page;
      return `${pageNumber + 1}`;
    },
    futurePageNumber() {
      const pageNumber = this.page;
      return `${pageNumber - 1}`;
    },
    startYear() {
      return this.previousYear - ((this.page - 1) * this.pageYearCount);
    },
    endYear() {
      let endYear = this.startYear - (this.pageYearCount - 1);
      if (endYear <= this.yearOfBirth) {
        endYear = this.yearOfBirth;
      }
      return endYear;
    },
    historicYears() {
      const years = [];
      const yearsToShow = this.startYear - this.endYear;
      for (let i = 0; i <= yearsToShow; i += 1) {
        const year = this.startYear - i;
        years.push(year);
      }

      return years;
    },
    pastYearRange() {
      const yearArray = this.historicYears;
      const nextStart = yearArray[yearArray.length - 1] - 1;
      let nextEnd = nextStart - this.pageYearCount + 1;
      if (nextEnd <= this.yearOfBirth) {
        nextEnd = this.yearOfBirth;
      }
      return `${nextStart} to ${nextEnd}`;
    },
    futureYearRange() {
      const yearArray = this.historicYears;
      const nextEnd = yearArray[0] + 1;
      const nextStart = nextEnd + this.pageYearCount - 1;
      return `${nextStart} to ${nextEnd}`;
    },
    showPastYearRange() {
      return !this.historicYears.includes(Number(this.yearOfBirth));
    },
    showFutureYearRange() {
      return !this.historicYears.includes(this.previousYear);
    },
  },
  watch: {
    page() {
      this.updateHeader();
    },
  },
  async mounted() {
    if (this.isInvalidPage()) {
      redirectTo(this, `${CHOOSE_TEST_RESULT_YEAR_PATH}?page=1`);
    }

    this.updateHeader();
  },
  methods: {
    updateHeader() {
      const headerText = this.$t('navigation.pages.headers.testResultsChooseYear',
        { startYear: this.startYear, endYear: this.endYear });

      EventBus.$emit(UPDATE_HEADER, headerText, true);
    },
    getTestResultsForYear(year) {
      redirectTo(this, `${TEST_RESULTS_FOR_YEAR_PATH}?year=${year}`);
    },
    isInvalidPage() {
      const pageNumber = this.page;
      const age = new Date().getFullYear() - this.yearOfBirth;
      const maxPage = Math.trunc(age / this.pageYearCount) + 1;
      return ((pageNumber < 1) || pageNumber > maxPage);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/linked-profiles-index";
</style>
