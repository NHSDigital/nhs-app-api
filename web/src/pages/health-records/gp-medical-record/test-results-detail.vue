<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate"
       :class="[$style.content,
                'pull-content',
                !$store.state.device.isNativeApp && $style.desktopWeb]">
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="testResults.hasErrored"
      :has-access="testResults.hasAccess"/>
    <Card v-else :class="$style['vision-test-results', 'test-result-content']">
      <span v-html="markup"/>
    </Card>
    <glossary v-if="markup"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      class="nhsuk-u-margin-top-3"
      :path="backPath"
      :button-text="'generic.back'"
      @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    Glossary,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      markup: null,
      testResults: null,
    };
  },
  computed: {
    showError() {
      return (this.testResults && !this.markup);
    },
  },
  async mounted() {
    if (!this.$store.state.myRecord.record.testResults) {
      await this.$store.dispatch('myRecord/loadTestResults');
    }

    this.markup = get('markup', this.$store.state.myRecord.testResults);
    this.testResults = get('testResults', this.$store.state.myRecord.record) || {};
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/test-results-detail";
</style>
