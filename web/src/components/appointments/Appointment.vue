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

    <span v-if="showCancellationLink">
      <hr :class="$style.cancel" aria-hidden="true">
      <p>
        <nuxt-link :class="$style['cancel-link']" to="/appointments/cancel" @click.native="select">
          Cancel appointment
        </nuxt-link>
      </p>
    </span>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment-timezone';
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';

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
  },
  methods: {
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mma'),
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    select() {
      if (this.showCancellationLink) {
        this.$store.dispatch('myAppointments/select', this.appointment);
      }
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
