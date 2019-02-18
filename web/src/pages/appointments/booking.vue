<template>
  <div v-if="showTemplate" class="pull-content">
    <form :action="bookingPath">
      <input :name="noJsInputName" :value="JSON.stringify(noJsData)" type="hidden">
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

      <noscript inline-template>
        <button :class="[this.$style.button, this.$style.desktopWeb]">
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
    </form>

    <form :action="appointmentsPath" method="get">
      <div :class="[isDesktopWeb ? $style.desktopBackButtonPadding : undefined]">
        <generic-button v-if="loadComplete"
                        id="back-to-appointments"
                        :class="[isDesktopWeb ? $style.desktopBackButton
                        : [$style.button, $style.grey]]"
                        @click.stop.prevent="goBack()">
          {{ $t(getBackButtonText) }}
        </generic-button>
      </div>
    </form>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import get from 'lodash/fp/get';
import isEmpty from 'lodash/fp/isEmpty';
import qs from 'qs';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericButton from '@/components/widgets/GenericButton';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';
import VueScrollTo from 'vue-scrollto';
import { APPOINTMENTS, APPOINTMENT_BOOKING } from '@/lib/routes';
import { noJsParameterName } from '@/lib/noJs';

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
  components: {
    MessageDialog,
    MessageText,
    MessageList,
    FloatingButtonBottom,
    Filters,
    SlotList,
    VueScrollTo,
    GenericButton,
  },
  data() {
    return {
      availableAppointmentsScreenReaderMessage: [],
      filtered: false,
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
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
    defaultSelectedOptions() {
      return getSelectedOptions(this.$store);
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
    getBackButtonText() {
      return (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios') ? 'appointments.booking.desktopBackButtonText'
        : 'appointments.booking.backButtonText';
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
    goBack() {
      this.$router.push(APPOINTMENTS.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/accessibility";
@import "../../style/textstyles";
@import "../../style/fonts";

div:focus {
  outline: none !important;
}
desktopBackButtonPadding {
 padding-left: 1em;
}
.desktopBackButton {
 font-family: $default-web;
 color: $nhs_blue;
 font-size: 1.125em;
 line-height: 1.125em;
 font-weight: normal;
 vertical-align: middle;
 cursor: pointer;
 display: inline-block;
 border: none;
 background: none;
 outline: none;
 text-decoration: underline;
 margin-top: 2em;
 margin-bottom: 2em;
}

.desktopBackButton:focus{
 box-sizing: content-box;
outline-color: $focus_highlight;
box-shadow: 0 0 0 4px $focus_highlight;
 outline-width: 2em;
}
.desktopBackButton:hover{
 background: #ffcd60;
 //box-shadow: 0 0 0 4px #ffcd60;
 outline: none;
 box-sizing: border-box;
 text-decoration: underline;
 background-clip: content-box;
}
</style>
