<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.bookable.title') }}
    </h3>

    <p v-if="hasSpecialty" :id="`requested-specialty-${referralId}`"
       class="nhsuk-u-margin-bottom-3">
      {{ requestedSpecialty }}
    </p>

    <p :id="`referral-date-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredDate',
             null, {referralDate: getFormattedReferredDate}) }}
    </p>

    <p v-if="hasSpecialty" :id="`referral-ready-to-book-${referralId}`"
       class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.bookable.yourReferralAppointmentIsReadyToBook',
             null, {specialty: requestedSpecialty}) }}
    </p>

    <p :id="`booking-reference-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.bookingReference',
             null, {reference: bookingReference}) }}
    </p>

    <p :id="`referred-by-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredBy',
             null, {referrer: referredBy}) }}
    </p>

    <primary-button :id="`bookOrManageReferral-${referralId}`" @click="onClick">
      {{ $t('wayfinder.referrals.bookable.bookOrManageThisReferral') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'ReferralBookableCard',
  components: {
    Card,
    PrimaryButton,
  },
  mixins: [RedirectorMixin],
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
    requestedSpecialty: {
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
    getFormattedReferredDate() {
      return this.$options.filters.longDate(this.referredDate);
    },
  },
};
</script>
