<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="immunisations.hasErrored"
      :has-access="immunisations.hasAccess"
      :has-undetermined-access="immunisations.hasUndeterminedAccess"/>
    <div
      v-else
      role="list"
      class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
      <MedicalRecordCardGroupItem
        v-for="(item, index) in orderedImmunisations"
        :key="`immunisation-${index}`"
        data-purpose="record-item"
        class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2">
        <Card data-label="immunisations">
          <div data-purpose="immunisations-card">
            <p v-if="item.effectiveDate && item.effectiveDate.value"
               class="nhsuk-u-margin-bottom-0 nhsuk-u-font-weight-bold">
              {{ item.effectiveDate.value | datePart(item.effectiveDate.datePart) }}
            </p>
            <p v-else class="nhsuk-u-margin-bottom-0 nhsuk-u-font-weight-bold">
              {{ $t('my_record.noStartDate') }}
            </p>
            <p class="nhsuk-body nhsuk-u-margin-bottom-2">
              {{ item.term }}
            </p>
            <p v-if="item.nextDate != null"
               class="nhsuk-body nhsuk-u-margin-bottom-2">
              {{ $t('my_record.immunisations.nextDate') }}{{ getNextDateFormatted(item.nextDate) }}
            </p>
            <p v-if="item.status != null"
               class="nhsuk-body nhsuk-u-margin-bottom-2">
              {{ $t('my_record.immunisations.status') }}{{ item.status }}
            </p>
          </div>
        </Card>
      </MedicalRecordCardGroupItem>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
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
    DcrErrorNoAccessGpRecord,
    DesktopGenericBackLink,
    MedicalRecordCardGroupItem,
    Glossary,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD_PATH,
      immunisations: null,

    };
  },
  computed: {
    orderedImmunisations() {
      return orderBy([item =>
        this.getEffectiveDate(item.effectiveDate, '')], ['desc'])((this.immunisations || {}).data);
    },
    showError() {
      return this.immunisations &&
        (this.immunisations.hasErrored ||
        this.immunisations.data.length === 0 ||
        !this.immunisations.hasAccess);
    },
  },
  async mounted() {
    if (!['EMIS', 'VISION', 'MICROTEST'].includes(this.$store.state.myRecord.record.supplier)) {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (!this.$store.state.myRecord.record.immunisations) {
      await this.$store.dispatch('myRecord/load');
    }

    this.immunisations = this.$store.state.myRecord.record.immunisations;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value
        ? effectiveDate.value
        : defaultValue;
    },
    getNextDateFormatted(nextDate) {
      return nextDate.rawValue != null ?
        nextDate.rawValue : this.$options.filters.datePart(nextDate.value, nextDate.datePart);
    },
  },
};
</script>

<style module scoped lang="scss">
@import "../../../style/colours";
@import "../../../style/desktopWeb/accessibility";
</style>
