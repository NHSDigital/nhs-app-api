<template>
<div>
  <appointments-no-connection v-if="noConnection" @retry="onRetryButtonClicked"/>
  <div id="mainDiv" v-else>
    <spinner />
    <main :class="$style.main">
      <div data-purpose="no-slots-error" v-if="showNoAppointment">
        <p :class="$style.summary">{{$t('appointments.noSlotErrorMessage.summary')}}</p>
        <p :class="$style.info">{{$t('appointments.noSlotErrorMessage.info')}}</p>
      </div>
      <ul data-purpose="slots" v-if="showAppointments">
        <li :key="slot.id" v-for="slot in slots" :class="$style.slot">
          <appointment-slot :slotId="slot.id"/>
        </li>
      </ul>
    </main>
  </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import AppointmentsNoConnection from '@/components/appointments/AppointmentsNoConnection';
import AppointmentSlot from '@/components/appointments/AppointmentSlot';
import Spinner from '@/components/Spinner';


export default {
  components: {
    AppointmentsNoConnection,
    AppointmentSlot,
    Spinner,
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
    ...mapGetters({
      slots: 'appointmentSlots/slots',
    }),
  },
  methods: {
    onRetryButtonClicked() {
      this.noConnection = !navigator.onLine;
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

