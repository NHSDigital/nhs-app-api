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
                 :required="required"
                 @select="selected"/>
  </fieldset>
</template>

<script>
import RadioGroup from '@/components/RadioGroup';
import { questionChoiceAnswerValid } from '@/lib/online-consultations/answer-validators';

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
      default: undefined,
    },
    value: {
      type: String,
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
      return this.options.map(o => o.code);
    },
  },
  watch: {
    selectedValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.selectedValue);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionChoiceAnswerValid(value, this.required, this.validValues));
    },
    selected(value) {
      this.selectedValue = value;
    },
  },
};
</script>
