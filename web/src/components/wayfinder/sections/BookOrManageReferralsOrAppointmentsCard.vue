<template>
  <card-group v-if="hasAnyReferralsOrAppointments" class="nhsuk-grid-row">
    <card-group-item v-for="(appointment, index) in unconfirmedAppointments"
                     :key="`upcoming-appointment-${index}`"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
      <appointment-ready-to-confirm-card
        :appointment-id="index"
        :location-description="appointment.locationDescription"
        :deep-link-url="appointment.deepLinkUrl"/>
    </card-group-item>
    <card-group-item v-for="referral in bookableWasCancelledReferrals"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">

      <referral-ready-to-rebook-card
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :referral-id="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>
    </card-group-item>

    <card-group-item v-for="referral in bookableReferrals"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">

      <referral-bookable-card
        :requested-specialty="referral.serviceSpecialty"
        :referred-date="referral.referredDateTime"
        :referral-id="referral.referralId"
        :referred-by="referral.referrerOrganisation"
        :deep-link-url="referral.deepLinkUrl"/>
    </card-group-item>

    <card-group-item v-for="referral in overdueReferrals"
                     :key="referral.referralId"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">

      <referral-review-overdue-card
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
import { isBefore, isNonEmptyArray } from '@/lib/utils';
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
  data() {
    const bookableWasCancelledReferrals = this.referralsNotInReview
      .filter(this.isBookableWasCancelledReferral);
    const bookableReferrals = this.referralsNotInReview
      .filter(this.isBookableReferral);
    const overdueReferrals = this.referralsInReview
      .filter(this.isReviewOverdueReferral);

    const hasAnyReferralsOrAppointments =
      isNonEmptyArray(this.referralsNotInReview) ||
      isNonEmptyArray(this.referralsInReview) ||
      isNonEmptyArray(this.unconfirmedAppointments);

    return {
      bookableWasCancelledReferrals,
      bookableReferrals,
      overdueReferrals,
      hasAnyReferralsOrAppointments,
    };
  },
  methods: {
    isBookableReferral(referral) {
      return referral.status === 'bookable';
    },
    isBookableWasCancelledReferral(referral) {
      return referral.status === 'bookableWasCancelled';
    },
    isReviewOverdueReferral(referral) {
      return referral.status === 'inReview' && isBefore(referral.reviewDueDate);
    },
  },
};
</script>
