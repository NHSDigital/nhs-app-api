<template>
  <fieldset class="nhsuk-fieldset">
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${name}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <legend v-if="legend" class="nhsuk-u-visually-hidden">{{ legend }}</legend>
    <radio-group :key="name"
                 v-model="selectedValue"
                 :name="name"
                 :radios="options"
                 :current-value="selectedValue"
                 :required="required"
                 :render-as-html="renderAsHtml"
                 :a-described-by="ariaDescribed"
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
    legend: {
      type: String,
      default: undefined,
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
      type: Array,
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
    renderAsHtml: {
      type: Boolean,
      default: false,
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
    ariaDescribed() {
      const ariaDescribedContent = [
        this.error && this.errorText ? `${this.name}error` : undefined,
        this.required ? undefined : 'optional-label',
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
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
