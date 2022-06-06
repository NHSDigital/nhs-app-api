<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <test-results :results="results" />

      <desktop-generic-back-link
        v-if="!$store.state.device.isNativeApp"
        id="desktopBackLink"
        :path="backPath"
        :button-text="'generic.back'"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import TestResults from '@/components/gp-medical-record/SharedComponents/TestResults';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';

export default {
  components: {
    DesktopGenericBackLink,
    TestResults,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      results: null,
    };
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.testResults) {
      await this.$store.dispatch('myRecord/load');
    }
    this.results = this.$store.state.myRecord.record.testResults;
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/inline-block-pointer";
</style>
