<template>
  <card-group v-if="hasReferralsOrAppointments" class="nhsuk-grid-row">
    <card-group-item v-for="(appointment, index) in unconfirmedAppointments"
                     :key="`upcoming-appointment-${index}`"
                     class="nhsuk-grid-column-three-quarters">
      <appointment-ready-to-confirm-card
        :appointment-id="index"
        :location-description="appointment.locationDescription"
        :deep-link-url="appointment.deepLinkUrl"/>
    </card-group-item>
    <card-group-item v-for="referral in referralsNotInReview"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-three-quarters">

      <referral-ready-to-rebook-card
        v-if="isBookableWasCancelled(referral)"
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :referral-id="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>

      <referral-bookable-card
        v-else-if="isBookable(referral)"
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :referral-id="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>
    </card-group-item>

    <card-group-item v-for="referral in referralsInReview"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-three-quarters">

      <referral-review-overdue-card
        v-if="isReviewOverdue(referral)"
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :review-date="referral.reviewDueDate"
        :referral-id="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>

    </card-group-item>
  </card-group>
  <p v-else id="no-referrals-or-appointments-text">
    {{ $t('wayfinder.noReferralsOrAppointments') }}
  </p>
</template>

<script>
import AppointmentReadyToConfirmCard from '@/components/wayfinder/appointments/AppointmentReadyToConfirmCard';
import ReferralReadyToRebookCard from '@/components/wayfinder/referrals/ReferralReadyToRebookCard';
import ReferralReviewOverdueCard from '@/components/wayfinder/referrals/ReferralReviewOverdueCard';
import ReferralBookableCard from '@/components/wayfinder/referrals/ReferralBookableCard';
import { isBefore } from '@/lib/utils';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

export default {
  name: 'BookOrManageReferralsOrAppointmentsCard',
  components: {
    AppointmentReadyToConfirmCard,
    ReferralReadyToRebookCard,
    ReferralReviewOverdueCard,
    ReferralBookableCard,
    CardGroup,
    CardGroupItem,
  },
  props: {
    hasReferralsOrAppointments: {
      type: Boolean,
      default: false,
    },
    referralsInReview: {
      type: Array,
      default: null,
    },
    referralsNotInReview: {
      type: Array,
      default: null,
    },
    unconfirmedAppointments: {
      type: Array,
      default: null,
    },
  },
  methods: {
    isBookable(referral) {
      return referral.status === 'bookable';
    },
    isBookableWasCancelled(referral) {
      return referral.status === 'bookableWasCancelled';
    },
    isReviewOverdue(referral) {
      return referral.status === 'inReview' && isBefore(referral.reviewDueDate);
    },
  },
};
</script>
