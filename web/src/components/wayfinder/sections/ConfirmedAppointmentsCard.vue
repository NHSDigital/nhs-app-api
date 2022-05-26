<template>
  <card-group v-if="hasConfirmedAppointments" class="nhsuk-grid-row">
    <card-group-item
      v-for="(appointment, index) in confirmedAppointments"
      :key="`confirmed-appointment-${index}`"
      class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
      <appointment-booked-card
        v-if="isBooked(appointment)"
        :appointment-id="index"
        :location-description="appointment.locationDescription"
        :appointment-date-time="appointment.appointmentDateTime"
        :deep-link-url="appointment.deepLinkUrl"/>
      <appointment-cancelled-card
        v-else-if="isCancelled(appointment)"
        :appointment-id="index"
        :location-description="appointment.locationDescription"
        :appointment-date-time="appointment.appointmentDateTime"/>
    </card-group-item>
  </card-group>
  <p v-else id="no-confirmed-appointments-text">
    {{ $t('wayfinder.noConfirmedAppointments') }}
  </p>
</template>

<script>
import AppointmentBookedCard from '@/components/wayfinder/appointments/AppointmentBookedCard';
import AppointmentCancelledCard from '@/components/wayfinder/appointments/AppointmentCancelledCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

export default {
  name: 'ConfirmedAppointmentsCard',
  components: {
    AppointmentBookedCard,
    AppointmentCancelledCard,
    CardGroup,
    CardGroupItem,
  },
  props: {
    hasConfirmedAppointments: {
      type: Boolean,
      default: false,
    },
    confirmedAppointments: {
      type: Array,
      default: null,
    },
  },
  methods: {
    isBooked(appointment) {
      return appointment.appointmentStatus === 'booked';
    },
    isCancelled(appointment) {
      return appointment.appointmentStatus === 'cancelled';
    },
  },
};
</script>
