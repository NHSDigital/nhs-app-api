<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('appointments.wayfinder.referrals.bookable.title') }}
    </h3>

    <p v-if="hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ requestedSpeciality }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.referredDate',
             null, {referralDate: getFormattedReferredDate}) }}
    </p>

    <p v-if="hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.bookable.yourReferralAppointmentIsReadyToBook',
             null, {speciality: requestedSpeciality}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.bookingReference',
             null, {reference: bookingReference}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('appointments.wayfinder.referrals.referredBy',
             null, {referrer: referredBy}) }}
    </p>

    <primary-button id="bookOrManageReferral">
      {{ $t('appointments.wayfinder.referrals.bookable.bookOrManageThisReferral') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';

export default {
  name: 'BookableReferralCard',
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
    requestedSpeciality: {
      type: String,
      default: '',
    },
  },
  computed: {
    hasSpeciality() {
      return this.requestedSpeciality;
    },
    getFormattedReferredDate() {
      return this.$options.filters.longDate(this.referredDate);
    },
  },
};
</script>
