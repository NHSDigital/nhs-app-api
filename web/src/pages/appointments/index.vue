<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content',
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div v-if="showNoUpcomingAppointments" data-purpose="upcoming-info">
      <h2>{{ $t('appointments.index.empty.header') }}</h2>
      <div :class="$style.upComingAppointments">
        <p :class="$style.upComingAppointments">
          {{ $t('appointments.index.empty.text1') }}</p>
        <p :class="$style.upComingAppointments">
          {{ $t('appointments.index.empty.text2') }} </p>
        <p :class="$style.upComingAppointments">
          {{ $t('appointments.index.empty.text3') }} </p>
      </div>
    </div>

    <upcoming-appointments v-if="showUpcomingAppointments"
                           :appointments = "upcomingAppointments"
                           :cancellation-disabled = "cancellationDisabled"
                           :class="$style.upcomingAppointmentContainer"/>

    <div v-if="showNoPastAppointments" data-purpose="past-info">
      <h2>{{ $t('appointments.index.emptyPast.header') }}</h2>
      <div :class="$style.info">
        <p>{{ $t('appointments.index.emptyPast.text1') }}</p>
      </div>
    </div>

    <past-appointments v-if="showPastAppointments"
                       :appointments = "pastAppointments" />

    <no-js-form v-if="!shouldShowDesktopVersion" :action="guidancePath" :value="formData">
      <floating-button-bottom v-if="showBookAppointmentButton"
                              id="book-appointments-button"
                              @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.index.bookButtonText') }}
      </floating-button-bottom>
    </no-js-form>
  </div>
</template>

<script>
import PastAppointments from '@/components/appointments/PastAppointments';
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import NoJsForm from '@/components/no-js/NoJsForm';
import { APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';

export default {
  components: {
    FloatingButtonBottom,
    PastAppointments,
    UpcomingAppointments,
    NoJsForm,
  },
  computed: {
    showNoUpcomingAppointments() {
      return (
        this.$store.state.myAppointments.hasLoaded &&
        this.$store.state.myAppointments.upcomingAppointments.length === 0
      );
    },
    showNoPastAppointments() {
      return (
        this.$store.state.myAppointments.hasLoaded &&
        this.$store.state.myAppointments.pastAppointments.length === 0 &&
        this.$store.state.myAppointments.pastAppointmentsEnabled
      );
    },
    showUpcomingAppointments() {
      return (
        this.$store.state.myAppointments.hasLoaded &&
        this.$store.state.myAppointments.upcomingAppointments.length > 0
      );
    },
    showPastAppointments() {
      return (
        this.$store.state.myAppointments.hasLoaded &&
        this.$store.state.myAppointments.pastAppointments.length > 0 &&
        this.$store.state.myAppointments.pastAppointmentsEnabled
      );
    },
    showBookAppointmentButton() {
      return (
        this.$store.state.myAppointments.hasLoaded
      );
    },
    upcomingAppointments() {
      return this.$store.state.myAppointments.upcomingAppointments;
    },
    pastAppointments() {
      return this.$store.state.myAppointments.pastAppointments;
    },
    cancellationDisabled() {
      return this.$store.state.myAppointments.disableCancellation;
    },
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
    guidancePath() {
      return APPOINTMENT_BOOKING_GUIDANCE.path;
    },
    shouldShowDesktopVersion() {
      return (this.$store.state.device.source !== 'android' && this.$store.state.device.source !== 'ios');
    },
  },
  asyncData({ store }) {
    store.dispatch('myAppointments/clear');
    return store.dispatch('myAppointments/load');
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearAppointments');
  },
  methods: {
    onBookButtonClicked() {
      this.$store.app.$analytics.trackButtonClick(this.guidancePath, true);
      this.$router.push(this.guidancePath);
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

div {
 &.desktopWeb {
  .content {
   max-width: 540px;
   font-family: $default-web;
  }
  p {
   font-family: $default-web;
   font-weight: lighter;
   max-width: 540px;
  }
 }
}


</style>
