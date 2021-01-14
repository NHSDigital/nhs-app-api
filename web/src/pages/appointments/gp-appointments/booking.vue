<template>
  <div v-if="showTemplate">
    <div v-if="error || (hasLoaded && !availableAppointments)">
      <booking-errors :error="error"
                      :available-appointments="availableAppointments"/>
    </div>
    <div v-else>
      <ul :class="$style['sr-only']" role="list"
          aria-live="polite" aria-relevant="additions" aria-atomic="false">
        <li v-for="(text, index) in availableAppointmentsScreenReaderMessage" :key="index">
          <span :aria-hidden="availableAppointmentsScreenReaderMessage.length !== (index + 1)">
            {{ text }}
          </span>
        </li>
      </ul>

      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">

          <filters
            v-if="availableAppointments"
            v-model="selectedOptions"
            :options="filtersOptions"
            :guidance-msg="bookingGuidanceMsg"/>

          <h2 v-if="showAvailableSlotsTitle" class="nhsuk-u-margin-bottom-0 break">
            {{ $t('appointments.book.availableAppointments') }}
          </h2>
          <slot-list ref="slot_list" :available-slots="availableSlots" />

          <div ref="noMatching" tabindex="-1">
            <div v-if="showNoMatchingWarning" data-purpose="no-appointments-matching-filter">
              <h2>{{ $t('appointments.book.noAppointmentsAvailableForYourSearch') }}</h2>
              <p>{{ $t('appointments.book.tryToFilterAppointments') }}</p>
              <p>{{ $t('appointments.book.ifYouCannotFindAppointment') }}</p>
              <contact-111
                :text="$t('appointments.book.forUrgentMedicalAdvice.text')"
                :aria-label="$t('appointments.book.forUrgentMedicalAdvice.label')"/>
            </div>
          </div>

          <desktop-generic-back-link
            v-if="hasLoaded && !$store.state.device.isNativeApp"
            :path="appointmentsPath"
            @clickAndPrevent="goBack"/>

        </div>
      </div>
    </div>
  </div>
</template>

<script>
import vueScrollTo from 'vue-scrollto';
import { get, isEmpty } from 'lodash/fp';
import BookingErrors from '@/components/errors/pages/appointments/BookingErrors';
import Contact111 from '@/components/widgets/Contact111';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';

import {
  GP_APPOINTMENTS_PATH,
  APPOINTMENT_BOOKING_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

const FILTER_PARAMETERS = [
  'clinician',
  'location',
  'time-period',
  'type',
];

const containsFilter = parameters => (!parameters
  ? false
  : Object.keys(parameters)
    .some(x => FILTER_PARAMETERS.includes(x)));

const load = async ({ $store }, query) => {
  $store.dispatch('availableAppointments/init');
  await $store.dispatch('availableAppointments/load');

  if (containsFilter(query)) {
    $store.dispatch('availableAppointments/setSelectedFilters', { ...query, date: query['time-period'] });
    $store.dispatch('availableAppointments/filter');
  }
};

export default {
  name: 'GpAppointmentsBookingPage',
  components: {
    BookingErrors,
    Contact111,
    DesktopGenericBackLink,
    Filters,
    SlotList,
  },
  data() {
    return {
      appointmentsPath: GP_APPOINTMENTS_PATH,
      availableAppointmentsScreenReaderMessage: [],
      bookingPath: APPOINTMENT_BOOKING_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      coronaServiceUrl: this.$store.$env.CORONA_SERVICE_URL,
      filtered: false,
    };
  },
  computed: {
    availableAppointments() {
      return !isEmpty(this.slots);
    },
    availableSlots() {
      return get('state.availableAppointments.filteredSlots')(this.$store);
    },
    bookingGuidanceMsg() {
      return get('state.availableAppointments.bookingGuidance')(this.$store);
    },
    error() {
      return this.$store.state.availableAppointments.error;
    },
    filtersOptions() {
      return get('state.availableAppointments.filtersOptions')(this.$store);
    },
    hasLoaded() {
      return get('state.availableAppointments.hasLoaded')(this.$store);
    },
    numberOfAvailableAppointments() {
      let count = 0;
      this.availableSlots.forEach((value) => {
        count += value[1].length;
      });
      return count;
    },
    selectedOptions: {
      get() {
        return get('state.availableAppointments.selectedOptions')(this.$store);
      },
      set(val) {
        this.filterSlots(val);
      },
    },
    showNoMatchingWarning() {
      return !!(this.filtered &&
             get('selectedOptions.location')(this) &&
             get('selectedOptions.type')(this) &&
             isEmpty(this.availableSlots));
    },
    showAvailableSlotsTitle() {
      return this.filtered &&
             get('selectedOptions.location')(this) &&
             get('selectedOptions.type')(this) &&
             !isEmpty(this.availableSlots);
    },
    slots() {
      return get('state.availableAppointments.slots')(this.$store);
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await load(this, this.$route.query);
      this.filtered = containsFilter(this.$route.query);
    },
  },
  async created() {
    const { query } = this.$route;

    await load({ $store: this.$store }, query);

    this.filtered = containsFilter(query);
  },
  methods: {
    goBack() {
      redirectTo(this, this.appointmentsPath);
    },
    filterSlots(val) {
      this.$store.dispatch('availableAppointments/setSelectedFilters', val);
      this.$store.dispatch('availableAppointments/filter');
      this.filtered = true;

      if (this.showNoMatchingWarning) {
        vueScrollTo.scrollTo(this.$refs.noMatching, 500, { easing: vueScrollTo['ease-in'] });
        const errors = [];
        errors.push(this.$t('appointments.book.noAppointmentsAvailable'));
        this.$store.app.$analytics.logicError(errors);
      }

      const screenReaderMessage = this.$tc(
        'appointments.book.countAppointmentsAvailable',
        this.numberOfAvailableAppointments,
        { appointmentsCount: this.numberOfAvailableAppointments },
      );
      this.trackDropDown(this.numberOfAvailableAppointments);
      this.availableAppointmentsScreenReaderMessage.push(screenReaderMessage);
    },
    trackDropDown(numberOfAvailableAppointments) {
      let trackEvent;
      if (numberOfAvailableAppointments === 0) {
        trackEvent = 'appointmentError';
      } else {
        trackEvent = 'appointmentSuccess';
      }

      const objectToTrack = {
        appointmentType: this.selectedOptions.type,
        location: this.selectedOptions.location,
        practiceMember: this.selectedOptions.clinician,
        filterByDate: this.selectedOptions.date,
      };

      this.$store.dispatch('analytics/satelliteTrack', trackEvent, objectToTrack);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/booking";
  @import "@/style/custom/break";
  @import "@/style/custom/inline-block-a";
</style>
