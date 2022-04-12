<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.readyToBook.title') }}
    </h3>

    <p v-if="hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ requestedSpeciality }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredDate',
             null, {referralDate: getFormattedReferredDate}) }}
    </p>

    <p v-if="hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.readyToBook.youNeedToRebookYour',
             null, {speciality: requestedSpeciality}) }}
    </p>

    <p v-if="!hasSpeciality" class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.readyToBook.youNeedToRebookYourNoSpeciality') }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.bookingReference',
             null, {reference: bookingReference}) }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      {{ $tc('wayfinder.referrals.referredBy',
             null, {referrer: referredBy}) }}
    </p>

    <primary-button :id="`bookOrManageReferral-${bookingReference}`">
      {{ $t('wayfinder.referrals.readyToBook.bookOrManageThisReferral') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';

export default {
  name: 'ReferralReadyToRebookCard',
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
