<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.appointments.cancelled.title') }}
    </h3>
    <p :id="`datetime-${appointmentId}`">
      <strong>{{ appointmentDateTime | fullDate }}</strong><br>
      <span>{{ appointmentDateTime | formatDate('h.mma') }}</span>
    </p>
    <p :id="`location-description-${appointmentId}`" class="nhsuk-u-margin-bottom-3">
      {{ locationDescription }}
    </p>
    <p :id="`cancelled-message-${appointmentId}`" class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.appointments.cancelled.anAppointmentHasBeenCancelled') }}
    </p>
    <p :id="`view-this-appointment-${appointmentId}`" class="nhsuk-u-margin-bottom-3">
      <strong>
        <a href="#">
          {{ $t('wayfinder.appointments.cancelled.viewThisAppointment') }}
        </a>
      </strong>
    </p>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';

export default {
  name: 'AppointmentCancelledCard',
  components: {
    Card,
  },
  props: {
    appointmentId: {
      type: Number,
      required: true,
    },
    appointmentDateTime: {
      type: String,
      default: '',
    },
    locationDescription: {
      type: String,
      default: '',
    },
  },
};
</script>
