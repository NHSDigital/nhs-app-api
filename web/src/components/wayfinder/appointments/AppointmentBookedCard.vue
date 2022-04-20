<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.appointments.booked.title') }}
    </h3>
    <p :id="`datetime-${appointmentId}`">
      <strong>{{ appointmentDateTime | fullDate }}</strong><br>
      <span>{{ appointmentDateTime | formatDate('h:mma') }}</span>
    </p>
    <p :id="`location-description-${appointmentId}`" class="nhsuk-u-margin-bottom-3">
      {{ locationDescription }}
    </p>
    <primary-button :id="`bookOrManageAppointment-${appointmentId}`">
      {{ $t('wayfinder.appointments.booked.viewOrManageAppointment') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';

export default {
  name: 'AppointmentBookedCard',
  components: {
    Card,
    PrimaryButton,
  },
  props: {
    appointmentId: {
      type: String,
      default: '',
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
