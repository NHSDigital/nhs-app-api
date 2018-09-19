<template>
  <form :class="$style.appointmentTime">
    <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
      <h2>{{ formatDate(daySlots[0]) }}</h2>
      <ul v-if="hasAppointments(daySlots)"
          :class="[$style['selector-list'], $style.appointmentTimeSelector]">
        <time-slot v-for="slot in daySlots[1]" :key="slot.ref" :ref="slot.ref"
                   :time-slot="slot" tabindex="0" @click.native="select(slot.ref)"
                   @keypress.native="keyPressed($event, slot.ref)"/>
      </ul>
      <p v-else :class="$style.noAppointments">{{ $t('appointments.booking.noSlots') }}</p>
    </span>
  </form>
</template>

<script>
/* eslint-disable import/extensions */
import TimeSlot from '@/components/appointments/booking/TimeSlot';
import DateProvider from '@/services/DateProvider';
import { APPOINTMENT_CONFIRMATIONS } from '@/lib/routes';

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
    formatDate: dateTime => DateProvider.create(dateTime).format('dddd D MMMM YYYY'),
    select(ref) {
      this.$refs[ref][0].select();
      this.$router.push(APPOINTMENT_CONFIRMATIONS.path);
    },
    hasAppointments(daySlots) {
      return daySlots[1].length > 0;
    },
    keyPressed(event, ref) {
      if (event.key === ' ' || event.key === 'Spacebar') {
        event.preventDefault();
        this.select(ref);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/selectors";
@import "../../../style/errorvalidation";
@import "../../../style/appointmentsnew";
@import "../../../style/accessibility";

  .noAppointments {
    margin-bottom: 1.2em;
  }
</style>
