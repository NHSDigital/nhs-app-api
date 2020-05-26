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
              {{ $t('my_record.noStartDate') }}
            </p>
            <p class="nhsuk-u-margin-bottom-0"
               data-purpose="record-item-detail"> {{ encounter.description }} </p>
            <p class="nhsuk-u-margin-bottom-0"
               data-purpose="record-item-detail">
              {{ $t('my_record.encounters.value') }}{{ encounter.value }} </p>
            <p class=" nhsuk-u-margin-bottom-0"
               data-purpose="record-item-detail">
              {{ $t('my_record.encounters.unit') }}{{ encounter.unit }} </p>
          </div>
        </Card>
      </MedicalRecordCardGroupItem>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="backPath"
      :button-text="'rp03.backButton'"
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
import { GP_MEDICAL_RECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
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
      backPath: GP_MEDICAL_RECORD.path,
    };
  },
  computed: {
    orderedEncounters() {
      return orderBy([encounter => this.getRecordedOnDate(encounter.recordedOn, '')],
        ['desc'])(this.encounters.data);
    },
    showError() {
      return (
        (this.encounters || {}).hasErrored ||
        (this.encounters || {}).data.length === 0 ||
        !this.encounters.hasAccess
      );
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.encounters) {
      await store.dispatch('myRecord/load');
    }
    return {
      encounters: store.state.myRecord.record.encounters,
    };
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
