<template>
  <main :class="bottomStyle()">
    <div v-if="showNoAppointment" data-purpose="no-slots-error">
      <p :class="$style.summary">{{ $t('appointments.noSlotErrorMessage.summary') }}</p>
      <p :class="$style.info">{{ $t('appointments.noSlotErrorMessage.info') }}</p>
    </div>
    <ul v-if="showAppointments" data-purpose="slots">
      <li v-for="(slot, index) in slots" :key="slot.id" :class="$style.slot">
        <appointment-slot :the-slot = "slot" :aria-label="'slot ' + (index +1)"/>
      </li>
    </ul>

    <floating-button-bottom
      :button-classes="['green']"
      :clickable="hasASlotSelected"
      @on-click="onBookButtonClicked"
    >
      {{ $t('appointments.booking.bookButtonText') }}
    </floating-button-bottom>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import { mapGetters } from 'vuex';
import AppointmentSlot from '../../components/AppointmentSlot';
import FloatingButtonBottom from '../../components/FloatingButtonBottom';

export default {
  middleware: ['auth', 'meta'],
  components: {
    AppointmentSlot,
    FloatingButtonBottom,
  },
  computed: {
    ...mapGetters({
      slots: 'appointmentSlots/slots',
      findById: 'appointmentSlots/findById',
    }),
    showNoAppointment() {
      return (
        this.$store.state.appointmentSlots.hasLoaded &&
        this.$store.state.appointmentSlots.slots.length === 0
      );
    },
    showAppointments() {
      return (
        this.$store.state.appointmentSlots.hasLoaded &&
        this.$store.state.appointmentSlots.slots.length > 0
      );
    },
    hasASlotSelected() {
      return this.slots.filter(slot => slot.selected === true).length > 0;
    },
  },
  mounted() {
    this.$store.dispatch('appointmentSlots/load');
  },
  beforeDestroy() {
    this.$store.dispatch('appointmentSlots/reset');
  },
  methods: {
    bottomStyle() {
      if (
        this.$store.state.appointmentSlots.hasLoaded &&
        this.$store.state.appointmentSlots.slots.length !== 0
      ) {
        return this.$style.mainShowingSlots;
      }
      return this.$style.main;
    },
    onBookButtonClicked() {
      this.$store.dispatch('appointment/save');
      this.$router.push('/appointments/confirmation');
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/spacings";

  .main {
    @include space(padding, all, $three);
  }

  .mainShowingSlots {
    @include space(padding, all, $three);
    padding-bottom: 78px;
  }

  .summary {
    font-weight: bold;
    @include space(margin, bottom, $three);
  }

  .info {
    @include default_text;
    font-size: 12pt;
    @include space(margin, bottom, $three);
  }

  .slot {
    list-style: none;
    @include space(margin, bottom, $three);
  }

  .summary {
    font-weight: bold;
    @include space(margin, bottom, $three);
  }

</style>
