<template>
  <form :class="$style.appointmentTime">
    <div :class="validationClass()">
      <error-message v-if="showErrorMessage()" id="error-slot">
        {{ $t('appointments.booking.validationErrors.slot') }}
      </error-message>
      <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
        <h2>{{ formatDate(daySlots[0]) }}</h2>
        <ul :class="[$style['selector-list'], $style.appointmentTimeSelector]">
          <time-slot v-for="slot in daySlots[1]" :key="slot.ref" :ref="slot.ref"
                     :time-slot="slot" @click.native="select(slot.ref)" />
        </ul>
      </span>
    </div>
  </form>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment-timezone';
import TimeSlot from '@/components/appointments/booking/TimeSlot';
import ErrorMessage from '@/components/widgets/ErrorMessage';

export default {
  components: {
    TimeSlot,
    ErrorMessage,
  },
  props: {
    availableSlots: {
      type: Array,
      default: () => [],
    },
    showValidationError: {
      type: Boolean,
      default: false,
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
    showErrorMessage() {
      return this.showValidationError && this.availableSlots.length > 0;
    },
    validationClass() {
      const validationClass = [this.$style['validation-inline']];
      if (this.showErrorMessage()) {
        validationClass.push(this.$style['validation-border-left']);
      }

      return validationClass;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/selectors";
@import "../../../style/errorvalidation";
@import "../../../style/appointmentsnew";

</style>
