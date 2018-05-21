<template>
  <div>
    <appointments-no-connection v-if="noConnection" @retry="onRetryButtonClicked"/>
    <div v-else id="mainDiv">
      <spinner />
      <main :class="bottomStyle()">
        <div v-if="showNoAppointment" data-purpose="no-slots-error">
          <p :class="$style.summary">{{ $t('appointments.noSlotErrorMessage.summary') }}</p>
          <p :class="$style.info">{{ $t('appointments.noSlotErrorMessage.info') }}</p>
        </div>
        <ul v-if="showAppointments" data-purpose="slots">
          <li v-for="slot in slots" :key="slot.id" :class="$style.slot">
            <appointment-slot :slot-id="slot.id"/>
          </li>
        </ul>

        <floating-button-bottom :button-classes="['green']"
                                :clickable="hasASlotSelected" @on-click="onBookButtonClicked">
          {{ $t('appointments.bookAppointmentButtonText') }}
        </floating-button-bottom>
      </main>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { mapGetters } from 'vuex';
import AppointmentsNoConnection from '../../components/AppointmentsNoConnection';
import AppointmentSlot from '../../components/AppointmentSlot';
import Spinner from '../../components/Spinner';
import FloatingButtonBottom from '../../components/FloatingButtonBottom';

export default {
  middleware: ['auth', 'meta'],
  components: {
    AppointmentsNoConnection,
    AppointmentSlot,
    Spinner,
    FloatingButtonBottom,
  },
  data() {
    return {
      noConnection: true,
    };
  },
  computed: {
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
      return typeof this.currentSlot !== 'undefined';
    },
    ...mapGetters({
      slots: 'appointmentSlots/slots',
      findById: 'appointmentSlots/findById',
      currentSlot: 'appointmentSlots/currentSlot',
    }),
  },
  mounted() {
    this.$store.dispatch('appointmentSlots/load', this.$config);
    this.$store.dispatch('appointmentSlots/select', undefined);
    this.noConnection = !navigator.onLine;
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
    onRetryButtonClicked() {
      this.noConnection = !navigator.onLine;
    },
    onBookButtonClicked() {
      this.$router.push('appointment-confirmation');
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
