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
        <a href="#" @click="goToUrlViaRedirector(deepLinkUrl)">
          {{ $t('wayfinder.appointments.cancelled.viewThisAppointment') }}
        </a>
      </strong>
    </p>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'AppointmentCancelledCard',
  components: {
    Card,
  },
  mixins: [RedirectorMixin],
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
    deepLinkUrl: {
      type: String,
      required: true,
    },
  },
};
</script>
