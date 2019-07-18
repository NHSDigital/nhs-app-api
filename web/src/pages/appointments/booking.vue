<template>
  <div v-if="showTemplate">
    <form :action="bookingPath">
      <input :name="noJsInputName" :value="JSON.stringify(noJsData)" type="hidden">
      <ul :class="$style['sr-only']" role="list"
          aria-live="polite" aria-relevant="additions" aria-atomic="false">
        <li v-for="(text, index) in availableAppointmentsScreenReaderMessage" :key="index">
          <span :aria-hidden="availableAppointmentsScreenReaderMessage.length !== (index + 1)">
            {{ text }}
          </span>
        </li>
      </ul>

      <div v-if="noAvailableAppointments" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <message-dialog message-type="warning">
            <message-text :is-header="true">
              {{ $t('appointments.booking.noAppointmentsAvailable.title') }}
            </message-text>
            <message-text>
              {{ $t('appointments.booking.noAppointmentsAvailable.line1') }}
            </message-text>
            <message-text>
              {{ $t('appointments.booking.noAppointmentsAvailable.line2') }}
            </message-text>
          </message-dialog>
        </div>
      </div>

      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <filters
            v-if="availableAppointments"
            v-model="selectedOptions"
            :options="filtersOptions"
            :guidance-msg="bookingGuidanceMsg"/>
        </div>
      </div>

      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <noscript inline-template>
            <button class="nhsuk-button">
            {{ $t('appointments.booking.nojs.findButton') }}
            </button>
          </noscript>
        </div>
      </div>

      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <slot-list ref="slot_list" :available-slots="availableSlots" />
        </div>
      </div>
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <div ref="noMatching" tabindex="-1">
            <message-dialog v-if="showNoMatchingWarning"
                            :icon-text="$t('appointments.booking.adjustSearch.title')"
                            message-type="warning">
              <message-text>
                {{ $t('appointments.booking.adjustSearch.line1') }}
              </message-text>
              <message-text>
                {{ $t('appointments.booking.adjustSearch.line2') }}
              </message-text>
            </message-dialog>
          </div>
        </div>
      </div>
    </form>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">

        <desktopGenericBackLink
          v-if="loadComplete && !$store.state.device.isNativeApp"
          :path="appointmentsPath"
          :button-text="'appointments.booking.desktopBackButtonText'"
          @clickAndPrevent="goBack"/>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { APPOINTMENT_BOOKING, APPOINTMENTS } from '@/lib/routes';
import { noJsParameterName } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';
import get from 'lodash/fp/get';
import isEmpty from 'lodash/fp/isEmpty';
import qs from 'qs';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';
import VueScrollTo from 'vue-scrollto';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';

const FILTER_PARAMETERS = [
  'clinician',
  'location',
  'time-period',
  'type',
];

const containsFilter = parameters => (!parameters
  ? false
  : Object
    .keys(parameters)
    .some(x => FILTER_PARAMETERS.includes(x)));

const getBookingGuidance = get('state.availableAppointments.bookingGuidance');
const getFilteredSlots = get('state.availableAppointments.filteredSlots');
const getFiltersOptions = get('state.availableAppointments.filtersOptions');
const getSelectedOptions = get('state.availableAppointments.selectedOptions');
const getSlots = get('state.availableAppointments.slots');
const hasLoaded = get('state.availableAppointments.hasLoaded');

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    MessageDialog,
    MessageText,
    Filters,
    SlotList,
  },
  data() {
    return {
      availableAppointmentsScreenReaderMessage: [],
      filtered: false,
    };
  },
  computed: {
    selectedOptions: {
      get() {
        return getSelectedOptions(this.$store);
      },
      set(val) {
        this.filterSlots(val);
      },
    },
    showNoMatchingWarning() {
      return !!(this.filtered &&
             get('selectedOptions.location')(this) &&
             get('selectedOptions.type')(this) &&
             isEmpty(getFilteredSlots(this.$store)));
    },
    filtersOptions() {
      return getFiltersOptions(this.$store);
    },
    bookingGuidanceMsg() {
      return getBookingGuidance(this.$store);
    },
    availableSlots() {
      return getFilteredSlots(this.$store);
    },
    noAvailableAppointments() {
      const hasNoSlots = isEmpty(getSlots(this.$store));
      return hasLoaded(this.$store) && hasNoSlots;
    },
    availableAppointments() {
      const hasSlots = !isEmpty(getSlots(this.$store));
      return hasLoaded(this.$store) && hasSlots;
    },
    loadComplete() {
      return hasLoaded(this.$store);
    },
    numberOfAvailableAppointments() {
      let count = 0;
      this.availableSlots.forEach((value) => {
        count += value[1].length;
      });
      return count;
    },
    bookingPath() {
      return APPOINTMENT_BOOKING.path;
    },
    noJsData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
    noJsInputName() {
      return noJsParameterName;
    },
  },
  asyncData({ store, req }) {
    const query = req ? qs.parse(req.url.substr(req.url.indexOf('?') + 1)) : undefined;

    const result = store
      .dispatch('availableAppointments/init')
      .then(() => store.dispatch('availableAppointments/load'));

    if (containsFilter(query)) {
      query.date = query['time-period'];
      result
        .then(() => store.dispatch('availableAppointments/setSelectedFilters', query))
        .then(() => store.dispatch('availableAppointments/filter'));
    }

    return result.then(() => ({
      appointmentsPath: APPOINTMENTS.path,
      filtered: containsFilter(query),
    }));
  },
  methods: {
    filterSlots(val) {
      this.$store.dispatch('availableAppointments/setSelectedFilters', val);
      this.$store.dispatch('availableAppointments/filter');
      this.filtered = true;

      if (this.showNoMatchingWarning) {
        VueScrollTo.scrollTo(this.$refs.noMatching, 500, { easing: VueScrollTo['ease-in'] });
        const errors = [];
        errors.push(this.$t('appointments.booking.adjustSearch.title'));
        this.$store.app.$analytics.logicError(errors);
      }

      const screenReaderMessage = this.$tc(
        'appointments.booking.availableAppointmentsScreenReaderMessage',
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
    goBack() {
      redirectTo(this, APPOINTMENTS.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/accessibility";

div:focus {
  outline: none !important;
}

</style>
