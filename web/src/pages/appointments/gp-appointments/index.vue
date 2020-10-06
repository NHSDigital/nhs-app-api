<template>
  <div v-if="showTemplate">
    <div v-if="error && hasLoaded">
      <gp-appointment-errors :error="error"/>
    </div>
    <div v-else-if="hasLoaded">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <corona-virus-message />
          <generic-button id="book-appointments-button"
                          :button-classes="['nhsuk-button',
                                            'nhsuk-u-margin-bottom-3',
                                            'nhsuk-u-margin-top-3']"
                          @click="onBookButtonClicked">
            {{ $t('appointments.bookAnAppointment') }}
          </generic-button>

          <div v-if="showNoUpcomingAppointments"
               class="nhsuk-u-margin-bottom-3"
               data-purpose="upcoming-info">
            <h2 class="nhsuk-u-margin-bottom-0">{{
              $t('appointments.upcoming.upcomingAppointments')
            }}</h2>
            <p class="nhsuk-u-margin-top-0">
              {{ $t('appointments.upcoming.none.ifYouHaveAnAppointment') }}</p>
          </div>
          <upcoming-appointments v-if="showUpcomingAppointments"
                                 :appointments="upcomingAppointments"
                                 :cancellation-disabled="cancellationDisabled"/>

          <div v-if="showNoPastAppointments" data-purpose="past-info">
            <h2 class="nhsuk-u-margin-bottom-0">{{ $t('appointments.past.pastAppointments') }}</h2>
            <div>
              <p class="nhsuk-u-padding-bottom-3">
                {{ $t('appointments.past.none.youHaveNoRecentPastAppointments') }}
              </p>
            </div>
          </div>
          <past-appointments v-if="showPastAppointments"
                             :appointments="pastAppointments"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';

import CoronaVirusMessage from '@/components/widgets/CoronaVirusMessage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import GenericButton from '@/components/widgets/GenericButton';
import GpAppointmentErrors from '@/components/errors/pages/appointments/GpAppointmentErrors';
import PastAppointments from '@/components/appointments/PastAppointments';
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';

import showShutterPage from '@/lib/proxy/shutter';
import {
  APPOINTMENTS_PATH,
  APPOINTMENT_BOOKING_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

const loadData = async (store) => {
  store.dispatch('myAppointments/clear');
  await store.dispatch('myAppointments/load');
};

export default {
  name: 'GpAppointmentsIndexPage',
  components: {
    GpAppointmentErrors,
    CoronaVirusMessage,
    GenericButton,
    PastAppointments,
    UpcomingAppointments,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      backUrl: APPOINTMENTS_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      coronaServiceUrl: this.$store.$env.CORONA_SERVICE_URL,
      bookingUrl: APPOINTMENT_BOOKING_PATH,
    };
  },
  computed: {
    cancellationDisabled() {
      return this.$store.state.myAppointments.disableCancellation;
    },
    error() {
      return this.$store.state.myAppointments.error;
    },
    hasLoaded() {
      return this.$store.state.myAppointments.hasLoaded;
    },
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    pastAppointments() {
      return this.$store.state.myAppointments.pastAppointments;
    },
    showNoPastAppointments() {
      return this.hasLoaded &&
          this.$store.state.myAppointments.pastAppointmentsEnabled &&
          isEmpty(this.pastAppointments);
    },
    showNoUpcomingAppointments() {
      return this.hasLoaded && isEmpty(this.upcomingAppointments);
    },
    showPastAppointments() {
      return this.hasLoaded &&
          this.$store.state.myAppointments.pastAppointmentsEnabled &&
          !isEmpty(this.pastAppointments);
    },
    showUpcomingAppointments() {
      return this.hasLoaded && !isEmpty(this.upcomingAppointments);
    },
    upcomingAppointments() {
      return this.$store.state.myAppointments.upcomingAppointments;
    },
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      loadData(this.$store);
    },
    error(value) {
      if (value) {
        showShutterPage(this.$router.currentRoute, this);
      }
    },
    hasLoaded() {
      if (this.hasLoaded) {
        this.$store.dispatch('flashMessage/show');
      }
    },
  },
  async mounted() {
    await loadData(this.$store);

    if (this.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }

    if (this.$route.query.hr) {
      this.$store.dispatch('session/setRetry', true);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearAppointments');
  },
  methods: {
    onBookButtonClicked() {
      this.$store.app.$analytics.trackButtonClick(this.bookingUrl, true);
      redirectTo(this, this.bookingUrl);
    },
  },
};
</script>

<style module lang="scss" scoped>
.inline-link > a {
  display: inline-block;
}
</style>
