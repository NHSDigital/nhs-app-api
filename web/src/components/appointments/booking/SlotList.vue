<template>
  <form :class="$style.appointmentTime">
    <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
      <h5>{{ formatDate(daySlots[0]) }}</h5>
      <ul :class="$style.selector">
        <time-slot v-for="slot in daySlots[1]" :key="slot.ref" :ref="slot.ref"
                   :time-slot="slot" @click.native="select(slot.ref)" />
      </ul>
    </span>
  </form>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment-timezone';
import TimeSlot from '@/components/appointments/booking/TimeSlot';

export default {
  components: {
    TimeSlot,
  },
  props: {
    availableSlots: {
      type: Array,
      default: () => [],
    },
  },
  methods: {
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    select(ref) {
      if (this.$store.state.availableAppointments.selectedSlot) {
        this.$refs[this.$store.state.availableAppointments.selectedSlot.ref][0].deselect();
      }

      this.$refs[ref][0].select();
    },
  },
};
</script>

<style module lang="scss">
@import "../../../style/textstyles";
@import "../../../style/colours";
@import "../../../style/fonts";
.appointmentTime {
  overflow: hidden;
  margin-bottom: 8px;
  h5 {
    @include h5;
    color: $nhs_blue;
    margin-bottom: 8px;
  }
  .selector {
    display: flex;
    flex-wrap: wrap;
    margin-left: -8px;
    margin-bottom: 8px;
  }
}

</style>
