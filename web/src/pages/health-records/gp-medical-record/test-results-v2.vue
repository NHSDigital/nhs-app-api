<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <h2>{{ getYear() }}</h2>
      <test-results
        :results="results"
        :show-glossary="false"
        :from="from"/>
      <menu-item-list class="nhsuk-u-margin-top-5">
        <menu-item id="view-older-results"
                   data-purpose="view-older-results"
                   header-tag="h2"
                   :href="chooseTestResultYearPath"
                   :text="$t('myRecord.detailedCodedRecord.viewAllTestResults')"
                   :aria-label="
                     getAriaLabel($t('myRecord.detailedCodedRecord.viewAllTestResults'))"
                   :click-func="goToUrl"
                   :click-param="chooseTestResultYearPath"/>
      </menu-item-list>
      <glossary v-if="!showError"/>
    </div>
  </div>
</template>

<script>
import Glossary from '@/components/Glossary';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import TestResults from '@/components/gp-medical-record/SharedComponents/TestResults';
import { CHOOSE_TEST_RESULT_YEAR_PATH } from '@/router/paths';

export default {
  components: {
    Glossary,
    MenuItem,
    MenuItemList,
    TestResults,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      results: null,
      chooseTestResultYearPath: CHOOSE_TEST_RESULT_YEAR_PATH,
      from: 'v2',
    };
  },
  computed: {
    showError() {
      return this.results && (
        this.results.hasErrored ||
        this.results.data.length === 0 ||
        !this.results.hasAccess);
    },
  },
  async mounted() {
    this.$store.dispatch('myRecord/clearLastViewedTestResultYear');
    if (!this.$store.state.myRecord.record.testResults) {
      await this.$store.dispatch('myRecord/load');
    }
    this.results = this.$store.state.myRecord.record.testResults;
  },
  methods: {
    getYear() {
      return new Date().getFullYear();
    },
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/inline-block-pointer";
</style>
