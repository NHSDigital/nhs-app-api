<template>
  <card-group v-if="hasAny"
              class="nhsuk-grid-row">
    <card-group-item v-for="(referral, index) in referralsInReviewNotOverdue"
                     :key="`referral-in-review-not-overdue-${index}`"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
      <referral-in-review-card :item="referral"/>
    </card-group-item>
  </card-group>
  <p v-else
     id="no-referrals-in-review-text">
    {{ $t('wayfinder.noReferralsInReview') }}
  </p>
</template>

<script>
import ReferralInReviewCard from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralInReviewCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import { isNonEmptyArray } from '@/lib/utils';

export default {
  name: 'ReferralsInReviewGroup',
  components: {
    ReferralInReviewCard,
    CardGroup,
    CardGroupItem,
  },
  computed: {
    hasAny() {
      return isNonEmptyArray(this.referralsInReviewNotOverdue);
    },
    referralsInReviewNotOverdue() {
      return this.$store.state.wayfinder.summary.referralsInReviewNotOverdue;
    },
  },
};
</script>
