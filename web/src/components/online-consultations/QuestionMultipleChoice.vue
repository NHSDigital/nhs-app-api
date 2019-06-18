<template>
  <fieldset class="nhsuk-fieldset">
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <checkbox-group :key="name"
                    v-model="selectedValues"
                    :name="name"
                    :checkboxes="options"
                    :value="selectedValues"
                    @select="selectedValuesChanged" />
  </fieldset>
</template>

<script>
import CheckboxGroup from '@/components/CheckboxGroup';

export default {
  name: 'QuestionMultipleChoice',
  components: {
    CheckboxGroup,
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
    value: {
      type: Array,
      default: () => [],
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
      selectedValues: this.value,
    };
  },
  created() {
    this.checkAndEmitIsValueValid(this.selectedValues);
  },
  methods: {
    checkAndEmitIsValueValid(val) {
      this.isValid = this.isValidInput(val);
      this.$emit('validate', this.isValid);
    },
    selectedValuesChanged(val) {
      this.selectedValues = val;
      this.checkAndEmitIsValueValid(val);
      this.$emit('input', val);
    },
    isValidInput(val) {
      return !this.required ? true : val.length > 0;
    },
  },
};
</script>
