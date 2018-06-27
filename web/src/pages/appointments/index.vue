<template>
  <main v-if="showTemplate" :class="$style.main">
    <success-dialog v-if="justBookedAnAppointment">
      <p>
        {{ $t('appointments.index.successText') }}
      </p>
    </success-dialog>

    <success-dialog v-if="justCancelledAnAppointment">
      <p>
        {{ $t('appointments.cancel.successText') }}
      </p>
    </success-dialog>

    <div v-if="showNoUpcomingAppointments" :class="$style.info">
      <h3>{{ $t('appointments.index.empty.header') }}</h3>
      <p>{{ $t('appointments.index.empty.text1') }}</p>
      <p>{{ $t('appointments.index.empty.text2') }} </p>
    </div>

    <upcoming-appointments v-if="showUpcomingAppointments" :appointments = "upcomingAppointments" />

    <floating-button-bottom @on-click="onBookButtonClicked">
      {{ $t('appointments.index.bookButtonText') }}
    </floating-button-bottom>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import SuccessDialog from '@/components/widgets/SuccessDialog';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';

export default {
  middleware: ['auth', 'meta'],
  components: {
    SuccessDialog,
    FloatingButtonBottom,
    UpcomingAppointments,
  },
  data() {
    return {
      justBookedAnAppointment: false,
      justCancelledAnAppointment: false,
    };
  },
  computed: {
    showNoUpcomingAppointments() {
      return (
        this.$store.state.myAppointments.hasLoaded &&
        this.$store.state.myAppointments.appointments.length === 0
      );
    },
    showUpcomingAppointments() {
      return (
        this.$store.state.myAppointments.hasLoaded &&
        this.$store.state.myAppointments.appointments.length > 0
      );
    },
    upcomingAppointments() {
      return this.$store.state.myAppointments.appointments;
    },
  },
  mounted() {
    this.justBookedAnAppointment = this.$store.state.appointment.justBookedAnAppointment;
    this.justCancelledAnAppointment = this.$store.state.myAppointments.justCancelledAnAppointment;
    this.$store.dispatch('appointment/resetJustBooked');
    this.$store.dispatch('myAppointments/clear');
    this.$store.dispatch('myAppointments/load');
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearAppointments');
  },
  methods: {
    onBookButtonClicked() {
      this.$router.push('/appointments/booking-guidance');
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/spacings";
  @import "../../style/textstyles";
  @import "../../style/colours";

  .main {
    @include space(padding, all, $three);

    .info {
      p {
        @include default_text;
        font-size: 12pt;
        display: block;
        margin-bottom: 16px;
      }
      h3 {
        @include h5;
        font-size: 13pt;
        color: $dark_grey;
        margin-bottom: 16px;
      }
    }

    a:link, a:visited {
      color: $red;
      display: inline-block;
    }
  }

</style>
