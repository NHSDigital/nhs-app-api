<template>
  <div :class="$style.content">
    <h2>Past appointments</h2>
    <span class="custom-dropdown custom-dropdown--white">
      <select class="custom-dropdown__select custom-dropdown__select--white" id="type">
        <option v-for="(filterName,index) in filterOptions" :key="index">
          {{ filterName }}
        </option>
      </select>
    </span>
    <ul data-purpose="slots">
      <li v-for="(slot, index) in slots" :key="slot.id" :class="$style.slot">
        <appointment-slot :the-slot="slot" :aria-label="'past appointment slot ' + (index +1)" />
      </li>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import AppointmentSlot from '@/components/appointments/AppointmentSlot';
export default {
  components: {
    AppointmentSlot,
  },
  props: {
    slots: {
      type: Array,
      default: [],
    },
  },
  data() {
    return {
      filterOptions: [
        'Past month',
        'Past two months',
        'Past six months',
        'Past year',
        'All repeat prescriptions'
      ],
    };
  },
  methods: {
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";
  @import '../../style/fonts';
  @import '../../style/colours';

  .content {
    h2 {
      display: block;
      font-family: $frutiger-roman;
      font-weight: 700;
      font-size: 22px;
      line-height: 24px;
      color: $nhs_blue;
      margin-bottom: 16px;
    }

    p {
      display: block;
      font-family: $default;
      font-weight: normal;
      font-size: 16px;
      line-height: 22px;
      color: $dark_grey;
      margin-bottom: 16px;
    }

    .slot {
      list-style: none;
      @include space(margin, bottom, $three);
    }
  }

</style>
