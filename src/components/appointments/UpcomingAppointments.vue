<template>
  <div :class="$style.content">
    <h2>{{ $t('appointments.index.upcoming.header') }}</h2>
    <p>{{ $t('appointments.index.upcoming.info') }}</p>
    <ul data-purpose="slots">
      <li v-for="(slot, index) in slots" :key="slot.id" :class="$style.slot">
        <appointment-slot :the-slot = "slot" :aria-label="'upcoming slot ' + (index +1)"/>
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
