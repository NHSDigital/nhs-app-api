<template>
  <div>
    <h2 id="book-Or-Manage-Referrals-And-Appointments-Title"
        class="nhsuk-u-padding-bottom-5">
      {{ groupTitleWithCounter(totalActionableReferralsAndAppointments) }}
    </h2>
    <help-link
      id="wayfinder-help-jump-off-link-referrals-or-appointments"
      :href="wayfinderHelpPath"
      :click-func="redirectToWayfinderHelp"
      :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.referralsOrAppointments')"/>
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
    <!--Card group is currently the only way to get this to act as the other groups style wise !-->
    <card-group v-else class="nhsuk-grid-row">
      <card-group-item class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
        <p
          id="no-referrals-or-appointments-text">
          {{ $t('wayfinder.noReferralsOrAppointments') }}
        </p>
      </card-group-item>
    </card-group>
  </div>
</template>

<script>
import AppointmentReadyToConfirmCard from '@/components/appointments/hospital-referrals-appointments/appointments/AppointmentReadyToConfirmCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import ReferralReadyToRebookCard from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralReadyToRebookCard';
import ReferralReviewOverdueCard from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralReviewOverdueCard';
import ReferralBookableCard from '@/components/appointments/hospital-referrals-appointments/referrals/ReferralBookableCard';
import { isNonEmptyArray, redirectTo } from '@/lib/utils';
import { WAYFINDER_HELP_PATH } from '@/router/paths';

export default {
  name: 'BookOrManageReferralsOrAppointmentsGroup',
  components: {
    CardGroup,
    CardGroupItem,
    AppointmentReadyToConfirmCard,
    ReferralReadyToRebookCard,
    ReferralReviewOverdueCard,
    ReferralBookableCard,
    HelpLink,
  },
  data() {
    return {
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
    };
  },
  computed: {
    hasAny() {
      return isNonEmptyArray(this.actionableReferralsAndAppointments);
    },
    actionableReferralsAndAppointments() {
      return this.$store.state.wayfinder.summary.actionableReferralsAndAppointments;
    },
    totalActionableReferralsAndAppointments() {
      return this.$store.state.wayfinder.summary.actionableReferralsAndAppointments.length;
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
    redirectToWayfinderHelp() {
      redirectTo(this, this.wayfinderHelpPath);
    },
    groupTitleWithCounter(count) {
      if (count === 0 || count > 1) {
        return this.$t('wayfinder.bookOrManageReferralsAndAppointmentsTitle')
          .replace('{count}', count)
          .replace('{plural}', 's')
          .replace('{plural}', 's');
      }

      return this.$t('wayfinder.bookOrManageReferralsAndAppointmentsTitle')
        .replace('{count}', count)
        .replace('{plural}', '')
        .replace('{plural}', '');
    },
  },
};
</script>
