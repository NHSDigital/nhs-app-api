<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.inReview.title') }}
    </h3>

    <p v-if="hasSpecialty" :id="`requested-specialty-${referralId}`"
       class="nhsuk-u-margin-bottom-3">
      {{ requestedSpecialty }}
    </p>

    <p v-if="hasSpecialty" :id="`healthcare-requested-specialty-${referralId}`"
       class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.inReview.yourHealthcareProviderHasRequested',
             null, {specialty: requestedSpecialty}) }}
    </p>

    <p v-else :id="`no-requested-specialty-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.inReview.yourHealthcareProviderHasRequestedNoSpecialty') }}
    </p>

    <p :id="`referral-date-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredDate',
             null, {referralDate: getFormattedReferredDate}) }}
    </p>

    <p v-if="hasReviewDate" :id="`review-date-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.reviewDate',
             null, {reviewDate: getFormattedReviewDate}) }}
    </p>

    <p :id="`booking-reference-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.bookingReference',
             null, {reference: bookingReference}) }}
    </p>

    <p :id="`referred-by-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredBy',
             null, {referrer: referredBy}) }}
    </p>

    <primary-button :id="`manageInReviewReferral-${referralId}`" @click="onClick">
      {{ $t('wayfinder.referrals.manageThisReferral') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'ReferralInReviewCard',
  components: {
    Card,
    PrimaryButton,
  },
  mixins: [RedirectorMixin],
  props: {
    bookingReference: {
      type: String,
      required: true,
    },
    referredBy: {
      type: String,
      required: true,
    },
    referredDate: {
      type: String,
      required: true,
    },
    requestedSpecialty: {
      type: String,
      default: '',
    },
    reviewDate: {
      type: String,
      default: '',
    },
    referralId: {
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
