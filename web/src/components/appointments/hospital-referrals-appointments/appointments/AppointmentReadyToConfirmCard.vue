<template>
  <Card class="nhsuk-u-margin-bottom-5">

    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <h3 class="nhs-app-card__title-text nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0">
        {{ $t('wayfinder.appointments.readyToConfirm.title') }}
      </h3>
      <span data-label="status-text"
            class="nhsuk-tag nhs-app-card__title-tag"
            :class="[tagStyle]"
            :aria-label="tagAriaLabel">
        {{ tagText }}
      </span>
    </div>

    <p class="nhsuk-body-s nhsuk-u-margin-bottom-3" data-purpose="appointment-advice">
      {{ $t('wayfinder.appointments.readyToConfirm.anAppointmentHasBeenBooked') }}
    </p>
    <p class="nhsuk-body-l" data-purpose="location-description">
      {{ locationDescription }}
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
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';

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
  computed: {
    tagText() {
      return this.$t('wayfinder.appointments.readyToConfirm.tagText');
    },
    tagAriaLabel() {
      return `${this.$t('wayfinder.appointments.tagPromptText')} ${this.tagText}`;
    },
    tagStyle() {
      return this.tagText === 'Action' ? 'nhsuk-tag--red' : 'nhsuk-tag--yellow';
    },
  },
};
</script>
