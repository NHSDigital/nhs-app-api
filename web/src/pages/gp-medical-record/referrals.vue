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
                 data-purpose="record-item-header">{{ $t('my_record.noStartDate') }}</p>

              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('my_record.referrals.description') }}{{ referral.description }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('my_record.referrals.speciality') }}{{ referral.speciality }} </p>
              <p class="nhsuk-u-margin-bottom-0"
                 data-purpose="record-item-detail">
                {{ $t('my_record.referrals.ubrn') }}{{ referral.ubrn }} </p>
            </div>
          </Card>
        </MedicalRecordCardGroupItem>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'rp03.backButton'"
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
import { MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
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
      backPath: MYRECORD.path,
    };
  },
  computed: {
    orderedReferrals() {
      return orderBy([referral => this.getRecordDate(referral.recordDate, '')],
        ['desc'])(this.referrals.data);
    },
    showError() {
      return this.referrals.hasErrored
             || this.referrals.data.length === 0
             || !this.referrals.hasAccess;
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.referrals) {
      await store.dispatch('myRecord/load');
    }
    return {
      referrals: store.state.myRecord.record.referrals,
    };
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
@import "../../style/colours";
@import "../../style/desktopWeb/accessibility";
a {
  display: inline-block;
  &:focus {
    @include outlineStyle;
    background-color: $focus_highlight;
  }
  &:hover {
    @include linkHoverStyle;
    cursor: pointer;
  }
}
</style>
