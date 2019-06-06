<template>
  <form :class="[$style.appointmentTime, !$store.state.device.isNativeApp && $style.desktopWeb]">
    <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
      <div v-if="hasAppointments(daySlots)" data-purpose="appointment-day">
        <h2 data-purpose="appointment-day-heading">{{ formatDate(daySlots[0]) }}</h2>
        <ul
          :class="[$style['selector-list'], $style.appointmentTimeSelector]">
          <time-slot v-for="slot in daySlots[1]"
                     :key="slot.ref"
                     :ref="slot.ref"
                     :time-slot="slot"
                     @click.native="select(slot.ref)"
                     @select="select($event)" />
        </ul>
      </div>
    </span>
  </form>
</template>

<script>
/* eslint-disable import/extensions */
import TimeSlot from '@/components/appointments/booking/TimeSlot';
import DateProvider from '@/services/DateProvider';
import { APPOINTMENT_CONFIRMATIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'SlotList',
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
      redirectTo(this, APPOINTMENT_CONFIRMATIONS.path, null);
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

form {
 &.desktopWeb {
   max-width: 560px;
   padding-right: 1em;

  h2 {
   font-family: $default-web;
   font-weight: lighter;
  }
  .noAppointments {
    font-family: $default-web;
    font-weight: lighter;
   }
 }
}
  .noAppointments {
    margin-bottom: 1.2em;
  }
</style>
