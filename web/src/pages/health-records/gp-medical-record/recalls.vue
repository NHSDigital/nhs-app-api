<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="recalls.hasErrored"
      :has-access="recalls.hasAccess"
      :has-undetermined-access="recalls.hasUndeterminedAccess"/>
    <div v-else data-purpose="health-conditions">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(recall, index) in orderedRecalls"
          :key="`recall-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2"
        >
          <Card data-label="recalls">
            <div data-purpose="recalls-card">
              <p
                v-if="recall.recordDate && recall.recordDate.value"
                class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
                data-purpose="record-item-header">
                {{ recall.recordDate.value | datePart(recall.recordDate.datePart) }}</p>
              <p v-else class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">{{ $t('my_record.noStartDate') }}</p>

              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail"> {{ recall.name }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail"> {{ recall.description }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('my_record.recalls.result') }}{{ recall.result }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('my_record.recalls.nextDate') }}{{ recall.nextDate }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('my_record.recalls.status') }}{{ recall.status }} </p>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'generic.backButton.text'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import Card from '@/components/widgets/card/Card';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Glossary from '@/components/Glossary';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    Card,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
    DcrErrorNoAccessGpRecord,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      recalls: null,
    };
  },
  computed: {
    orderedRecalls() {
      return orderBy([recall => this.getRecordDate(recall.recordDate, '')],
        ['desc'])((this.recalls || {}).data);
    },
    showError() {
      return this.recalls &&
             (this.recalls.hasErrored
             || this.recalls.data.length === 0
             || !this.recalls.hasAccess);
    },
  },
  async mounted() {
    if (this.$store.state.myRecord.record.supplier !== 'MICROTEST') {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (!this.$store.state.myRecord.record.recalls) {
      await this.$store.dispatch('myRecord/load');
    }

    this.recalls = this.$store.state.myRecord.record.recalls;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getRecordDate(recordDate, defaultValue) {
      return recordDate && recordDate.value ? recordDate.value : defaultValue;
    },
  },
};
</script>

<style module scoped lang="scss">
a {
  display: inline-block;
  cursor: pointer;
}
</style>
