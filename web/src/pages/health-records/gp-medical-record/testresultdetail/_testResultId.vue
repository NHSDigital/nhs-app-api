<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="showTemplate"
         :class="[$style.content,'pull-content',
                  !$store.state.device.isNativeApp &&
                    $style.desktopWeb]">
      <dcr-error-no-access-gp-record v-if="!result" :no-test-data="true" :has-access="true" />
      <card v-else id="resultDetails">
        <span v-html="result.testResult" />
      </card>
      <div class="nhsuk-u-margin-top-3">
        <glossary v-if="result" />
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          class="nhsuk-u-margin-top-3"
          :path="getBackPath"
          :button-text="'rp03.backButton'"
          @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import { TESTRESULTS_PATH } from '@/router/paths';

import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    Glossary,
  },
  data() {
    return {
      result: null,
    };
  },
  computed: {
    getBackPath() {
      return TESTRESULTS_PATH;
    },
  },
  async mounted() {
    await this.$store.dispatch(
      'myRecord/loadDetailedTestResult',
      this.$route.params.testResultId,
    );
    this.result = this.$store.state.myRecord.detailedTestResult.data;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../../style/_textstyles';
</style>
