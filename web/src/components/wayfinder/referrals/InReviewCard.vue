<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('appointments.wayfinder.referrals.inReview.title') }}
    </h3>

    <p v-if="hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ requestedSpeciality }}
    </p>

    <p v-if="hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.inReview.yourHealthcareProviderHasRequested',
             null, {speciality: requestedSpeciality}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.referredDate',
             null, {referralDate: getFormattedReferredDate}) }}
    </p>

    <p v-if="hasReviewDate" class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.reviewDate',
             null, {reviewDate: getFormattedReviewDate}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.bookingReference',
             null, {reference: bookingReference}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.referredBy',
             null, {referrer: referredBy}) }}
    </p>

    <primary-button id="manageInReviewReferral">
      {{ $t('appointments.wayfinder.referrals.manageThisReferral') }}
    </primary-button>
  </Card>
</template>
<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import moment from 'moment-timezone';

export default {
  name: 'InReviewReferralsCard',
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
    requestedSpeciality: {
      type: String,
      default: '',
    },
  },

  computed: {
    hasSpeciality() {
      return this.requestedSpeciality;
    },
    hasReviewDate() {
      return this.reviewDate;
    },
    getFormattedReviewDate() {
      return moment.tz(this.reviewDate, 'Europe/London').format('D MMMM YYYY');
    },
    getFormattedReferredDate() {
      return moment.tz(this.referredDate, 'Europe/London').format('D MMMM YYYY');
    },
  },
};
</script>
