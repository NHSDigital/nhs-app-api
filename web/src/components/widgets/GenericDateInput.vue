<template>
  <div>
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label nhsuk-date-input__label" :for="dayId">
          Day
        </label>
        <input :id="dayId"
               ref="day"
               v-model.number="dayValue"
               class="nhsuk-input nhsuk-date-input__input' nhsuk-input--width-2"
               :class="inputClasses(2)"
               :name="dayName" type="number"
               pattern="[0-9]*" max="31" >
      </div>
    </div>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label nhsuk-date-input__label"
               :for="monthId">
          Month
        </label>
        <input :id="monthId"
               ref="month"
               v-model.number="monthValue"
               :class="inputClasses(2)"
               :name="monthName" type="number"
               pattern="[0-9]*" max="12">
      </div>
    </div>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label nhsuk-date-input__label"
               :for="yearId">
          Year
        </label>
        <input :id="yearId"
               ref="year"
               v-model.number="yearValue"
               class="nhsuk-input nhsuk-date-input__input nhsuk-input--width-4"
               :class="inputClasses(4)"
               :name="yearName" type="number"
               pattern="[0-9]*">
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: 'GenericDateInput',
  props: {
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
          day: '',
          month: '',
          year: '',
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
  },
  computed: {
    dayValue: {
      get() {
        return this.value.day;
      },
      set(dayValue) {
        this.$emit('input', {
          ...this.value,
          day: (dayValue !== '' && dayValue < 10) ? `0${dayValue}` : dayValue,
        });
      },
    },
    monthValue: {
      get() {
        return this.value.month;
      },
      set(monthValue) {
        this.$emit('input', {
          ...this.value,
          month: (monthValue !== '' && monthValue < 10) ? `0${monthValue}` : monthValue,
        });
      },
    },
    yearValue: {
      get() {
        return this.value.year;
      },
      set(yearValue) {
        this.$emit('input', {
          ...this.value,
          year: yearValue,
        });
      },
    },
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
  },
  methods: {
    inputClasses(length) {
      return [
        'nhsuk-input',
        'nhsuk-date-input__input',
        `nhsuk-input--width-${length}`,
        this.error ? 'nhsuk-input--error' : undefined,
      ];
    },
  },
};
</script>
