<template>
  <form :class="[$style.appointmentTime, isDesktopWeb && $style.desktopWeb]">
    <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
      <div data-purpose="appointment-day">
        <h2 :class="[isDesktopWeb && $style.desktopWeb]"
            data-purpose="appointment-day-heading">{{ formatDate(daySlots[0]) }}</h2>
        <ul v-if="hasAppointments(daySlots)"
            :class="[$style['selector-list'], $style.appointmentTimeSelector]">
          <time-slot v-for="slot in daySlots[1]"
                     :key="slot.ref"
                     :ref="slot.ref"
                     :time-slot="slot"
                     @click.native="select(slot.ref)"
                     @select="select($event)" />
        </ul>
        <p v-else :class="[$style.noAppointments, isDesktopWeb && $style.desktopWeb]">
          {{ $t('appointments.booking.noSlots') }}</p>
      </div>
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
  data() {
    return {
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
    };
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
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/selectors";
@import "../../../style/errorvalidation";
@import "../../../style/appointmentsnew";

  .noAppointments {
    margin-bottom: 1.2em;
   &.desktopWeb {
    font-family: $default-web;
    font-weight: lighter;
   }
  }
</style>
