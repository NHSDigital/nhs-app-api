<template>
  <div>
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <div class="nhsuk-input__item">
      <label :id="quantityLabelId"
             :for="quantityId"
             class="nhsuk-label nhsuk-input__label">Quantity</label>
      <input :id="quantityId"
             v-model.number="quantity"
             :class="['nhsuk-input nhsuk-input--width-2', error && 'nhsuk-input--error']"
             type="number"
             pattern="[0-9]+"
             :min="0"
             :max="maxValue">
    </div>

    <div class="nhsuk-input__item">
      <label :id="selectLabelId"
             :for="selectId"
             class="nhsuk-label nhsuk-input__label">Unit</label>
      <select-dropdown v-model.number="unit"
                       :select-id="selectId"
                       :select-name="selectId"
                       :error-border="error">
        <option v-for="option in units"
                :key="option.value"
                :value="option.value"
                :disabled="option.value === initialDropdownValue"
                :selected="option.value === ''">
          {{ option.value }}
        </option>
      </select-dropdown>
    </div>
  </div>
</template>

<script>
import SelectDropdown from '@/components/widgets/SelectDropdown';

export default {
  name: 'QuestionQuantity',
  components: {
    SelectDropdown,
  },
  props: {
    quantityLabelId: {
      type: String,
      required: true,
    },
    quantityId: {
      type: String,
      required: true,
    },
    selectLabelId: {
      type: String,
      required: true,
    },
    selectId: {
      type: String,
      required: true,
    },
    errorId: {
      type: String,
      required: true,
    },
    unitOptions: {
      type: Array,
      default() {
        return [];
      },
    },
    maxValue: {
      type: Number,
      default: undefined,
    },
    value: {
      type: Object,
      default() {
        return {
          unit: '',
          quantity: '',
        };
      },
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      isValid: true,
      initialDropdownValue: this.$t('onlineConsultations.questions.quantity.initialUnitDropdownValue'),
    };
  },
  computed: {
    units() {
      return [{ value: this.initialDropdownValue }, ...this.unitOptions];
    },
    unit: {
      get() {
        return this.value.unit;
      },
      set(unit) {
        this.value.unit = unit;
        this.checkAndEmitIsValueValid(this.value);
        this.$emit('input', {
          ...this.value,
          unit,
        });
      },
    },
    quantity: {
      get() {
        return this.value.quantity;
      },
      set(quantity) {
        this.value.quantity = quantity;
        this.checkAndEmitIsValueValid(this.value);
        this.$emit('input', {
          ...this.value,
          quantity,
        });
      },
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.isValid = this.isValidInput(value);
      this.$emit('validate', this.isValid);
    },
    isValidInput(value) {
      const unitEmpty = value.unit === '' || value.unit === undefined;
      const quantityEmpty = value.quantity === '' || value.quantity === undefined;

      if (!this.required && quantityEmpty) {
        return true;
      }

      const lessThanOrEqualToMax = this.maxValue === undefined || value.quantity <= this.maxValue;
      const greaterThanOrEqualToMinValue = value.quantity >= 0;
      return !unitEmpty && !quantityEmpty && lessThanOrEqualToMax && greaterThanOrEqualToMinValue;
    },
  },
};
</script>
