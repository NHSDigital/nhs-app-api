<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="showTemplate">
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="procedures.hasErrored"
      :has-access="procedures.hasAccess"/>
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
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="backPath"
      :button-text="'my_record.diagnosisDetails.backButton'"
      @clickAndPrevent="onBackButtonClicked"/>
    <glossary v-if="!showError"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import { MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import Card from '@/components/widgets/card/Card';
import Glossary from '@/components/Glossary';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    DcrErrorNoAccessGpRecord,
    Card,
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
    await store.dispatch('myRecord/loadProcedures');
    return {
      markup: get('markup', store.state.myRecord.procedures),
      procedures: get('procedures', store.state.myRecord.record) || {},
    };
  },
  methods: {
    onBackButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
  },
};
</script>
