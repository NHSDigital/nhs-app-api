<template>
  <div class="container" v-on:click="select" :class="{ selected: isSelected(appointmentSlot.id) }">
    <h2 class="start-time">{{ formatTime(appointmentSlot.startTime) }}</h2>
    <h3 class="date">{{ formatDate(appointmentSlot.startTime) }}</h3>
    <hr/>
    <h3 class="session">{{ appointmentSession | truncate(24)}}</h3>
    <hr/>
    <div class="location">
      <location-icon/>
      &nbsp;{{ location | truncate(24) }}
    </div>
    <ul :key="clinician.id" v-for="clinician in appointmentSlot.clinicians" class="clinicians">
      <li>
        <clinician-icon/>
        &nbsp;{{ clinician.displayName | truncate(24) }}
      </li>
    </ul>
  </div>
</template>

<script>
import { find, get } from 'lodash/fp';
import { mapGetters } from 'vuex';
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
  computed: {
    appointmentSlot() {
      return find(slot => slot.id === this.slotId)(this.$store.state.appointmentSlots.slots) || {};
    },
    appointmentSession() {
      return get('appointmentSession.displayName')(this.appointmentSlot);
    },
    location() {
      return get('location.displayName')(this.appointmentSlot);
    },
    ...mapGetters({
      isSelected: 'appointmentSlots/isSelected',
    }),
  },
  methods: {
    formatTime: dateTime => moment(dateTime).format('h:mm a'),
    formatDate: dateTime => moment(dateTime).format('dddd D MMMM YYYY'),
    select() {
      this.$store.dispatch('appointmentSlots/select', this.slotId);
    },
  },
  props: {
    slotId: {
      type: String,
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
    font-weight: normal;
  }

  div.container {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    background: $white;
    padding: 18px;
    transition: all ease 0.5s;
    hr {
      height: 1px;
      border: none;
      background-color: #4A4A4A;
      opacity: 0.2;
      margin-bottom: 16px;
      margin-top: 16px;
    }

    &.selected {
      background: #005EB8;
      color: $white;
    }

  }

  div.clinicians {
    font-size: 0.9em;
  }

  li {
    list-style: none;
  }
</style>
