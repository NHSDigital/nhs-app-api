<template>
  <li
    :class="[isSelected?$style.selected:undefined, $style.timeSlot]"
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
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mm a'),
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

<style module lang="scss">
@import "../../../style/textstyles";
@import "../../../style/colours";
@import "../../../style/fonts";
  .timeSlot {
    @include default_text;
    width: 101px;
    box-sizing: border-box;
    white-space: nowrap;
    text-align: center;
    background-color: $white;
    border: 1px $light_grey solid;
    box-sizing: border-box;
    border-radius: 5px;
    padding: 16px;
    list-style: none;
    margin: 8px;
    transition: all ease 0.5s;
    &:last-child {
      margin-right: 0px;
    }
  }
  .selected {
    background-color: $nhs_blue;
    border: 1px $nhs_blue solid;
    color: $white;
    label {
      color: $white;
    }
  }
</style>
