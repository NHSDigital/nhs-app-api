<!-- eslint-disable vue/no-v-html -->
<template>
  <generic-text-input :id="id"
                      v-model="numberValue"
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
import { INTEGER, DECIMAL } from '@/lib/online-consultations/constants/question-types';
import { questionNumberAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionNumber',
  components: {
    GenericTextInput,
  },
  props: {
    id: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: undefined,
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
      default: INTEGER,
      validator: value => ([INTEGER, DECIMAL].includes(value)),
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
      numberValue: this.value,
    };
  },
  watch: {
    numberValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionNumberAnswerValid(value, this.required, this.type, this.min, this.max));
    },
  },
};
</script>
