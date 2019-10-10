<template>
  <dcr-error-no-access v-if="showError"
                       :has-errored="referrals.hasErrored"
                       :has-access="referrals.hasAccess"
                       :class="[$style['record-content'],
                                getCollapseState]"
                       :aria-hidden="isCollapsed"
                       :has-undetermined-access="referrals.hasUndeterminedAccess"/>
  <div v-else-if="!isCollapsed" :class="[$style['record-content'], getCollapseState,
                                         !$store.state.device.isNativeApp && $style.desktopWeb]"
       :aria-hidden="isCollapsed">
    <div v-for="(referral, referralIndex) in orderedReferrals"
         :key="`referral-${ referralIndex }`" :class="$style['record-item']"
         data-purpose="record-item">
      <p v-if="referral.recordDate && referral.recordDate.value"
         data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">
        {{ referral.recordDate.value | datePart(referral.recordDate.datePart) }}
      </p>
      <p v-else data-purpose="record-item-header"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3 nhsuk-u-padding-top-3
         nhsuk-body-s">{{ $t('my_record.noStartDate') }}</p>

      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.referrals.description') }}{{ referral.description }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.referrals.speciality') }}{{ referral.speciality }} </p>
      <p data-purpose="record-item-detail"
         class="nhsuk-u-padding-0 nhsuk-u-margin-0 nhsuk-u-padding-left-3">
        {{ $t('my_record.referrals.ubrn') }}{{ referral.ubrn }} </p>
      <hr aria-hidden="true">
    </div>
  </div>
</template>

<script>

import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccess from '@/components/my-record/SharedComponents/DCRErrorNoAccess';

export default {
  name: 'Referrals',
  components: {
    DcrErrorNoAccess,
  },
  props: {
    isCollapsed: {
      type: Boolean,
      default: true,
    },
    referrals: {
      type: Object,
      default: () => {},
    },
  },
  computed: {
    getCollapseState() {
      return this.isCollapsed ? this.$style.closed : this.$style.opened;
    },
    orderedReferrals() {
      return orderBy([referral => this.getRecordDate(referral.recordDate, '')],
        ['desc'])(this.referrals.data);
    },
    showError() {
      return this.referrals.hasErrored ||
          this.referrals.data.length === 0 ||
          !this.referrals.hasAccess;
    },
  },
  methods: {
    getRecordDate(recordDate, defaultValue) {
      return recordDate && recordDate.value ? recordDate.value : defaultValue;
    },
  },
};

</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordcontent';
</style>
