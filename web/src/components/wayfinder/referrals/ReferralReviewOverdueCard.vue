<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.overdue.title') }}
    </h3>

    <p v-if="hasSpecialty" :id="`requested-specialty-${referralId}`"
       class="nhsuk-u-margin-bottom-3">
      <strong>{{ requestedSpecialty }}</strong>
    </p>

    <p v-if="hasSpecialty" :id="`contact-specialty-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.overdue.youNeedToContactSpecialty',
            null, {specialty: requestedSpecialty}) }}
    </p>

    <p v-else :id="`contact-no-specialty-${referralId}`" class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.overdue.youNeedToContact') }}
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

    <p v-if="hasReviewDate" :id="`review-date-${referralId}`" class="nhsuk-u-margin-bottom-3">
      <strong>
        <span :id="`review-date-header-${referralId}`">
          {{ $t('wayfinder.referrals.reviewDate') }}
        </span>
      </strong>
      <br>
      <span :id="`review-date-text-${referralId}`">{{ getFormattedReviewDate }}</span>
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

    <primary-button :id="`manageInReviewReferral-${referralId}`" @click="onClick">
      {{ $t('wayfinder.referrals.overdue.contactTheClinic') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'ReferralReviewOverdueCard',
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
    reviewDate: {
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
