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
      :button-text="'rp03.backButton'"
      @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
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
  computed: {
    showError() {
      return !this.markup;
    },
  },
  async asyncData({ store }) {
    await store.dispatch('myRecord/loadTestResults');
    return {
      markup: get('markup', store.state.myRecord.testResults),
      testResults: get('testResults', store.state.myRecord.record) || {},
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
    overflow-x: scroll;
    display: inline-block;
    margin-right: 2em;
    min-width: 100%;
  }
</style>
