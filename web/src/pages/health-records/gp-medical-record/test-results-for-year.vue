<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <test-results :results="results" />
    </div>
  </div>
</template>

<script>
import TestResults from '@/components/gp-medical-record/SharedComponents/TestResults';
import { redirectTo } from '@/lib/utils';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';

export default {
  components: {
    TestResults,
  },
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      results: null,
      year: this.$router.currentRoute.query.year,
    };
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.historicTestResults || this.$store.state.myRecord.record.historicTestResults[`_${this.year}`] === undefined) {
      await this.$store.dispatch('myRecord/loadHistoricTestResult', this.year);
    }
    this.results = this.$store.state.myRecord.record.historicTestResults[`_${this.year}`];

    const headerText = this.$t('navigation.pages.headers.testResultsForYear', { year: this.year });

    EventBus.$emit(UPDATE_HEADER, headerText, true);
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/inline-block-pointer";
</style>
