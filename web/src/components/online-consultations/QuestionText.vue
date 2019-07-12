<template>
  <generic-text-area :id="id"
                     v-model="textValue"
                     :required="required"
                     :name="name"
                     :error="error"
                     :error-text="errorText"
                     :maxlength="maxlength"/>
</template>

<script>
import GenericTextArea from '@/components/widgets/GenericTextArea';
import { questionTextAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionText',
  components: {
    GenericTextArea,
  },
  props: {
    value: {
      type: String,
      default: '',
    },
    id: {
      type: String,
      default: 'text-answer',
    },
    name: {
      type: String,
      default: 'text-answer',
    },
    maxlength: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
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
    return {
      textValue: this.value,
    };
  },
  watch: {
    textValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionTextAnswerValid(value, this.required, this.maxlength));
    },
  },
};
</script>
