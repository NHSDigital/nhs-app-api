<template>
  <div id="mainDiv">
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
</template>

<script>
import { mapGetters } from 'vuex';
import AppointmentSlot from '@/components/AppointmentSlot';
import Spinner from '@/components/Spinner';


export default {
  components: {
    AppointmentSlot,
    Spinner,
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
  mounted() {
    this.$store.dispatch('appointmentSlots/load', this.$config);
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
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';
  @import '../style/buttons';

</style>
