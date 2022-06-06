<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate">
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="testResults.hasErrored"
      :has-access="testResults.hasAccess"/>
    <div v-else-if="markup"
         class="nhsuk-u-margin-bottom-4">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
          <card>
            <span v-html="markup"/>
          </card>
        </div>
      </div>
      <no-further-information-available />
    </div>
    <glossary v-if="markup"/>
    <desktop-generic-back-link
      v-if="!$store.state.device.isNativeApp"
      class="nhsuk-u-margin-top-3"
      :path="backPath"
      :button-text="'generic.back'"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import NoFurtherInformationAvailable from '@/components/gp-medical-record/SharedComponents/NoFurtherInformationAvailable';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';

export default {
  components: {
    Card,
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    Glossary,
    NoFurtherInformationAvailable,
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
    await this.$store.dispatch('myRecord/loadTestResults');

    this.markup = get('markup', this.$store.state.myRecord.testResults);
    this.testResults = get('testResults', this.$store.state.myRecord.record) || {};
  },
};
</script>
