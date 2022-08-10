<template>
  <card-group v-if="hasAny"
              class="nhsuk-grid-row">
    <card-group-item v-for="(appointment, index) in confirmedAppointments"
                     :key="`confirmed-appointment-${index}`"
                     class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
      <component :is="getAppointmentCardComponent(appointment)"
                 :item="appointment" />
    </card-group-item>
  </card-group>

  <p v-else
     id="no-confirmed-appointments-text">
    {{ $t('wayfinder.noConfirmedAppointments') }}
  </p>
</template>

<script>
import AppointmentBookedCard from '@/components/wayfinder/appointments/AppointmentBookedCard';
import AppointmentCancelledCard from '@/components/wayfinder/appointments/AppointmentCancelledCard';
import AppointmentPendingChangeCard from '@/components/wayfinder/appointments/AppointmentPendingChangeCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import { isNonEmptyArray } from '@/lib/utils';

export default {
  name: 'ConfirmedAppointmentsGroup',
  components: {
    AppointmentBookedCard,
    AppointmentCancelledCard,
    AppointmentPendingChangeCard,
    CardGroup,
    CardGroupItem,
  },
  computed: {
    hasAny() {
      return isNonEmptyArray(this.confirmedAppointments);
    },
    confirmedAppointments() {
      return this.$store.state.wayfinder.summary.confirmedAppointments;
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
  },
};
</script>
