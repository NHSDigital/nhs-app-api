<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-two-thirds">

        <p>{{ $t('appointments.appointment.addToCalendar.yourCalendarWillNotUpdate') }}</p>

        <primary-button id="addToCalendarButton" @click.stop.prevent="onAddToCalendar">
          {{ $t('appointments.appointment.addToCalendar.addToCalendar') }}
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
import { EventBus, UPDATE_HEADER, UPDATE_TITLE, UPDATE_LOCALISED_CAPTION } from '@/services/event-bus';

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
    EventBus.$emit(UPDATE_HEADER, {
      headerKey: 'appointments.appointment.addToCalendar.ifThisAppointmentChanges',
    });
    EventBus.$emit(UPDATE_TITLE, 'appointments.appointment.addToCalendar.ifThisAppointmentChanges');

    EventBus.$emit(UPDATE_LOCALISED_CAPTION,
      `${this.formatDate(this.appointment.startTime)} at ${this.formatTime(this.appointment.startTime)}`);
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
      const startTimeEpochInSeconds = moment(this.appointment.startTime).valueOf() / 1000;
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
  @import "@/style/custom/add-to-calendar-interrupt";
</style>
