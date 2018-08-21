<template>
  <form :class="$style.appointmentTime">
    <div :class="validationClass()">
      <error-message v-if="showErrorMessage()" id="error-slot">
        {{ $t('appointments.booking.validationErrors.slot') }}
      </error-message>
      <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
        <h2>{{ formatDate(daySlots[0]) }}</h2>
        <ul v-if="hasAppointments(daySlots)"
            :class="[$style['selector-list'], $style.appointmentTimeSelector]">
          <time-slot v-for="slot in daySlots[1]" :key="slot.ref" :ref="slot.ref"
                     :time-slot="slot" @click.native="select(slot.ref)" />
        </ul>
        <p v-else :class="$style.noAppointments">{{ $t('appointments.booking.noSlots') }}</p>
      </span>
    </div>
  </form>
</template>

<script>
/* eslint-disable import/extensions */
import TimeSlot from '@/components/appointments/booking/TimeSlot';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import DateProvider from '@/services/DateProvider';

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
    formatDate: dateTime => DateProvider.create(dateTime).format('dddd D MMMM YYYY'),
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
  }
</style>
