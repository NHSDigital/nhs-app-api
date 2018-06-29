<template>
  <div :class="$style.panel">
    <h5 aria-label="date">{{ formatDate(appointment.startTime) }}</h5>
    <h4 aria-label="start time">{{ formatTime(appointment.startTime) }}</h4>
    <hr aria-hidden="true">
    <p aria-label="session name">
      {{ displaySession(appointment) | truncate(24) }}
    </p>
    <hr aria-hidden="true">

    <p :class="$style.location">
      <location-icon/>&nbsp;
      <span aria-label="location">{{ displayName(appointment.location) | truncate(24) }}</span>
    </p>

    <p v-for="(clinician, index) in appointment.clinicians" :key="clinician.id"
       :class="$style.clinician">
      <clinician-icon/>&nbsp;
      <span :aria-label="'clinician ' + (index +1)">
        {{ displayName(clinician) | truncate(24) }}
      </span>
    </p>

    <span v-if="showCancellationLink">
      <hr aria-hidden="true">
      <p>
        <nuxt-link to="/appointments/cancel" @click.native="select">
          Cancel appointment
        </nuxt-link>
      </p>
    </span>

  </div>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment';
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
    formatTime: dateTime => moment(dateTime).format('h:mm a'),
    formatDate: dateTime => moment(dateTime).format('dddd D MMMM YYYY'),
    displayName(property) {
      return (property) ? property.displayName : '';
    },
    displaySession(appointment) {
      const slotType = (appointment.slotType) ? ` - ${appointment.slotType}` : '';
      return this.displayName(appointment.appointmentSession) + slotType;
    },
    select() {
      if (this.showCancellationLink) {
        this.$store.dispatch('myAppointments/select', this.appointment);
      }
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";
  @import '../../style/fonts';
  @import '../../style/colours';
  @import '../../style/elements';

  .panel {
     width: 100%;
     box-sizing: border-box;
     padding: 16px;
     margin-bottom: 16px;
     margin-top: 8px;
     background-color: $white;
     -webkit-box-shadow: 1px 1px 2px 0px rgba(0, 0, 0, 0.2);
     -moz-box-shadow: 1px 1px 2px 0px rgba(0, 0, 0, 0.2);
     box-shadow: 1px 1px 2px 0px rgba(0, 0, 0, 0.2);
     display: table;
     transition: all ease 0.5s;
   }

  .panel p {
    display: block;
    font-weight: normal;
    font-size: 1em;
    line-height: 1.5em;
    color: #4A4A4A;
  }

  .location, .clinician {
    margin-bottom: 8px;
  }

</style>
