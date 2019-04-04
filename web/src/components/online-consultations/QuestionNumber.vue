<!-- eslint-disable vue/no-v-html -->
<template>
  <div :class="formGroupClasses">
    <label :id="questionId"
           :for="id"
           class="question nhsuk-label"
           v-html="text"/>
    <generic-text-input :id="id"
                        v-model.number="numberValue"
                        :a-labelled-by="questionId"
                        :name="name"
                        :min="min"
                        :max="max"
                        :required="true"
                        :error="showError"
                        :error-text="errorText"
                        step="any"
                        type="number"/>
  </div>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';

const integerPattern = '^-?\\d+$';
const decimalPattern = '^-?\\d+(\\.\\d+)?$';

export default {
  components: {
    GenericTextInput,
  },
  props: {
    questionId: {
      type: String,
      default: 'number-question',
    },
    text: {
      type: String,
      required: true,
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
      validator: value => (['integer', 'decimal'].indexOf(value) !== -1),
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: String,
      default: undefined,
    },
  },
  data() {
    const isInteger = this.type === 'integer';
    return {
      isInteger,
      hasErrored: this.error,
      isValid: this.isValidInput(this.value),
      pattern: isInteger ? integerPattern : decimalPattern,
      regex: new RegExp(isInteger ? integerPattern : decimalPattern),
    };
  },
  computed: {
    formGroupClasses() {
      return [
        'nhsuk-form-group',
        this.showError ? 'nhsuk-form-group--error' : undefined,
      ];
    },
    showError() {
      return this.hasErrored;
    },
    numberValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.isValid = this.isValidInput(value);
        this.$emit('input', value);
      },
    },
  },
  methods: {
    isValidInput(value) {
      const isANumber = !Number.isNaN(value);
      const lessThanMax = this.max === undefined || value <= this.max;
      const moreThanMin = this.min === undefined || value >= this.min;
      const matchesRegex = `${value}`.match(this.regex) !== null;

      return isANumber && lessThanMax && moreThanMin && matchesRegex;
    },
    validate() {
      this.hasErrored = !this.isValid;
    },
  },
};
</script>

<style lang="scss">
.question {
  display: inline-block;
  margin-bottom: 1em;
}
</style>
