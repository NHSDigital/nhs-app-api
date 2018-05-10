<template>
  <appointment-slot>
    <div v-bind:class="getClass()" v-on:click="select">
      <h2 :class="$style.startTime">{{ formatTime(appointmentSlot.startTime) }}</h2>
      <h3 :class="$style.date">{{ formatDate(appointmentSlot.startTime) }}</h3>
      <hr/>
      <h3 :class="$style.session">{{ appointmentSession | truncate(24)}}</h3>
      <hr/>
      <div>
        <location-icon/>
        &nbsp;{{ location | truncate(24) }}
      </div>
      <ul :key="clinician.id"
          v-for="clinician in appointmentSlot.clinicians" :class="$style.clinicians">
        <li title="getType();">
          <clinician-icon/>
          &nbsp;{{ clinician.displayName | truncate(24) }}
        </li>
      </ul>
    </div>
  </appointment-slot>
</template>

<script>
import { find, get } from 'lodash/fp';
import { mapGetters } from 'vuex';
import moment from 'moment';
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';

export default {
  components: {
    ClinicianIcon,
    LocationIcon,
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
    getClass() {
      return this.isSelected(this.slotId) ? this.$style.selectedContainer : this.$style.container;
    },
  },
  props: {
    slotId: {
      type: String,
    },
  },
};
</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/colours';
  @import '../../style/spacings';

  .startTime {
    @include space(margin, bottom, $one);
  }

  .date {
    font-size: 1.1em;
    font-weight: normal;
  }

  .session {
    font-weight: normal;
  }

  .selectedContainer {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    @include space(padding, all, $three);
    transition: all ease 0.5s;
    hr {
      height: 1px;
      border: none;
      background-color: $dark_grey;
      opacity: 0.2;
      @include space(margin, top, $two);
      @include space(margin, bottom, $two);
    }
    background: $nhs_blue;
    color: $white;
  }

  .container {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    background: $white;
    @include space(padding, all, $three);
    transition: all ease 0.5s;
    hr {
      height: 1px;
      border: none;
      background-color: $dark_grey;
      opacity: 0.2;
      @include space(margin, top, $two);
      @include space(margin, bottom, $two);
    }
  }

  .clinicians {
    font-size: 0.9em;
  }

  li {
    list-style: none;
  }
</style>
