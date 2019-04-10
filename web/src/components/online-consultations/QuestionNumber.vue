<!-- eslint-disable vue/no-v-html -->
<template>
  <generic-text-input :id="id"
                      v-model="numberValue"
                      :a-labelled-by="questionId"
                      :name="name"
                      :min="min"
                      :max="max"
                      :required="required"
                      :error="error"
                      :error-text="errorText"
                      step="any"
                      type="tel"/>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';

const integerPattern = '^-?\\d+$';
const decimalPattern = '^-?\\d+(\\.\\d+)?$';

export default {
  name: 'QuestionNumber',
  components: {
    GenericTextInput,
  },
  props: {
    questionId: {
      type: String,
      default: 'number-question',
    },
    id: {
      type: String,
      default: 'number-answer',
    },
    name: {
      type: String,
      default: 'number-answer',
    },
    max: {
      type: Number,
      default: undefined,
    },
    min: {
      type: Number,
      default: undefined,
    },
    value: {
      type: [Number, String],
      default: undefined,
    },
    type: {
      type: String,
      default: 'integer',
      validator: value => (['integer', 'decimal'].includes(value)),
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
    const isInteger = this.type === 'integer';

    return {
      isInteger,
      pattern: isInteger ? integerPattern : decimalPattern,
      regex: new RegExp(isInteger ? integerPattern : decimalPattern),
      isValid: true,
    };
  },
  computed: {
    numberValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.checkAndEmitIsValueValid(value);
        this.$emit('input', value);
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
      if (!this.required && !value) {
        return true;
      }
      const isANumber = !Number.isNaN(value);
      const lessThanOrEqualToMax = this.max === undefined || value <= this.max;
      const moreThanOrEqualToMin = this.min === undefined || value >= this.min;
      const matchesRegex = `${value}`.match(this.regex) !== null;

      return isANumber &&
             lessThanOrEqualToMax &&
             moreThanOrEqualToMin &&
             matchesRegex;
    },
  },
};
</script>
