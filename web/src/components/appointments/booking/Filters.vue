<template>
  <div :class="$style.form">
    <label for="type">{{ $t('appointments.booking.filters.type.label') }}</label>
    <error-message v-if="!validationError.isTypeValid">
      <span id="error-type">
        {{ $t('appointments.booking.validationErrors.type') }}
      </span>
    </error-message>
    <select-dropdown v-model="type" :error-border="!validationError.isTypeValid"
                     select-id = "type" select-name="type">
      <option v-for="option in options.types" :key="option.value" :value="option.value"
              :selected="type === option.value" :disabled="option.value===''">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label for="location">{{ $t('appointments.booking.filters.location.label') }}</label>
    <error-message v-if="!validationError.isLocationValid">
      <span id="error-location">
        {{ $t('appointments.booking.validationErrors.location') }}
      </span>
    </error-message>
    <select-dropdown v-model="location" :error-border="!validationError.isLocationValid"
                     select-id = "location" select-name="location">
      <option v-for="option in options.locations" :key="option.value" :value="option.value"
              :selected="location === option.value" :disabled="option.value===''">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label for="clinician">{{ $t('appointments.booking.filters.clinician.label') }}</label>
    <select-dropdown v-model="clinician" select-id = "clinician" select-name="clinician">
      <option v-for="option in options.clinicians" :key="option. value" :value="option.value"
              :selected="clinician === option.value">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label for="time-period">{{ $t('appointments.booking.filters.date.label') }}</label>
    <select-dropdown v-model="date" select-id = "time-period" select-name="time-period">
      <option v-for="option in options.dates" :key="option.value" :value="option.value"
              :selected="date === option.value">
        {{ displayName(option) }}
      </option>
    </select-dropdown>
    <hr aria-hidden="true">
  </div>
</template>


<script>
/* eslint-disable import/extensions */
import SelectDropdown from '@/components/widgets/SelectDropdown';
import ErrorMessage from '@/components/widgets/ErrorMessage';

export default {
  components: {
    SelectDropdown,
    ErrorMessage,
  },
  props: {
    options: {
      type: Object,
      default: () => ({
        types: [],
        locations: [],
        clinicians: [],
        dates: [],
      }),
    },
    selectedOptions: {
      type: Object,
      default: () => ({
        type: '',
        location: '',
        clinician: '',
        date: '',
      }),
    },
    validationError: {
      type: Object,
      default: () => ({
        isTypeValid: true,
        isLocationValid: true,
      }),
    },
  },
  computed: {
    type: {
      get() { return this.selectedOptions.type; },
      set(val) {
        this.selectedOptions.type = val;
        this.returnSelectedOptions();
      },
    },
    location: {
      get() { return this.selectedOptions.location; },
      set(val) {
        this.selectedOptions.location = val;
        this.returnSelectedOptions();
      },
    },
    clinician: {
      get() { return this.selectedOptions.clinician; },
      set(val) {
        this.selectedOptions.clinician = val;
        this.returnSelectedOptions();
      },
    },
    date: {
      get() { return this.selectedOptions.date; },
      set(val) {
        this.selectedOptions.date = val;
        this.returnSelectedOptions();
      },
    },
  },
  methods: {
    returnSelectedOptions() {
      this.$emit('input', this.selectedOptions);
    },
    displayName(option) {
      if (option.translate === true) {
        return this.$t(option.name);
      }
      return option.name;
    },
  },
};
</script>

<style module lang="scss">
@import "../../../style/textstyles";
@import "../../../style/fonts";
.form {
  margin-bottom: 24px;
  label {
    @include default_label;
    margin-top: 16px;
    margin-bottom: 8px;
  }
  hr {
    height: 1px;
    border: none;
    background-color: $dark_grey;
    opacity: 0.2;
    margin-bottom: 16px;
    margin-top: 16px;
  }
}

</style>
