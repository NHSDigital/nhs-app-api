<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="showTemplate" :class="[$style.content,
                                      'pull-content',
                                      !$store.state.device.isNativeApp && $style.desktopWeb]">
      <dcr-error-no-access-gp-record
        v-if="!myRecord.testResults.markup"
        :has-errored="myRecord.record.testResults.hasErrored"
        :has-access="myRecord.record.testResults.hasAccess"
        :has-undetermined-access="myRecord.record.testResults.hasUndeterminedAccess"/>
      <Card v-else :class="$style['vision-test-results', 'test-result-content']">
        <span v-html="myRecord.testResults.markup"/>
      </Card>
      <desktopGenericBackLink
        v-if="!$store.state.device.isNativeApp"
        class="nhsuk-u-margin-top-3"
        :path="backPath"
        :button-text="'rp03.backButton'"
        @clickAndPrevent="backButtonClicked"/>
      <glossary v-if="myRecord.testResults.markup"/>
    </div>
  </div>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    Glossary,
  },
  data() {
    return {
      backPath: MYRECORD.path,
    };
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.testResults) {
      await store.dispatch('myRecord/loadTestResults');
    }
    return {
      myRecord: store.state.myRecord,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/spacings';
  @import '../../style/_textstyles';
  .vision-test-results {
    min-width: 50em;
  }

  .test-result-content {
    box-sizing: border-box;
    padding: 1em;
    padding-top: 0.5em;
    padding-bottom: 0.5em;
    margin-top: 0.5em;
    background-color: #ffffff;
    @include space(margin, bottom, $three);
    overflow-x: scroll;
    display: inline-block;
    margin-right: 2em;
    min-width: 100%;
  }
</style>
