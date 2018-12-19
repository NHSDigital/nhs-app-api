<template>
  <div :class="$style.panel">
    <h3 data-label="date">{{ formatDate(appointment.startTime) }}</h3>
    <h4 data-label="start time">{{ formatTime(appointment.startTime) }}</h4>
    <hr aria-hidden="true">
    <p :class="$style.session" data-label="session name">
      {{ appointment.type }}
    </p>
    <hr aria-hidden="true">

    <p :class="$style.location">
      <location-icon/>&nbsp;
      <span data-label="location">{{ appointment.location }}</span>
    </p>

    <p v-for="(clinician, index) in appointment.clinicians" :key="clinician"
       :class="$style.person">
      <clinician-icon/>&nbsp;
      <span :data-label="'clinician ' + (index +1)">
        {{ clinician }}
      </span>
    </p>

    <span v-if="showCancellationLink && !cancellationDisabled && !appointment.disableCancellation">
      <hr :class="$style.cancel" aria-hidden="true">
      <p>
        <a :class="$style['cancel-link']" :href="appointmentCancellingPath"
           @click.stop.prevent="onCancel">
          {{ this.$t('appointments.index.cancelButtonText') }}
        </a>
      </p>
    </span>
    <span v-if="showCancellationLink && (cancellationDisabled || appointment.disableCancellation)">
      <hr :class="$style.cancel" aria-hidden="true">
      <p :class="$style['cancel-disabled']">
        {{ this.$t('appointments.index.cancellationDisabledText') }}
      </p>
    </span>
  </div>
</template>

<script>
import moment from 'moment-timezone';
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';
import { APPOINTMENT_CANCELLING } from '@/lib/routes';
import { createUri } from '@/lib/noJs';

export default {
  components: {
    ClinicianIcon,
    LocationIcon,
  },
  props: {
    appointment: {
      type: Object,
      required: true,
    },
    showCancellationLink: {
      default: true,
      type: Boolean,
    },
    cancellationDisabled: {
      default: false,
      type: Boolean,
    },
  },
  computed: {
    appointmentCancellingPath() {
      const noJsData = {
        myAppointments: {
          selectedAppointment: this.appointment,
          cancellationReasons: this.$store.state.myAppointments.cancellationReasons,
        },
      };
      return createUri({ path: APPOINTMENT_CANCELLING.path, noJs: noJsData });
    },
  },
  methods: {
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mma'),
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    onCancel() {
      if (this.showCancellationLink) {
        this.$store.dispatch('myAppointments/select', this.appointment);
      }
      this.$router.push(APPOINTMENT_CANCELLING.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/panels";

.cancel {
  margin-top: 0.5em;
}
</style>
