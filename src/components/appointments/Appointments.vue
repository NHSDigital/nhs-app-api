<template>
<div>
  <appointments-no-connection v-if="noConnection" @retry="onRetryButtonClicked"/>
  <div id="mainDiv" v-else>
    <spinner />
    <main class="content">
      <div data-purpose="no-slots-error" v-if="showNoAppointment">
        <p class="summary">{{$t('appointments.noSlotErrorMessage.summary')}}</p>
        <p class="info">{{$t('appointments.noSlotErrorMessage.info')}}</p>
      </div>
      <ul data-purpose="slots" v-if="showAppointments">
        <li :key="slot.id" v-for="slot in slots">
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

<style scoped="true">
  li {
    list-style: none;
    margin-bottom: 18px;
  }

  .content {
    margin-right: 18px;
  }

  [data-purpose="no-slots-error"] .summary {
    font-weight: bold;
    margin-bottom: 18px;
  }
</style>

<style lang="scss">
  @import '../../style/html';
  @import '../../style/elements';

</style>
