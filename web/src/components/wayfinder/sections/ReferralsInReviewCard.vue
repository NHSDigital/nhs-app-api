<template>
  <card-group v-if="hasReferralsInReview" class="nhsuk-grid-row">
    <card-group-item v-for="referral in referralsInReviewIsNotReviewOverdue"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">

      <referral-in-review-card
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :review-date="referral.reviewDueDate"
        :referral-id="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>

    </card-group-item>
  </card-group>

  <p v-else id="no-referrals-in-review-text">
    {{ $t('wayfinder.noReferralsInReview') }}
  </p>
</template>

<script>
import ReferralInReviewCard from '@/components/wayfinder/referrals/ReferralInReviewCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import { isBefore } from '@/lib/utils';

export default {
  name: 'ReferralsInReviewCard',
  components: {
    ReferralInReviewCard,
    CardGroup,
    CardGroupItem,
  },
  props: {
    hasReferralsInReview: {
      type: Boolean,
      default: false,
    },
    referralsInReview: {
      type: Array,
      default: null,
    },
  },
  data() {
    return {
      referralsInReviewIsNotReviewOverdue: this.referralsInReview.filter(this.isReviewNotOverdue),
    };
  },
  methods: {
    isReviewNotOverdue(referral) {
      return referral.status === 'inReview' && !isBefore(referral.reviewDueDate);
    },
  },
};
</script>
