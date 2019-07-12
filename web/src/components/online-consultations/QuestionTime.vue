<!-- eslint-disable vue/no-v-html -->
<template>
  <generic-time-input :id="id"
                      v-model="timeValue"
                      :required="required"
                      :name="name"
                      :error="error"
                      :error-text="errorText"/>
</template>

<script>
import GenericTimeInput from '@/components/widgets/GenericTimeInput';
import { questionTimeAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionTime',
  components: {
    GenericTimeInput,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    name: {
      type: String,
      required: true,
    },
    value: {
      type: Object,
      default() {
        return {
          hour: '',
          minute: '',
        };
      },
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
      timeValue: this.value,
    };
  },
  watch: {
    timeValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionTimeAnswerValid(value, this.required));
    },
  },
};
</script>
