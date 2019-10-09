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
      <span v-if="referral.recordDate && referral.recordDate.value"
            :class="$style.fieldName">
        {{ referral.recordDate.value | datePart(referral.recordDate.datePart) }}
      </span>
      <span v-else :class="$style.fieldName">{{ $t('my_record.noStartDate') }}</span>

      <p> {{ $t('my_record.referrals.description') }}{{ referral.description }} </p>
      <p> {{ $t('my_record.referrals.speciality') }}{{ referral.speciality }} </p>
      <p> {{ $t('my_record.referrals.ubrn') }}{{ referral.ubrn }} </p>
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
  @import '../../../style/medrecordtitle';
  @import "../../../style/colours";

  .fieldName {
    padding-left: 1.3em;
    padding-right: 1.3em;
    padding-bottom: 0.250rem;
    color: $dark_grey;
    font-size: 0.813em;
    font-weight: 700;
  }

  div {
    &.desktopWeb {
      max-width: 540px;
      cursor: default;

      span {
        font-family: $default_web;
        font-weight: normal;
      }
      p {
        font-family: $default_web;
        font-weight: normal;
      }
    }
  }

</style>
