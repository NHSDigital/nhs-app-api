<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.bookable.title') }}
    </h3>

    <p v-if="hasSpecialty" :id="`requested-specialty-${referralId}`"
       class="nhsuk-u-margin-bottom-3">
      <strong>{{ requestedSpecialty }}</strong>
    </p>

    <p :id="`referral-date-${referralId}`" class="nhsuk-u-margin-bottom-3">
      <strong>
        <span :id="`referral-date-header-${referralId}`">
          {{ $t('wayfinder.referrals.referredDate') }}
        </span>
      </strong>
      <br>
      <span :id="`referral-date-text-${referralId}`">{{ getFormattedReferredDate }}</span>
    </p>

    <p :id="`referred-by-${referralId}`" class="nhsuk-u-margin-bottom-3">
      <strong>
        <span :id="`referred-by-header-${referralId}`">
          {{ $t('wayfinder.referrals.referredBy') }}
        </span>
      </strong>
      <br>
      <span :id="`referred-by-text-${referralId}`">{{ referredBy }}</span>
    </p>

    <primary-button
      :id="`bookOrManageReferral-${referralId}`"
      @click="goToUrlViaRedirector(deepLinkUrl)">
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
    deepLinkUrl: {
      type: String,
      required: true,
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
