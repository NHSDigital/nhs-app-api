<template>
  <generic-date-input :id="id"
                      v-model="dateValue"
                      :day-id="dayId"
                      :month-id="monthId"
                      :year-id="yearId"
                      :day-name="dayName"
                      :month-name="monthName"
                      :year-name="yearName"
                      :error="error"
                      :error-text="errorText"/>
</template>

<script>
import GenericDateInput from '@/components/widgets/GenericDateInput';

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
      isValid: true,
    };
  },
  computed: {
    dateValue: {
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
    this.checkAndEmitIsValueValid(this.dateValue);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.isValid = this.isValidInput(value);
      this.$emit('validate', this.isValid);
    },
    isValidInput(date) {
      const dayEmpty = date.day === undefined || date.day === '';
      const monthEmpty = date.month === undefined || date.month === '';
      const yearEmpty = date.year === undefined || date.year === '';

      if (!this.required &&
          dayEmpty &&
          monthEmpty &&
          yearEmpty) {
        return true;
      }

      if (dayEmpty ||
          monthEmpty ||
          yearEmpty ||
          date.year < 1000 ||
          date.year > 9999) {
        return false;
      }

      return !Number.isNaN(new Date(`${date.year}-${date.month}-${date.day}`).getTime());
    },
  },
};
</script>
