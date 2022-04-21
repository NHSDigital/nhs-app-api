<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.overdue.title') }}
    </h3>

    <p v-if="hasSpecialty" class="nhsuk-u-margin-bottom-3">
      {{ requestedSpecialty }}
    </p>

    <p v-if="hasSpecialty" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.overdue.youNeedToContactSpecialty',
             null, {specialty: requestedSpecialty}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredDate',
             null, {referralDate: getFormattedReferredDate}) }}
    </p>

    <p v-if="hasReviewDate" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.reviewDate',
             null, {reviewDate: getFormattedReviewDate}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.bookingReference',
             null, {reference: bookingReference}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredBy',
             null, {referrer: referredBy}) }}
    </p>

    <primary-button :id="`manageInReviewReferral-${bookingReference}`">
      {{ $t('wayfinder.referrals.overdue.contactTheClinic') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';

export default {
  name: 'ReferralReviewOverdueCard',
  components: {
    Card,
    PrimaryButton,
  },
  props: {
    bookingReference: {
      type: String,
      default: '',
    },
    referredBy: {
      type: String,
      default: '',
    },
    referredDate: {
      type: String,
      default: '',
    },
    reviewDate: {
      type: String,
      default: '',
    },
    requestedSpecialty: {
      type: String,
      default: '',
    },
  },

  computed: {
    hasSpecialty() {
      return this.requestedSpecialty;
    },
    hasReviewDate() {
      return this.reviewDate;
    },
    getFormattedReviewDate() {
      return this.$options.filters.longDate(this.reviewDate);
    },
    getFormattedReferredDate() {
      return this.$options.filters.longDate(this.referredDate);
    },
  },
};
</script>
