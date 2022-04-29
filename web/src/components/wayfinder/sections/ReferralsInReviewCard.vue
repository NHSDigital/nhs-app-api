<template>
  <card-group v-if="hasReferralsInReview" class="nhsuk-grid-row">
    <card-group-item v-for="referral in referralsInReview"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-three-quarters">

      <referral-in-review-card
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :review-date="referral.reviewDueDate"
        :booking-reference="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>
    </card-group-item>
  </card-group>

  <p v-else id="no-referrals-or-appointments-text">
    {{ $t('wayfinder.noReferralsOrAppointments') }}
  </p>
</template>

<script>
import ReferralInReviewCard from '@/components/wayfinder/referrals/ReferralInReviewCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

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
};
</script>
