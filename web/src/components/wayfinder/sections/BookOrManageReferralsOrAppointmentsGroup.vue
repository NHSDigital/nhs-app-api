<template>
  <card-group v-if="hasAny" class="nhsuk-grid-row">
    <card-group-item
      v-for="(summaryItem, index) in actionableReferralsAndAppointments"
      :key="`actionable-item-${index}`"
      class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
      <component
        :is="getSummaryItemComponent(summaryItem)"
        :item="summaryItem" />
    </card-group-item>
  </card-group>

  <p v-else
     id="no-referrals-or-appointments-text">
    {{ $t('wayfinder.noReferralsOrAppointments') }}
  </p>
</template>

<script>
import AppointmentReadyToConfirmCard from '@/components/wayfinder/appointments/AppointmentReadyToConfirmCard';
import ReferralReadyToRebookCard from '@/components/wayfinder/referrals/ReferralReadyToRebookCard';
import ReferralReviewOverdueCard from '@/components/wayfinder/referrals/ReferralReviewOverdueCard';
import ReferralBookableCard from '@/components/wayfinder/referrals/ReferralBookableCard';
import { isNonEmptyArray } from '@/lib/utils';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

export default {
  name: 'BookOrManageReferralsOrAppointmentsGroup',
  components: {
    CardGroup,
    CardGroupItem,
    AppointmentReadyToConfirmCard,
    ReferralReadyToRebookCard,
    ReferralReviewOverdueCard,
    ReferralBookableCard,
  },
  computed: {
    hasAny() {
      return isNonEmptyArray(this.actionableReferralsAndAppointments);
    },
    actionableReferralsAndAppointments() {
      return this.$store.state.wayfinder.summary.actionableReferralsAndAppointments;
    },
  },
  methods: {
    getSummaryItemComponent(summaryItem) {
      if (summaryItem.itemType === 'UpcomingAppointment') {
        return AppointmentReadyToConfirmCard;
      }

      if (summaryItem.status === 'bookableWasCancelled') {
        return ReferralReadyToRebookCard;
      }

      if (summaryItem.status === 'bookable') {
        return ReferralBookableCard;
      }

      return ReferralReviewOverdueCard;
    },
  },
};
</script>
