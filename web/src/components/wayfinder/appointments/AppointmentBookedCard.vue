<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.appointments.booked.title') }}
    </h3>
    <p data-purpose="appointment-date-time">
      <strong>{{ appointmentDateTime | fullDate }}</strong><br>
      <span>{{ appointmentDateTime | formatDate('h.mma') }}</span>
    </p>
    <p data-purpose="location-description"
       class="nhsuk-u-margin-bottom-3">
      {{ locationDescription }}
    </p>
    <primary-button data-purpose="book-or-manage-appointment-button"
                    @click="goToUrlViaRedirector(deepLinkUrl)">
      {{ $t('wayfinder.appointments.booked.viewOrManageAppointment') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'AppointmentBookedCard',
  components: {
    Card,
    PrimaryButton,
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
