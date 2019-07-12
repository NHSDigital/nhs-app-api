<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <no-js-form :action="guidancePath" :value="formData">
          <button id="book-appointments-button"
                  class="nhsuk-button"
                  @click.stop.prevent="onBookButtonClicked">
            {{ $t('appointments.index.bookButtonText') }}
          </button>
        </no-js-form>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">

        <div v-if="showNoUpcomingAppointments"
             class="nhsuk-u-padding-bottom-6"
             data-purpose="upcoming-info">
          <h2>{{ $t('appointments.index.empty.header') }}</h2>
          <div >
            <p>
              {{ $t('appointments.index.empty.text1') }}</p>
          </div>
        </div>

        <upcoming-appointments v-if="showUpcomingAppointments"
                               :appointments="upcomingAppointments"
                               :cancellation-disabled="cancellationDisabled"/>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="showNoPastAppointments" data-purpose="past-info">
          <h2>{{ $t('appointments.index.emptyPast.header') }}</h2>
          <div>
            <p class="nhsuk-u-padding-bottom-6">
              {{ $t('appointments.index.emptyPast.text1') }}
            </p>
          </div>
        </div>

        <past-appointments v-if="showPastAppointments"
                           :appointments="pastAppointments"/>
      </div>
    </div>
  </div>
</template>

<script>
import PastAppointments from '@/components/appointments/PastAppointments';
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import NoJsForm from '@/components/no-js/NoJsForm';
import { APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',

  components: {
    PastAppointments,
    UpcomingAppointments,
    NoJsForm,
  },
  computed: {
    hasConnectionProblem() {
      return this.$store.state.errors.hasConnectionProblem;
    },
    showApiError() {
      return this.$store.getters['errors/showApiError'];
    },
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
  },
  asyncData({ store }) {
    store.dispatch('myAppointments/clear');
    return store.dispatch('myAppointments/load');
  },
  mounted() {
    if (this.$store.state.myAppointments.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearAppointments');
  },
  methods: {
    onBookButtonClicked() {
      this.$store.app.$analytics.trackButtonClick(this.guidancePath, true);
      redirectTo(this, this.guidancePath, null);
    },
  },
};
</script>
