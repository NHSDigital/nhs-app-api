<!-- eslint-disable vue/no-v-html -->
<template>
  <generic-time-input :id="id"
                      v-model="timeValue"
                      :hour-id="hourId"
                      :minute-id="minuteId"
                      :required="required"
                      :error="error"
                      :error-text="errorText"/>
</template>

<script>
import GenericTimeInput from '@/components/widgets/GenericTimeInput';

export default {
  name: 'QuestionTime',
  components: {
    GenericTimeInput,
  },
  props: {
    value: {
      type: Object,
      default() {
        return {
          hour: '',
          minute: '',
        };
      },
    },
    id: {
      default: 'answer-id',
      type: String,
    },
    hourId: {
      default: 'hour-id',
      type: String,
    },
    minuteId: {
      default: 'minute-id',
      type: String,
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
      isValid: true,
    };
  },
  computed: {
    timeValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.checkAndEmitIsValueValid(value);
        this.$emit('input', value);
      },
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.isValid = this.isValidInput(value);
      this.$emit('validate', this.isValid);
    },
    isValidInput(time) {
      const hourIsEmpty = time.hour === undefined || time.hour === '';
      const timeIsEmpty = time.minute === undefined || time.minute === '';

      if (!this.required && hourIsEmpty && timeIsEmpty) {
        return true;
      }

      return !hourIsEmpty && !timeIsEmpty &&
        (time.minute <= 59 && time.minute >= 0) &&
        (time.hour <= 23 && time.hour >= 0);
    },
  },
};
</script>
