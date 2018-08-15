<template>
  <li
    :class="[isSelected?$style.selected:undefined, $style.selector]"
    :aria-label="isSelected?'selected-slot':undefined" >
    {{ formatTime(timeSlot.startTime) }}
  </li>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment-timezone';

export default {
  props: {
    timeSlot: {
      type: Object,
      default: () => {},
      required: true,
    },
  },
  data() {
    return {
      isSelected: false,
    };
  },
  methods: {
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mma'),
    select() {
      this.isSelected = true;
      this.$store.dispatch('availableAppointments/select', this.timeSlot);
    },
    deselect() {
      this.isSelected = false;
      this.$store.dispatch('availableAppointments/deselect');
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/selectors";
.selector {
  user-select: none;
}
</style>
