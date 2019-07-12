<template>
  <div :class="$style.form">
    <label class="nhsuk-label" for="type">
      {{ $t('appointments.booking.filters.type.label') }}
    </label>
    <div class="nhsuk-u-padding-bottom-3">
      <collapsible-dialog v-if="guidanceMsg" >
        <template slot="header">
          {{ $t('appointments.booking.gpMessage.header') }}
        </template>
        <p>{{ guidanceMsg }}</p>
      </collapsible-dialog>
    </div>

    <select-dropdown v-model="type" select-id="type" select-name="type">
      <option v-for="option in options.types"
              :key="option.value"
              :value="option.value"
              :disabled="option.value===''"
              :selected="option.value===''">
        {{ displayName(option) }}
      </option>
      <!-- empty optgroup tag forces ios to not cut off text in options -->
      <optgroup v-if="this.$store.state.device.source === 'ios'" label=""/>
    </select-dropdown>

    <label class="nhsuk-label" for="location">
      {{ $t('appointments.booking.filters.location.label') }}
    </label>
    <select-dropdown v-model="location" select-id="location" select-name="location">
      <option v-for="option in options.locations"
              :key="option.value"
              :value="option.value"
              :disabled="option.value===''">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label class="nhsuk-label" for="clinician">
      {{ $t('appointments.booking.filters.clinician.label') }}
    </label>
    <select-dropdown
      v-model="clinician"
      :required="false"
      select-id="clinician"
      select-name="clinician">
      <option v-for="option in options.clinicians" :key="option.value" :value="option.value">
        {{ displayName(option) }}
      </option>
    </select-dropdown>

    <label class="nhsuk-label" for="time-period">
      {{ $t('appointments.booking.filters.date.label') }}
    </label>
    <select-dropdown v-model="date" select-id="time-period" select-name="time-period">
      <option v-for="option in options.dates" :key="option.value" :value="option.value">
        {{ displayName(option) }}
      </option>
    </select-dropdown>
    <h2>{{ $t('appointments.booking.filters.date.header') }}</h2>
  </div>
</template>


<script>
import SelectDropdown from '@/components/widgets/SelectDropdown';
import CollapsibleDialog from '@/components/widgets/CollapsibleDialog';
import cloneDeep from 'lodash/fp/cloneDeep';

export default {
  name: 'Filters',
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
    value: {
      type: Object,
      default: () => ({
        type: '',
        location: '',
        clinician: '',
        date: '',
      }),
    },
  },
  data() {
    return {
      selectedOptions: cloneDeep(this.value),
    };
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
      const copyOptions = cloneDeep(this.selectedOptions);
      this.$emit('input', copyOptions);
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
 </style>
