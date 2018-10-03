<template>
  <div v-if="showTemplate" class="pull-content">

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

    <generic-button v-if="loadComplete"
                    id="back-to-appointments"
                    :class="[$style.button, $style.grey]"
                    @click="$router.push(backButtonPath)">
      {{ $t('appointments.booking.backButtonText') }}
    </generic-button>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericButton from '@/components/widgets/GenericButton';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';
import VueScrollTo from 'vue-scrollto';
import { APPOINTMENTS } from '@/lib/routes';

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
      backButtonPath: APPOINTMENTS.path,
      showNoMatchingWarning: false,
      filters: null,
      availableAppointmentsScreenReaderMessage: [],
    };
  },
  computed: {
    selectedOptions: {
      get() { return this.filters; },
      set(val) {
        this.filters = val;
        this.filterSlots();
      },
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
  mounted() {
    this.$store.dispatch('availableAppointments/init');
    this.$store.dispatch('availableAppointments/load');
  },
  beforeDestroy() {
    this.$store.dispatch('availableAppointments/clear');
  },
  methods: {
    filterSlots() {
      this.$store.dispatch('availableAppointments/setSelectedFilters', this.filters);
      this.$store.dispatch('availableAppointments/filter');

      this.showNoMatchingWarning = this.shouldShowNoMatchingWarning();
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
    shouldShowNoMatchingWarning() {
      const filterMatchingSlotsCount = this.$store.state.availableAppointments.filteredSlots.length;
      return filterMatchingSlotsCount === 0 && this.filters
             && this.filters.type !== '' && this.filters.location !== '';
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
