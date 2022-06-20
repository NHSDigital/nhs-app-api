<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.appointments.cancelled.title') }}
    </h3>
    <p data-purpose="appointment-date-time">
      <strong>{{ appointmentDateTime | fullDate }}</strong><br>
      <span>{{ appointmentDateTime | formatDate('h.mma') }}</span>
    </p>
    <p data-purpose="location-description"
       class="nhsuk-u-margin-bottom-3">
      {{ locationDescription }}
    </p>
    <p data-purpose="appointment-cancelled-info"
       class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.appointments.cancelled.anAppointmentHasBeenCancelled') }}
    </p>
    <p class="nhsuk-u-margin-bottom-3">
      <strong>
        <a data-purpose="view-appointment-link"
           href="#"
           @click="goToUrlViaRedirector(deepLinkUrl)">
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
    item: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      appointmentDateTime: this.item.appointmentDateTime,
      locationDescription: this.item.locationDescription,
      deepLinkUrl: this.item.deepLinkUrl,
    };
  },
};
</script>
