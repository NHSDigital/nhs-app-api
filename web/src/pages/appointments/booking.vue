<template>
  <div v-if="showTemplate" class="pull-content">
    <form action="/appointments/booking" method="get">
      <ul :class="$style['sr-only']" role="presentation"
          aria-live="polite" aria-relevant="additions" aria-atomic="false">
        <li v-for="(text, index) in availableAppointmentsScreenReaderMessage" :key="index">
          <span :aria-hidden="availableAppointmentsScreenReaderMessage.length !== (index + 1)">
            {{ text }}
          </span>
        </li>
      </ul>

      <message-dialog v-if="noAvailableAppointments" message-type="warning">
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

      <filters
        v-if="availableAppointments"
        v-model="selectedOptions"
        :options="filtersOptions"
        :selected-options="defaultSelectedOptions"
        :guidance-msg="bookingGuidanceMsg"
      />

      <noscript>
        <button :class="this.$style.button">
        {{ $t('appointments.booking.nojs.findButton') }}
        </button>
      </noscript>

      <slot-list ref="slot_list" :available-slots="availableSlots" />

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

      <nuxt-link v-if="loadComplete"
                 :class="[$style.button, $style.grey]"
                 :to="backButtonPath" tag="button" >
        {{ $t('appointments.booking.backButtonText') }}
      </nuxt-link>
    </form>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import isEmpty from 'lodash/fp/isEmpty';
import qs from 'qs';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';
import VueScrollTo from 'vue-scrollto';
import { APPOINTMENTS } from '@/lib/routes';

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

export default {
  components: {
    MessageDialog,
    MessageText,
    MessageList,
    FloatingButtonBottom,
    Filters,
    SlotList,
    VueScrollTo,
  },
  data() {
    return {
      backButtonPath: APPOINTMENTS.path,
      availableAppointmentsScreenReaderMessage: [],
      filtered: false,
    };
  },
  computed: {
    selectedOptions: {
      get() { return this.$store.state.availableAppointments.selectedOptions; },
      set(val) {
        this.filterSlots(val);
      },
    },
    showNoMatchingWarning() {
      return this.filtered &&
             isEmpty(this.$store.state.availableAppointments.filteredSlots);
    },
    filtersOptions() {
      return this.$store.state.availableAppointments.filtersOptions;
    },
    bookingGuidanceMsg() {
      return this.$store.state.availableAppointments.bookingGuidance;
    },
    defaultSelectedOptions() {
      return this.$store.state.availableAppointments.selectedOptions;
    },
    availableSlots() {
      return this.$store.state.availableAppointments.filteredSlots;
    },
    noAvailableAppointments() {
      const hasNoSlots = this.$store.state.availableAppointments.slots.length === 0;
      return this.$store.state.availableAppointments.hasLoaded && hasNoSlots;
    },
    availableAppointments() {
      const hasSlots = this.$store.state.availableAppointments.slots.length > 0;
      return this.$store.state.availableAppointments.hasLoaded && hasSlots;
    },
    loadComplete() {
      return this.$store.state.availableAppointments.hasLoaded;
    },
    numberOfAvailableAppointments() {
      let count = 0;
      this.availableSlots.forEach((value) => {
        count += value[1].length;
      });
      return count;
    },
  },
  asyncData({ store, req }) {
    const query = req ? qs.parse(req.url.substr(req.url.indexOf('?') + 1)) : undefined;

    const result = store
      .dispatch('availableAppointments/init')
      .then(() => store.dispatch('availableAppointments/load'));

    if (containsFilter(query)) {
      result
        .then(() => store.dispatch('availableAppointments/setSelectedFilters', query))
        .then(() => store.dispatch('availableAppointments/filter'));
    }

    return result.then(() => ({ filtered: containsFilter(query) }));
  },
  beforeDestroy() {
    this.$store.dispatch('availableAppointments/clear');
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

      this.availableAppointmentsScreenReaderMessage.push(screenReaderMessage);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/accessibility";

div:focus {
  outline: none !important;
}

</style>
