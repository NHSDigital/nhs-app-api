<template>
  <div :class="$style.form">
    <label for="type">{{ $t('appointments.booking.filters.type.label') }}</label>
    <collapsible-dialog v-if="guidanceMsg !==''">
      <template slot="header">
        {{ $t('appointments.booking.gpMessage.header') }}
      </template>
      <p>{{ guidanceMsg }}</p>
    </collapsible-dialog>
    <select-dropdown v-model="type" select-id="type" select-name="type">
      <option v-for="option in options.types"
              :key="option.value"
              :value="option.value"
              :disabled="option.value===''"
              :selected="option.value===''">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label for="location">{{ $t('appointments.booking.filters.location.label') }}</label>
    <select-dropdown v-model="location" select-id = "location" select-name="location">
      <option v-for="option in options.locations"
              :key="option.value"
              :value="option.value"
              :disabled="option.value===''">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label for="clinician">{{ $t('appointments.booking.filters.clinician.label') }}</label>
    <select-dropdown
      v-model="clinician"
      :required="false"
      select-id = "clinician"
      select-name="clinician">
      <option v-for="option in options.clinicians" :key="option.value" :value="option.value">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <hr :class="$style.line" aria-hidden="true">
    <h2>{{ $t('appointments.booking.filters.date.header') }}</h2>
    <label for="time-period">{{ $t('appointments.booking.filters.date.label') }}</label>
    <select-dropdown v-model="date" select-id = "time-period" select-name="time-period">
      <option v-for="option in options.dates" :key="option.value" :value="option.value">
        {{ displayName(option) }}
      </option>
    </select-dropdown>
  </div>
</template>


<script>
import SelectDropdown from '@/components/widgets/SelectDropdown';
import CollapsibleDialog from '@/components/widgets/CollapsibleDialog';

export default {
  components: {
    SelectDropdown,
    CollapsibleDialog,
  },
  props: {
    guidanceMsg: {
      type: String,
      default: '',
    },
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

<style module lang="scss" scoped>
@import "../../../style/forms";
@import "../../../style/colours";

.line {
  margin-top: 0.5em
}

.form {
  :focus {
    outline-color: $focus_highlight;
  }
}
</style>
