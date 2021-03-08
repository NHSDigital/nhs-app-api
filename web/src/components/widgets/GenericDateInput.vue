<template>
  <div>
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
      {{ errorText }}
    </span>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label nhsuk-date-input__label"
               :for="`${id}-day`">
          {{ $t('generic.day') }}
        </label>
        <input :id="`${id}-day`"
               ref="day"
               v-model.number="dayValue"
               class="nhsuk-input nhsuk-date-input__input' nhsuk-input--width-2"
               :class="['ios-accessibility', inputClasses(2)]"
               :name="`${name}-day`" type="number"
               :required="required"
               pattern="[0-9]*" max="31"
               :aria-describedby="ariaDescribed">
      </div>
    </div>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label nhsuk-date-input__label"
               :for="`${id}-month`">
          {{ $t('generic.month') }}
        </label>
        <input :id="`${id}-month`"
               ref="month"
               v-model.number="monthValue"
               :class="['ios-accessibility', inputClasses(2)]"
               :name="`${name}-month`"
               :required="required"
               type="number"
               pattern="[0-9]*" max="12"
               :aria-describedby="ariaDescribed">
      </div>
    </div>
    <div class="nhsuk-date-input__item">
      <div class="nhsuk-form-group">
        <label class="nhsuk-label nhsuk-date-input__label"
               :for="`${id}-year`">
          {{ $t('generic.year') }}
        </label>
        <input :id="`${id}-year`"
               ref="year"
               v-model.number="yearValue"
               class="nhsuk-input nhsuk-date-input__input nhsuk-input--width-4"
               :class="['ios-accessibility', inputClasses(4)]"
               :name="`${name}-year`"
               :required="required"
               type="number"
               pattern="[0-9]*"
               :aria-describedby="ariaDescribed">
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: 'GenericDateInput',
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
    required: {
      type: Boolean,
      default: true,
    },
    aDescribedBy: {
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
    ariaDescribed() {
      const ariaDescribedContent = [
        this.aDescribedBy ? this.aDescribedBy : undefined,
        this.error && this.errorText ? `${this.errorId}` : undefined,
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
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

<style module lang="scss" scoped>
  @import "@/style/custom/ios-accessibility";
</style>
