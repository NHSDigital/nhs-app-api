<template>
  <div :class="$style.appointments" >
    <div class="panel-title">
      <h2>{{ $t('appointments.index.upcoming.header') }}</h2>
    </div>

    <div v-for="appointment in appointments" :class="$style.panel" :key="appointment.id">
      <h5 aria-label="date">{{ formatDate(appointment.startTime) }}</h5>
      <h4 aria-label="start time">{{ formatTime(appointment.startTime) }}</h4>
      <hr aria-hidden="true">
      <p aria-label="session name">
        {{ displaySession(appointment) | truncate(24) }}
      </p>
      <hr aria-hidden="true">

      <p :class="$style.location" aria-label="location">
        <location-icon/>&nbsp;{{ displayName(appointment.location) | truncate(24) }}
      </p>

      <p v-for="clinician in appointment.clinicians" :key="clinician.id"
         :class="$style.clinician" aria-label="clinicians">
        <clinician-icon/>&nbsp;{{ displayName(clinician) | truncate(24) }}
      </p>

      <hr aria-hidden="true">
      <p>
        <nuxt-link to="#" @click.native="select(appointment)">
          {{ $t('appointments.index.cancelButtonText') }}
        </nuxt-link>
      </p>
    </div>
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
    appointments: {
      type: Array,
      default: () => [],
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
    select(appointment) {
      this.$store.dispatch('myAppointments/select', appointment);
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

  .appointments {
    margin-bottom: 80px;
  }

  .location, .clinician {
    margin-bottom: 8px;
  }

</style>
