<template>
  <main v-if="showTemplate" :class="bottomStyle()">
    <error-warning-dialog v-if="noAvailableAppointments" error-or-warning="warning">
      <p>
        {{ $t('appointments.booking.noAppointmentsAvailable.line1') }}
      </p>
      <p>
        {{ $t('appointments.booking.noAppointmentsAvailable.line2') }}
      </p>
    </error-warning-dialog>

    <error-warning-dialog v-if="notMatchSearchCriteria" error-or-warning="warning">
      <p>
        {{ $t('appointments.booking.adjustSearch.line1') }}
      </p>
      <p>
        {{ $t('appointments.booking.adjustSearch.line2') }}
      </p>
    </error-warning-dialog>

    <div ref = "errors" tabindex="-1">
      <error-warning-dialog v-show="showValidationError"
                            error-or-warning="error" error-warning-id="validationErrors">
        <p> {{ $t('appointments.booking.validationErrors.problemFound') }} </p>
        <p v-if="!validationError.isTypeValid">
          - {{ $t('appointments.booking.validationErrors.type') }}
        </p>
        <p v-if="!validationError.isLocationValid">
          - {{ $t('appointments.booking.validationErrors.location') }}
        </p>
        <p>- {{ $t('appointments.booking.validationErrors.slot') }}</p>
      </error-warning-dialog>
    </div>

    <filters
      v-if="availableAppointments"
      v-model="selectedOptions"
      :options="filtersOptions"
      :selected-options="defaultSelectedOptions"
      :validation-error="validationError"
    />

    <slot-list :available-slots="availableSlots"/>

    <button
      v-if="availableAppointments"
      :class="[$style.button, $style.green]"
      @click="onConfirmButtonClicked">
      {{ $t('appointments.booking.bookButtonText') }}
    </button>
    <nuxt-link :class="[$style.button, $style.grey]"
               :to="backButtonPath" tag="button" >
      {{ $t('appointments.booking.backButtonText') }}
    </nuxt-link>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import Filters from '@/components/appointments/booking/Filters';
import SlotList from '@/components/appointments/booking/SlotList';
import Routes from '../../Routes';

export default {
  components: {
    ErrorWarningDialog,
    FloatingButtonBottom,
    Filters,
    SlotList,
  },
  data() {
    return {
      backButtonPath: Routes.APPOINTMENTS.path,
      showValidationError: false,
      validationError: {
        isTypeValid: true,
        isLocationValid: true,
      },
      filters: null,
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
    defaultSelectedOptions() {
      return this.$store.state.availableAppointments.selectedOptions;
    },
    availableSlots() {
      return this.$store.state.availableAppointments.filteredSlots;
    },
    noAvailableAppointments() {
      const hasNoSlots = this.$store.state.availableAppointments.slots.size === 0;
      return this.$store.state.availableAppointments.hasLoaded && hasNoSlots;
    },
    availableAppointments() {
      const hasSlots = this.$store.state.availableAppointments.slots.size > 0;
      return this.$store.state.availableAppointments.hasLoaded && hasSlots;
    },
    notMatchSearchCriteria() {
      let notMatch = false;
      if (this.$store.state.availableAppointments.filteredSlots.length === 0
        && this.filters
        && this.filters.type !== ''
        && this.filters.location !== '') {
        notMatch = true;
      }

      return notMatch;
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
      this.$store.dispatch('availableAppointments/deselect');
      this.$store.dispatch('availableAppointments/setSelectedFilters', this.filters);
      this.$store.dispatch('availableAppointments/filter');
    },
    onConfirmButtonClicked() {
      this.validate();

      if (this.showValidationError) {
        this.$refs.errors.focus();
      } else {
        this.$router.push(Routes.APPOINTMENT_CONFIRMATIONS);
      }
    },
    validate() {
      this.validationError.isTypeValid = this.filters ? this.filters.type !== '' : false;

      this.validationError.isLocationValid = true;
      if ((!this.filters && this.$store.state.availableAppointments.selectedOptions.location === '')
        || (this.filters && this.filters.location === '')) {
        this.validationError.isLocationValid = false;
      }

      this.showValidationError = !this.validationError.isTypeValid ||
        !this.validationError.isLocationValid ||
        this.$store.state.availableAppointments.selectedSlot === null;
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

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/spacings";
  @import "../../style/buttons";
  @import "../../style/elements";

  .main {
    @include space(padding, all, $three);

    &.error {
      border: 3px $error solid;
    }

    .form {
      margin-bottom: 24px;
      label {
        @include default_label;
        padding-top: 16px;
        padding-bottom: 8px;
      }
    }

    .info p {
      display: block;
      font-weight: normal;
      font-size: 1em;
      line-height: 1.5em;
      color: #4A4A4A;
      font-size: 1em;
      margin-bottom: 1em;
    }
  }

  div:focus {
    outline: none !important;
  }

  .mainShowingSlots {
    @include space(padding, all, $three);
    padding-bottom: 78px;
  }

  .summary {
    font-weight: bold;
    @include space(margin, bottom, $three);
  }

  .info {
    @include default_text;
    font-size: 12pt;
    @include space(margin, bottom, $three);
  }

  .slot {
    list-style: none;
    @include space(margin, bottom, $three);
  }

  .summary {
    font-weight: bold;
    @include space(margin, bottom, $three);
  }

</style>
