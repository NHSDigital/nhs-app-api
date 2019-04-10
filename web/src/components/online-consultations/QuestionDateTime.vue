<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <div :class="$style.dateTimeContainer">
      <generic-date-input :id="dateAnswerId"
                          v-model="dateTimeValue"
                          :day-id="dayId"
                          :month-id="monthId"
                          :year-id="yearId"
                          :day-name="dayName"
                          :month-name="monthName"
                          :year-name="yearName"
                          :error="error"/>
      <generic-time-input :id="timeAnswerId"
                          v-model="dateTimeValue"
                          :hour-id="hourId"
                          :minute-id="minuteId"
                          :error="error"/>
    </div>
  </div>
</template>
<script>
import GenericDateInput from '@/components/widgets/GenericDateInput';
import GenericTimeInput from '@/components/widgets/GenericTimeInput';

export default {
  name: 'QuestionDatetime',
  components: {
    GenericDateInput,
    GenericTimeInput,
  },
  props: {
    id: {
      type: String,
      default: 'datetime-answer',
    },
    dayId: {
      type: String,
      default: undefined,
    },
    monthId: {
      type: String,
      default: undefined,
    },
    yearId: {
      type: String,
      default: undefined,
    },
    dayName: {
      type: String,
      default: undefined,
    },
    monthName: {
      type: String,
      default: undefined,
    },
    yearName: {
      type: String,
      default: undefined,
    },
    hourId: {
      default: 'hour-id',
      type: String,
    },
    minuteId: {
      default: 'minute-id',
      type: String,
    },
    value: {
      type: Object,
      default() {
        return {
          day: undefined,
          month: undefined,
          year: undefined,
          hour: undefined,
          minute: undefined,
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
      isValid: true,
    };
  },
  computed: {
    dateTimeValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.checkAndEmitIsValueValid(value);
        this.$emit('input', value);
      },
    },
    dateAnswerId() {
      return `${this.id}-date`;
    },
    timeAnswerId() {
      return `${this.id}-time`;
    },
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
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
    isValidInput(datetime) {
      const dayEmpty = datetime.day === undefined || datetime.day === '';
      const monthEmpty = datetime.month === undefined || datetime.month === '';
      const yearEmpty = datetime.year === undefined || datetime.year === '';
      const hourEmpty = datetime.hour === undefined || datetime.hour === '';
      const minuteEmpty = datetime.minute === undefined || datetime.minute === '';

      if (!this.required &&
          dayEmpty &&
          monthEmpty &&
          yearEmpty &&
          hourEmpty &&
          minuteEmpty) {
        return true;
      }

      if (dayEmpty ||
          monthEmpty ||
          yearEmpty ||
          hourEmpty ||
          minuteEmpty ||
          datetime.year < 1000 ||
          datetime.year > 9999 ||
          datetime.hour < 0 ||
          datetime.hour > 23 ||
          datetime.minute < 0 ||
          datetime.minute > 59) {
        return false;
      }

      return !Number.isNaN(new Date(`${datetime.year}-${datetime.month}-${datetime.day}`).getTime());
    },
  },
};
</script>
<style module lang="scss" scoped>
.dateTimeContainer{
  display: inline-flex;
}
</style>
