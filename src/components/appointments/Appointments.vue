<template>
<div>
  <appointments-no-connection v-if="noConnection" @retry="onRetryButtonClicked"/>
  <div id="mainDiv" v-else>
    <spinner />
    <main v-bind:class="bottomStyle()">
      <div data-purpose="no-slots-error" v-if="showNoAppointment">
        <p :class="$style.summary">{{$t('appointments.noSlotErrorMessage.summary')}}</p>
        <p :class="$style.info">{{$t('appointments.noSlotErrorMessage.info')}}</p>
      </div>
      <ul data-purpose="slots" v-if="showAppointments">
        <li :key="slot.id" v-for="slot in slots" :class="$style.slot">
          <appointment-slot :slotId="slot.id"/>
        </li>
      </ul>

      <floating-button-bottom :clickable="hasASlotSelected" @on-click="onBookButtonClicked">
        {{$t('appointments.bookAppointmentButtonText')}}
      </floating-button-bottom>
    </main>
  </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import AppointmentsNoConnection from '@/components/appointments/AppointmentsNoConnection';
import AppointmentSlot from '@/components/appointments/AppointmentSlot';
import Spinner from '@/components/Spinner';
import FloatingButtonBottom from '@/components/FloatingButtonBottom';

export default {
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
      return this.$store.state.appointmentSlots.hasLoaded
        && this.$store.state.appointmentSlots.slots.length === 0;
    },
    showAppointments() {
      return this.$store.state.appointmentSlots.hasLoaded
        && this.$store.state.appointmentSlots.slots.length > 0;
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
  methods: {
    bottomStyle() {
      if (this.$store.state.appointmentSlots.hasLoaded
        && this.$store.state.appointmentSlots.slots.length !== 0) {
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
  mounted() {
    this.$store.dispatch('appointmentSlots/load', this.$config);
    this.noConnection = !navigator.onLine;
  },
};
</script>

<style module lang="scss">
  @import '../../style/html';
  @import '../../style/fonts';
  @import '../../style/spacings';

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

