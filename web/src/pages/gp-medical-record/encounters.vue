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
            <span v-if="encounter.recordedOn && encounter.recordedOn.value">
              <strong>
                {{ encounter.recordedOn.value | datePart(encounter.recordedOn.datePart) }}
              </strong>
            </span>
            <span v-else>
              <strong>{{ $t('my_record.noStartDate') }}</strong>
            </span>
            <p> {{ encounter.description }} </p>
            <p> {{ $t('my_record.encounters.value') }}{{ encounter.value }} </p>
            <p> {{ $t('my_record.encounters.unit') }}{{ encounter.unit }} </p>
          </div>
        </Card>
      </MedicalRecordCardGroupItem>
    </div>

    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="backPath"
      :button-text="'rp03.backButton'"
      @clickAndPrevent="backButtonClicked"
    />
    <glossary v-if="!showError"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import MedicalRecordCardGroupItem from '@/components/gp-medical-record/SharedComponents/MedicalRecordCardGroupItem';
import Card from '@/components/widgets/card/Card';
import { MYRECORD } from '@/lib/routes';
import Glossary from '@/components/Glossary';
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
  data() {
    return {
      backPath: MYRECORD.path,
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

<style module scoped lang="scss">
@import "../../style/colours";
@import "../../style/desktopWeb/accessibility";
</style>
