<template>
  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">
    <div v-if="showNoUpcomingAppointments" data-purpose="info">
      <h2>{{ $t('appointments.index.empty.header') }}</h2>
      <div :class="$style.info">
        <p>{{ $t('appointments.index.empty.text1') }}</p>
        <p>{{ $t('appointments.index.empty.text2') }} </p>
      </div>
    </div>

    <upcoming-appointments v-if="showUpcomingAppointments"
                           :appointments = "upcomingAppointments"
                           :cancellation-disabled = "cancellationDisabled" />

    <no-js-form :action="guidancePath" :value="formData">
      <floating-button-bottom v-if="showBookAppointmentButton"
                              id="book-appointments-button"
                              @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.index.bookButtonText') }}
      </floating-button-bottom>
    </no-js-form>
  </div>
</template>

<script>
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import NoJsForm from '@/components/no-js/NoJsForm';
import { APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';

export default {
  components: {
    FloatingButtonBottom,
    UpcomingAppointments,
    NoJsForm,
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

</style>
