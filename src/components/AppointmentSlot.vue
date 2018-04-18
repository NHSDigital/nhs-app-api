<template>
  <div class="container">
    <h2 class="start-time">{{ formatTime(appointmentSlot.startTime) }}</h2>
    <h3 class="date">{{ formatDate(appointmentSlot.startTime) }}</h3>
    <h3 class="session">{{ appointmentSlot.appointmentSession.displayName }}</h3>
    <div class="location">
      <location-icon/>
      {{ appointmentSlot.location.displayName }}
    </div>
    <ul :key="clinician.id" v-for="clinician in appointmentSlot.clinicians" class="clinicians">
      <li>
        <clinician-icon/>
        {{ clinician.displayName }}
      </li>
    </ul>
  </div>
</template>

<script>
import moment from 'moment';
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';
import Spinner from '@/components/Spinner';

export default {
  components: {
    ClinicianIcon,
    LocationIcon,
    Spinner,
  },
  methods: {
    formatTime: dateTime => moment(dateTime).format('h:mm a'),
    formatDate: dateTime => moment(dateTime).format('dddd D MMMM YYYY'),
  },
  props: {
    appointmentSlot: {
      type: Object,
    },
  },
};
</script>

<style lang="scss" scoped>
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';
  @import '../style/buttons';

  h2 {
    margin-bottom: 8px;
  }

  h3.date {
    font-size: 1.1em;
    font-weight: normal;
  }

  h3.session {
    margin: 15px 0 15px 0;
    padding: 15px 0 15px 0;
    border-top: solid 1px $dark_white;
    border-bottom: solid 1px $dark_white;
    font-weight: normal;
  }

  div.container {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    background: $white;
    padding: 18px;
  }

  div.location {
    font-size: 0.9em;
  }

  li {
    list-style: none;
  }
</style>
