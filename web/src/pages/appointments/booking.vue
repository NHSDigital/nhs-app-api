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

    <div ref = "errors" tabindex="-1">
      <message-dialog v-show="showValidationError"
                      message-type="error" message-id="validationErrors">
        <message-text>
          {{ $t('appointments.booking.validationErrors.problemFound') }}
        </message-text>
        <message-list :is-error="true">
          <li v-if="!validationError.isTypeValid">
            {{ $t('appointments.booking.validationErrors.type') }}
          </li>
          <li v-if="!validationError.isLocationValid">
            {{ $t('appointments.booking.validationErrors.location') }}
          </li>
          <li> {{ $t('appointments.booking.validationErrors.slot') }}</li>
        </message-list>
      </message-dialog>
    </div>

    <filters
      v-if="availableAppointments"
      v-model="selectedOptions"
      :options="filtersOptions"
      :selected-options="defaultSelectedOptions"
      :validation-error="validationError"
      :guidance-msg="bookingGuidanceMsg"
    />

    <slot-list ref="slot_list" :available-slots="availableSlots"
               :show-validation-error="showValidationError"/>

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

    <button
      v-if="availableAppointments"
      :class="[$style.button, $style.green]"
      @click="onConfirmButtonClicked">
      {{ $t('appointments.booking.bookButtonText') }}
    </button>
    <nuxt-link v-if="loadComplete"
               :class="[$style.button, $style.grey]"
               :to="backButtonPath" tag="button" >
      {{ $t('appointments.booking.backButtonText') }}
    </nuxt-link>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';
import Routes from '@/Routes';

export default {
  components: {
    MessageDialog,
    MessageText,
    MessageList,
    FloatingButtonBottom,
    Filters,
    SlotList,
  },
  data() {
    return {
      backButtonPath: Routes.APPOINTMENTS.path,
      showValidationError: false,
      showNoMatchingWarning: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
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
      if (this.$store.state.availableAppointments.selectedSlot) {
        const childRefs = this.$refs.slot_list.$refs;
        childRefs[this.$store.state.availableAppointments.selectedSlot.ref][0].deselect();
      }
      this.$store.dispatch('availableAppointments/setSelectedFilters', this.filters);
      this.$store.dispatch('availableAppointments/filter');

      this.showNoMatchingWarning = this.shouldShowNoMatchingWarning();
      if (this.showNoMatchingWarning) {
        this.$refs.noMatching.focus();
      }

      this.showValidationError = false;
      this.validationError.isTypeValid = true;
      this.validationError.isLocationValid = true;

      const screenReaderMessage = this.$tc(
        'appointments.booking.availableAppointmentsScreenReaderMessage',
        this.numberOfAvailableAppointments,
        { appointmentsCount: this.numberOfAvailableAppointments },
      );

      this.availableAppointmentsScreenReaderMessage.push(screenReaderMessage);
    },
    onConfirmButtonClicked() {
      this.validate();

      if (this.showNoMatchingWarning) {
        this.$refs.noMatching.focus();
      } else if (this.showValidationError) {
        this.$refs.errors.focus();
      } else {
        this.$router.push(Routes.APPOINTMENT_CONFIRMATIONS);
      }
    },

    shouldShowNoMatchingWarning() {
      const filterMatchingSlotsCount = this.$store.state.availableAppointments.filteredSlots.length;
      return filterMatchingSlotsCount === 0 && this.filters
             && this.filters.type !== '' && this.filters.location !== '';
    },
    validate() {
      this.validationError.isTypeValid = this.filters ? this.filters.type !== '' : false;

      this.validationError.isLocationValid = true;
      if ((!this.filters && this.$store.state.availableAppointments.selectedOptions.location === '')
        || (this.filters && this.filters.location === '')) {
        this.validationError.isLocationValid = false;
      }

      this.showValidationError = !this.showNoMatchingWarning &&
        (!this.validationError.isTypeValid ||
        !this.validationError.isLocationValid ||
        this.$store.state.availableAppointments.selectedSlot === null);

      if (this.showValidationError) {
        const errors = [];
        if (!this.validationError.isTypeValid) {
          errors.push(this.$t('appointments.booking.validationErrors.type'));
        }
        if (!this.validationError.isLocationValid) {
          errors.push(this.$t('appointments.booking.validationErrors.location'));
        }
        errors.push(this.$t('appointments.booking.validationErrors.slot'));
        this.$store.app.$analytics.validationError(errors);
      }
    },
    bottomStyle() {
      if (
        this.$store.state.availableAppointments.hasLoaded &&
        this.$store.state.availableAppointments.slots.length !== 0
      ) {
        return this.$style.mainShowingSlots;
      }
      return this.$style.main;
    },
    onBookButtonClicked() {
      this.$router.push(Routes.APPOINTMENT_CONFIRMATIONS.path);
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
