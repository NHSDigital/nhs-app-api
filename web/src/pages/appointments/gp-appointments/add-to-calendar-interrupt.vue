<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-two-thirds">
        <h1 class="nhsuk-heading-xl nhs-app-xl">
          <span class="nhsuk-caption-m nhsuk-caption-–top">
            {{ formatDate(appointment.startTime) }} at {{ formatTime(appointment.startTime) }}
          </span>
          {{ $t('appointments.addToCalendar.paragraph1') }}
        </h1>

        <p>{{ $t('appointments.addToCalendar.paragraph2') }}</p>

        <primary-button id="addToCalendarButton" @click.stop.prevent="onAddToCalendar">
          {{ $t('appointments.index.addToCalendarText') }}
        </primary-button>
      </div>
    </div>
  </div>
</template>

<script>
import moment from 'moment-timezone';
import { APPOINTMENTS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import PrimaryButton from '@/components/PrimaryButton';
import NativeApp from '@/services/native-app';

export default {
  name: 'AddToCalendarInterruptPage',
  components: {
    PrimaryButton,
  },
  data() {
    return {
      appointment: null,
    };
  },
  created() {
    this.appointment = this.$store.state.myAppointments.selectedAppointment;
    if (!this.appointment) {
      redirectTo(this, APPOINTMENTS_PATH);
    }
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mma'),

    onAddToCalendar() {
      this.$store.dispatch('availableAppointments/completeBookingJourney');
      this.$store.dispatch('availableAppointments/deselect');

      const subject = this.appointment.type;
      let body = this.appointment.sessionName;
      this.appointment.clinicians.forEach((clinician) => {
        if (body) {
          body += `\n${clinician}`;
        } else {
          body += `${clinician}`;
        }
      });

      const { location } = this.appointment;
      const startTimeEpochInSeconds = moment(this.appointment.startTime, 'YYYY-M-D H:mm').valueOf() / 1000;
      const endTimeEpochInSeconds = startTimeEpochInSeconds;

      const calendarEvent = JSON.stringify({
        subject,
        body,
        location,
        startTimeEpochInSeconds,
        endTimeEpochInSeconds,
      });

      NativeApp.addEventToCalendar(calendarEvent);
      redirectTo(this, APPOINTMENTS_PATH);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';

h1 {
  @include nhsuk-responsive-margin(4, "bottom");
}

</style>
