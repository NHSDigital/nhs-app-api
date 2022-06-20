<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.appointments.readyToConfirm.title') }}
    </h3>
    <p data-purpose="location-description">
      {{ locationDescription }}
    </p>
    <p data-purpose="appointment-advice">
      {{ $t('wayfinder.appointments.readyToConfirm.anAppointmentHasBeenBooked') }}
    </p>
    <primary-button data-purpose="contact-the-clinic-button"
                    @click="goToUrlViaRedirector(deepLinkUrl)">
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
    item: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      locationDescription: this.item.locationDescription,
      deepLinkUrl: this.item.deepLinkUrl,
    };
  },
};
</script>
