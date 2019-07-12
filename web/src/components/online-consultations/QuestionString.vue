<template>
  <generic-text-input :id="id"
                      v-model="stringValue"
                      :name="name"
                      :error="error"
                      :error-text="errorText"
                      type="text"
                      :required="required"/>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { questionStringAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionString',
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
    required: {
      type: Boolean,
      default: true,
    },
    value: {
      type: String,
      default: '',
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
      stringValue: this.value,
    };
  },
  watch: {
    stringValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionStringAnswerValid(value, this.required));
    },
  },
};
</script>
