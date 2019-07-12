<template>
  <generic-date-input :id="id"
                      v-model="dateValue"
                      :required="required"
                      :name="name"
                      :error="error"
                      :error-text="errorText"/>
</template>

<script>
import GenericDateInput from '@/components/widgets/GenericDateInput';
import { questionDateAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionDate',
  components: {
    GenericDateInput,
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
          day: undefined,
          month: undefined,
          year: undefined,
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
      dateValue: this.value,
    };
  },
  watch: {
    dateValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.dateValue);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionDateAnswerValid(value, this.required));
    },
  },
};
</script>
