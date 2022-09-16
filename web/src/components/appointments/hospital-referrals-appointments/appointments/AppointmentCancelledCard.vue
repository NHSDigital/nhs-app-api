<template>
  <Card class="nhsuk-u-margin-bottom-5">

    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <h3 class="nhs-app-card__title-text nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0">
        {{ $t('wayfinder.appointments.cancelled.title') }}
      </h3>
    </div>

    <p class="nhsuk-body-s nhsuk-u-margin-bottom-3" data-purpose="appointment-cancelled-info">
      {{ $t('wayfinder.appointments.cancelled.anAppointmentHasBeenCancelled') }}
    </p>

    <p data-purpose="appointment-date-time">
      <strong>
        <span class="nhsuk-body">{{ appointmentDateTime | fullDate }}</span><br>
        <span class="nhsuk-body-l">{{ appointmentDateTime | formatDate('h.mma') }}</span>
      </strong>
    </p>

    <p class="nhsuk-body-l nhsuk-u-margin-bottom-3" data-purpose="location-description">
      {{ locationDescription }}
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
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';

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
