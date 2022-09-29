<template>
  <div>
    <h2 id="confirmed-appointments-title"
        class="nhsuk-u-padding-bottom-5">
      {{ groupTitleWithCounter(totalConfirmedAppointments) }}
    </h2>
    <help-link
      id="wayfinder-help-jump-off-link-appointments"
      :href="wayfinderHelpPath"
      :click-func="redirectToWayfinderHelp"
      :text="$t('wayfinder.wayfinderHelp.indexPageJumpOffLinks.appointments')"/>
    <card-group v-if="hasAny"
                class="nhsuk-grid-row">
      <card-group-item v-for="(appointment, index) in confirmedAppointments"
                       :key="`confirmed-appointment-${index}`"
                       class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
        <component :is="getAppointmentCardComponent(appointment)"
                   :item="appointment" />
      </card-group-item>
    </card-group>
    <!--Card group is currently the only way to get this to act as the other groups style wise !-->
    <card-group v-else class="nhsuk-grid-row">
      <card-group-item class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
        <p
          id="no-confirmed-appointments-text">
          {{ $t('wayfinder.noConfirmedAppointments') }}
        </p>
      </card-group-item>
    </card-group>
  </div>
</template>

<script>
import AppointmentBookedCard from '@/components/appointments/hospital-referrals-appointments/appointments/AppointmentBookedCard';
import AppointmentCancelledCard from '@/components/appointments/hospital-referrals-appointments/appointments/AppointmentCancelledCard';
import AppointmentPendingChangeCard from '@/components/appointments/hospital-referrals-appointments/appointments/AppointmentPendingChangeCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import HelpLink from '@/components/appointments/hospital-referrals-appointments/HelpLink';
import { isNonEmptyArray, redirectTo } from '@/lib/utils';
import { WAYFINDER_HELP_PATH } from '@/router/paths';

export default {
  name: 'ConfirmedAppointmentsGroup',
  components: {
    AppointmentBookedCard,
    AppointmentCancelledCard,
    AppointmentPendingChangeCard,
    CardGroup,
    CardGroupItem,
    HelpLink,
  },
  data() {
    return {
      wayfinderHelpPath: WAYFINDER_HELP_PATH,
    };
  },
  computed: {
    hasAny() {
      return isNonEmptyArray(this.confirmedAppointments);
    },
    confirmedAppointments() {
      return this.$store.state.wayfinder.summary.confirmedAppointments;
    },
    totalConfirmedAppointments() {
      return this.$store.state.wayfinder.summary.confirmedAppointments.length;
    },
  },
  methods: {
    getAppointmentCardComponent(appointment) {
      if (appointment.appointmentStatus === 'booked') {
        return AppointmentBookedCard;
      }

      if (appointment.appointmentStatus === 'cancelled') {
        return AppointmentCancelledCard;
      }

      return AppointmentPendingChangeCard;
    },
    redirectToWayfinderHelp() {
      redirectTo(this, this.wayfinderHelpPath);
    },
    groupTitleWithCounter(count) {
      if (count === 0 || count > 1) {
        return this.$t('wayfinder.confirmedAppointmentsTitle')
          .replace('{count}', count)
          .replace('{plural}', 's');
      }

      return this.$t('wayfinder.confirmedAppointmentsTitle')
        .replace('{count}', count)
        .replace('{plural}', '');
    },
  },
};
</script>
