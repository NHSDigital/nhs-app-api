<template>
  <Card class="nhsuk-u-margin-bottom-5">

    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <h3 class="nhs-app-card__title-text nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0">
        {{ $t('wayfinder.appointments.pendingChange.title') }}
      </h3>
      <span data-label="status-text"
            class="nhsuk-tag nhs-app-card__title-tag"
            :class="[tagStyle]"
            :aria-label="tagAriaLabel">
        {{ tagText }}
      </span>
    </div>

    <p class="nhsuk-body-s nhsuk-u-margin-bottom-3" data-purpose="appointment-advice">
      {{ $t('wayfinder.appointments.pendingChange.requestToChangeOrCancel') }}
    </p>

    <p data-purpose="appointment-date-time">
      <strong>
        <span class="nhsuk-body">{{ appointmentDateTime | fullDate }}</span><br>
        <span class="nhsuk-body-l">{{ appointmentDateTime | formatDate('h.mma') }}</span>
      </strong>
    </p>

    <p class="nhsuk-body-l" data-purpose="location-description">
      {{ locationDescription }}
    </p>
    <p class="nhsuk-u-margin-bottom-3">
      <strong>
        <a data-purpose="view-appointment-link"
           href="#"
           @click="goToUrlViaRedirector(deepLinkUrl)">
          {{ $t('wayfinder.appointments.pendingChange.viewThisAppointment') }}
        </a>
      </strong>
    </p>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';

export default {
  name: 'AppointmentPendingChangeCard',
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
  computed: {
    tagText() {
      return this.$t('wayfinder.appointments.pendingChange.tagText');
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
