<template>
  <fieldset class="nhsuk-fieldset">
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${name}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <!--
      temporarily set required to false from the checkbox group until we
      can determine if ALL checkboxes are required or specific values
      are required.
    -->
    <checkbox-group :key="name"
                    v-model="selectedValues"
                    :name="name"
                    :checkboxes="options"
                    :required="false"
                    @select="selectedValuesChanged" />
  </fieldset>
</template>

<script>
import CheckboxGroup from '@/components/CheckboxGroup';
import { questionMultipleChoiceAnswerValid } from '@/lib/online-consultations/answer-validators';

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
      type: Array,
      default: undefined,
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
  computed: {
    validValues() {
      return this.options.map(o => o.code);
    },
  },
  watch: {
    selectedValues(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.selectedValues);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionMultipleChoiceAnswerValid(value, this.required, this.validValues));
    },
    selectedValuesChanged(value) {
      this.selectedValues = value;
    },
  },
};
</script>
