<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate">
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="diagnosis.hasErrored"
      :has-access="diagnosis.hasAccess"/>
    <div v-if="showTemplate"
         :class="['pull-content']"
         class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
      <div class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
        <div v-if="markup">
          <card>
            <span v-html="markup"/>
          </card>
        </div>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="backPath"
      @clickAndPrevent="onBackButtonClicked"/>
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
    DesktopGenericBackLink,
    DcrErrorNoAccessGpRecord,
    Card,
    Glossary,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      markup: null,
      diagnosis: null,
    };
  },
  computed: {
    showError() {
      return (this.diagnosis && !this.markup);
    },
  },
  async mounted() {
    if (this.$store.state.myRecord.record.supplier !== 'VISION') {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    await this.$store.dispatch('myRecord/loadDiagnosis');

    this.markup = get('markup', this.$store.state.myRecord.diagnosis);
    this.diagnosis = get('diagnosis', this.$store.state.myRecord.record) || {};
  },
  methods: {
    onBackButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
