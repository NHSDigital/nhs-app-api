<template>
  <card-group v-if="hasConfirmedAppointments" class="nhsuk-grid-row">
    <card-group-item
      v-for="(appointment, index) in confirmedAppointments"
      :key="`confirmed-appointment-${index}`"
      class="nhsuk-grid-column-three-quarters">

      <appointment-booked-card
        :appointment-id="index"
        :location-description="appointment.locationDescription"
        :appointment-date-time="appointment.appointmentDateTime"
        :deep-link-url="appointment.deepLinkUrl"/>
    </card-group-item>
  </card-group>
  <p v-else id="no-confirmed-appointments-text">
    {{ $t('wayfinder.noConfirmedAppointments') }}
  </p>
</template>

<script>
import AppointmentBookedCard from '@/components/wayfinder/appointments/AppointmentBookedCard';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

export default {
  name: 'ConfirmedAppointmentsCard',
  components: {
    AppointmentBookedCard,
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
};
</script>
