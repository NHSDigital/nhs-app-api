<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <div v-if="showNoUpcomingAppointments" data-purpose="info">
      <h2>{{ $t('appointments.index.empty.header') }}</h2>
      <div :class="$style.info">
        <p>{{ $t('appointments.index.empty.text1') }}</p>
        <p>{{ $t('appointments.index.empty.text2') }} </p>
      </div>
    </div>

    <upcoming-appointments v-if="showUpcomingAppointments" :appointments = "upcomingAppointments" />

    <floating-button-bottom v-if="showBookAppointmentButton" @on-click="onBookButtonClicked">
      {{ $t('appointments.index.bookButtonText') }}
    </floating-button-bottom>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Routes from '@/Routes';

export default {
  components: {
    FloatingButtonBottom,
    UpcomingAppointments,
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
    showBookAppointmentButton() {
      return (
        this.$store.state.myAppointments.hasLoaded
      );
    },
    upcomingAppointments() {
      return this.$store.state.myAppointments.appointments;
    },
  },
  mounted() {
    this.$store.dispatch('myAppointments/clear');
    this.$store.dispatch('myAppointments/load');
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearAppointments');
  },
  methods: {
    onBookButtonClicked() {
      this.$store.app.$analytics.trackButtonClick(Routes.APPOINTMENT_BOOKING_GUIDANCE.path, true);
      this.$router.push(Routes.APPOINTMENT_BOOKING_GUIDANCE.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/info";

.content {
  padding-bottom : 5em;
}

</style>
