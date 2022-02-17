<template>
  <span>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${name}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <nhs-uk-radio-group
      v-model="selectedValue"
      :heading="legend? ('<h1>' + legend + '</h1>') : undefined"
      :heading-as-html="true"
      :no-heading-required="legend == false"
      :name="name"
      :legend-size="'l'"
      :required="required"
      :items="options"
      :render-as-html="renderAsHtml"
      :disable-fieldset="true"
      :current-value="currentChoice"
      @onselect="selected"
    />
  </span>
</template>

<script>
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import { questionChoiceAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionChoice',
  components: {
    NhsUkRadioGroup,
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
    currentChoice() {
      return this.selectedValue;
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
