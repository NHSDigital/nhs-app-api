<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="referrals.hasErrored"
      :has-access="referrals.hasAccess"
      :has-undetermined-access="referrals.hasUndeterminedAccess"
    />
    <div v-else data-purpose="health-conditions">
      <div role="list" class="nhsuk-grid-row nhsuk-u-margin-bottom-4">
        <MedicalRecordCardGroupItem
          v-for="(referral, index) in orderedReferrals"
          :key="`referral-${index}`"
          class="nhsuk-grid-column-full nhsuk-u-padding-bottom-2"
        >
          <Card data-label="referrals">
            <div data-purpose="referrals-card">
              <p
                v-if="referral.recordDate && referral.recordDate.value"
                class="nhsuk-u-font-weight-bold nhsuk-u-margin-bottom-0"
                data-purpose="record-item-header">
                {{ referral.recordDate.value | datePart(referral.recordDate.datePart) }}</p>
              <p v-else class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-header">{{ $t('myRecord.unknownDate') }}</p>

              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('myRecord.gpMedicalRecord.reason') }}{{ referral.description }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('myRecord.gpMedicalRecord.speciality') }}{{ referral.speciality }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('myRecord.gpMedicalRecord.ubrn') }}{{ referral.ubrn }} </p>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'generic.back'"
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
      referrals: null,
    };
  },
  computed: {
    orderedReferrals() {
      return orderBy([referral => this.getRecordDate(referral.recordDate, '')],
        ['desc'])((this.referrals || {}).data);
    },
    showError() {
      return this.referrals &&
             (this.referrals.hasErrored
             || this.referrals.data.length === 0
             || !this.referrals.hasAccess);
    },
  },
  async mounted() {
    if (this.$store.state.myRecord.record.supplier !== 'MICROTEST') {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (!this.$store.state.myRecord.record.referrals) {
      await this.$store.dispatch('myRecord/load');
    }

    this.referrals = this.$store.state.myRecord.record.referrals;
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
  @import "@/style/custom/referrals";
</style>
