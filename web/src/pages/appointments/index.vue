<template>
  <main v-if="showTemplate" :class="$style.main">
    <div v-if="showNoUpcomingAppointments" :class="$style.info">
      <h2>{{ $t('appointments.index.empty.header') }}</h2>
      <p>{{ $t('appointments.index.empty.text1') }}</p>
      <p>{{ $t('appointments.index.empty.text2') }} </p>
    </div>

    <upcoming-appointments v-if="showUpcomingAppointments" :appointments = "upcomingAppointments" />

    <floating-button-bottom v-if="showBookAppointmentButton" @on-click="onBookButtonClicked">
      {{ $t('appointments.index.bookButtonText') }}
    </floating-button-bottom>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Routes from '../../Routes';

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
      this.$router.push(Routes.APPOINTMENT_BOOKING_GUIDANCE.path);
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
      h2 {
        @include h5;
        font-size: 13pt;
        color: $dark_grey;
        margin-bottom: 16px;
      }
      margin-bottom: 70px;
    }

    a:link, a:visited {
      color: $red;
      display: inline-block;
    }
  }

</style>
