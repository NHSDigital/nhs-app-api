<template>
  <fieldset class="nhsuk-fieldset">
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <radio-group :key="name"
                 v-model="selectedValue"
                 :name="name"
                 :radios="options"
                 :current-value="selectedValue"
                 @select="selected"/>
  </fieldset>
</template>

<script>
import RadioGroup from '@/components/RadioGroup';

export default {
  name: 'QuestionChoice',
  components: {
    RadioGroup,
  },
  props: {
    name: {
      type: String,
      required: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorId: {
      type: String,
      default: undefined,
    },
    errorText: {
      type: String,
      default: 'Please make a choice',
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: undefined,
    },
    options: {
      type: Array,
      required: true,
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      selectedValue: this.value,
    };
  },
  computed: {
    validValues() {
      const codes = this.options.map(option => option.code);
      if (!this.required) {
        codes.push(undefined);
      }
      return codes;
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.selectedValue);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.isValid = this.isValidInput(value);
      this.$emit('validate', this.isValid);
    },
    selected(value) {
      this.selectedValue = value;
      this.checkAndEmitIsValueValid(value);
      this.$emit('input', value);
    },
    isValidInput(value) {
      return this.validValues.includes(value);
    },
  },
};
</script>
