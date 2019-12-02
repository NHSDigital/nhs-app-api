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
      :button-text="'rp03.backButton'"
      @clickAndPrevent="backButtonClicked"/>
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
    orderedImmunisations() {
      return orderBy([item => item.effectiveDate.value], ['desc'])(this.immunisations.data);
    },
    showError() {
      return (
        (this.immunisations || {}).hasErrored ||
        (this.immunisations || {}).data.length === 0 ||
        !this.immunisations.hasAccess
      );
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.immunisations) {
      await store.dispatch('myRecord/load');
    }
    return {
      immunisations: store.state.myRecord.record.immunisations,
    };
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
@import "../../style/colours";
@import "../../style/desktopWeb/accessibility";
</style>
