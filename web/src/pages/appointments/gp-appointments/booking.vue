<template>
  <div v-if="showTemplate">
    <div v-if="error">
      <error-container v-if="error.status===403"
                       :id="generateErrorId()"
                       override-style="plain"
                       aria-live="polite">
        <error-title title="appointments.error.appointmentBookingUnavailable"/>
        <error-paragraph from="appointments.error.youAreNotCurrentlyAbleToBook" />
        <error-paragraph from="appointments.error.contactSurgeryOrOneOneOneForUrgentAdvice" />
        <error-header from="appointments.error.coronavirus.mightHave" />
        <error-paragraph from="appointments.error.coronavirus.stayAtHome" />
        <error-link :class="$style['inline-link']"
                    from="appointments.error.coronavirus.useOneOneOne"
                    :action="coronaServiceUrl"
                    data-purpose="corona-service"
                    target="_blank"/>
      </error-container>

      <error-container v-else-if="error.status===502 || error.status===500"
                       :id="generateErrorId()">
        <error-title title="appointments.error.thereIsAProblemLoading"/>
        <error-paragraph from="appointments.error.tryAgainOrContactUs"
                         :variable="error.serviceDeskReference"/>
        <error-paragraph from="appointments.error.ifTheProblemContinuesAndYouNeedToBook"/>
        <error-link from="generic.contactUsButton.text"
                    :action="contactUsUrl"
                    target="_blank"/>
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true"/>
      </error-container>

      <error-container v-else-if="error.status===504" :id="generateErrorId()">
        <error-title title="appointments.error.thereIsAProblemLoading"/>
        <error-paragraph from="appointments.error.tryAgainNowOrContactUs"
                         :variable="error.serviceDeskReference"/>
        <error-paragraph from="appointments.error.ifTheProblemContinuesAndYouNeedToBook"/>
        <error-button from="generic.tryAgainButton.text" @click="$router.go()" />
        <error-link from="generic.contactUsButton.text"
                    :action="contactUsUrl"
                    target="_blank" />
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true" />
      </error-container>
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
          <message-dialog v-if="noAvailableAppointments" message-type="warning">
            <message-text :is-header="true">
              {{ $t('appointments.book.noAppointmentsAvailable') }}
            </message-text>
            <message-text>
              {{ $t('appointments.book.thereAreCurrentlyNoAppointments') }}
            </message-text>
            <message-text>
              {{ $t('appointments.book.ifItIsUrgent') }}
            </message-text>
          </message-dialog>

          <filters
            v-if="availableAppointments"
            v-model="selectedOptions"
            :options="filtersOptions"
            :guidance-msg="bookingGuidanceMsg"/>

          <slot-list ref="slot_list" :available-slots="availableSlots" />

          <div ref="noMatching" tabindex="-1">
            <message-dialog v-if="showNoMatchingWarning"
                            :icon-text="$t('appointments.book.noAppointmentsAvailable')"
                            message-type="warning">
              <message-text>
                {{ $t('appointments.book.tryToFilterAppointments') }}
              </message-text>
              <message-text>
                {{ $t('appointments.book.ifItIsUrgent') }}
              </message-text>
            </message-dialog>
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
import VueScrollTo from 'vue-scrollto';
import { get, isEmpty } from 'lodash/fp';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorHeader from '@/components/errors/ErrorHeader';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import Filters from '@/components/appointments/booking/Filters';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
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
    DesktopGenericBackLink,
    ErrorButton,
    ErrorContainer,
    ErrorHeader,
    ErrorLink,
    ErrorParagraph,
    ErrorTitle,
    Filters,
    MessageDialog,
    MessageText,
    SlotList,
  },
  mixins: [ErrorPageMixin],
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
      return this.hasLoaded && !isEmpty(this.slots);
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
    noAvailableAppointments() {
      return this.hasLoaded && isEmpty(this.slots);
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
    generateErrorId() {
      return `error-dialog-${this.error.status}`;
    },
    goBack() {
      redirectTo(this, this.appointmentsPath);
    },
    filterSlots(val) {
      this.$store.dispatch('availableAppointments/setSelectedFilters', val);
      this.$store.dispatch('availableAppointments/filter');
      this.filtered = true;

      if (this.showNoMatchingWarning) {
        VueScrollTo.scrollTo(this.$refs.noMatching, 500, { easing: VueScrollTo['ease-in'] });
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
@import "../../../style/accessibility";

div:focus {
  outline: none !important;
}

.inline-link > a {
  display: inline-block;
}
</style>
