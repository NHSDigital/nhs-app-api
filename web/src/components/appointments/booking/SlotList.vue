<template>
  <form :class="$style.appointmentTime">
    <div id="slotList" aria-labelledby="aLabelledBy" tabindex="-1" aria-hidden="true"/>
    <span v-for="daySlots in availableSlots" :key="formatDate(daySlots[0])">
      <h5>{{ formatDate(daySlots[0]) }}</h5>
      <ul :class="$style.selector">
        <li v-for="(slot, index) in daySlots[1]"
            :class="[slot.isSelected?$style.selected:undefined]"
            :aria-label="slot.isSelected?'selected-slot':undefined"
            :key="key(slot, index)"
            @click="selectSlot(slot, index)">
          {{ formatTime(slot.startTime) }}
        </li>
      </ul>
    </span>
  </form>
</template>

<script>
/* eslint-disable import/extensions */
import moment from 'moment-timezone';

export default {
  props: {
    availableSlots: {
      type: Array,
      default: () => [],
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
  },
  methods: {
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mm a'),
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    key(slot, index) {
      return `${slot.id}_${index}`;
    },
    selectSlot(slot) {
      this.$store.dispatch('availableAppointments/deselect');
      this.$store.dispatch('availableAppointments/select', slot);
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
    li {
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
  }
}

</style>
