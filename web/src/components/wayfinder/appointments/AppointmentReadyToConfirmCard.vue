<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.appointments.readyToConfirm.title') }}
    </h3>
    <p>
      {{ $t('wayfinder.appointments.readyToConfirm.anAppointmentHasBeenBooked') }}
    </p>
    <p :id="`location-description-${appointmentId}`">
      {{ locationDescription }}
    </p>
    <primary-button :id="`contactTheClinicToConfirm-${appointmentId}`" @click="onClick">
      {{ $t('wayfinder.appointments.readyToConfirm.contactTheClinicToConfirm') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'AppointmentReadyToConfirmCard',
  components: {
    Card,
    PrimaryButton,
  },
  mixins: [RedirectorMixin],
  props: {
    appointmentId: {
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
