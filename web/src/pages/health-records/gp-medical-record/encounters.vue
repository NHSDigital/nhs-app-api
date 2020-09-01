<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="encounters.hasErrored"
      :has-access="encounters.hasAccess"
      :has-undetermined-access="encounters.hasUndeterminedAccess"/>
    <div
      v-else
      role="list"
      class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
      <MedicalRecordCardGroupItem
        v-for="(encounter, index) in orderedEncounters"
        :key="`encounter-${index}`"
        data-purpose="record-item"
        class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
        <Card data-label="encounters">
          <div data-purpose="encounters-card">
            <p v-if="encounter.recordedOn && encounter.recordedOn.value"
               data-purpose="record-item-header"
               class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0">
              {{ encounter.recordedOn.value | datePart(encounter.recordedOn.datePart) }}
            </p>
            <p v-else class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
               data-purpose="record-item-header">
              {{ $t('myRecord.unknownDate') }}
            </p>
            <p class="nhsuk-u-margin-bottom-0"
               data-purpose="record-item-detail"> {{ encounter.description }} </p>
            <p class="nhsuk-u-margin-bottom-0"
               data-purpose="record-item-detail">
              {{ $t('myRecord.gpMedicalRecord.value') }}{{ encounter.value }} </p>
            <p class=" nhsuk-u-margin-bottom-0"
               data-purpose="record-item-detail">
              {{ $t('myRecord.gpMedicalRecord.units') }}{{ encounter.unit }} </p>
          </div>
        </Card>
      </MedicalRecordCardGroupItem>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="backPath"
      :button-text="'generic.backButton.text'"
      @clickAndPrevent="backButtonClicked"
    />
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
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      encounters: null,
    };
  },
  computed: {
    orderedEncounters() {
      return orderBy([encounter => this.getRecordedOnDate(encounter.recordedOn, '')],
        ['desc'])((this.encounters || {}).data);
    },
    showError() {
      return this.encounters &&
        (this.encounters.hasErrored ||
        this.encounters.data.length === 0 ||
        !this.encounters.hasAccess);
    },
  },
  async mounted() {
    if (this.$store.state.myRecord.record.supplier !== 'MICROTEST') {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (!this.$store.state.myRecord.record.encounters) {
      await this.$store.dispatch('myRecord/load');
    }

    this.encounters = this.$store.state.myRecord.record.encounters;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getRecordedOnDate(recordedOn, defaultValue) {
      return recordedOn && recordedOn.value ? recordedOn.value : defaultValue;
    },
  },
};
</script>
