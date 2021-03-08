<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
      {{ errorText }}
    </span>
    <div class="inline">
      <div class="nhsuk-date-input__item">
        <label id="hourInputLabel"
               class="nhsuk-label nhsuk-date-input__label"
               :for="`${id}-hour`">
          {{ $t('generic.hour') }}
        </label>
        <input :id="`${id}-hour`"
               ref="hourInput"
               v-model.number="hourValue"
               pattern="[0-9]+"
               :min="0"
               :max="23"
               :required="required"
               :name="`${name}-hour`"
               type="number"
               step="any"
               :class="['ios-accessibility', inputClasses]"
               :aria-describedby="ariaDescribed">
      </div>

      <div class="nhsuk-date-input__item">
        <label id="minuteInputLabel"
               class="nhsuk-label nhsuk-date-input__label"
               :for="`${id}-minute`">
          {{ $t('generic.minute') }}
        </label>
        <input :id="`${id}-minute`"
               ref="minuteInput"
               v-model.number="minuteValue"
               pattern="[0-9]+"
               :min="0"
               :max="59"
               :required="required"
               :name="`${name}-minute`"
               type="number"
               step="any"
               :class="['ios-accessibility', inputClasses]"
               :aria-describedby="ariaDescribed">
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'GenericTimeInput',
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
    aDescribedBy: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    hourValue: {
      get() {
        return this.value.hour;
      },
      set(hourValue) {
        this.$emit('input', {
          ...this.value,
          hour: (hourValue !== '' && hourValue < 10) ? `0${hourValue}` : hourValue,
        });
      },
    },
    minuteValue: {
      get() {
        return this.value.minute;
      },
      set(minuteValue) {
        this.$emit('input', {
          ...this.value,
          minute: (minuteValue !== '' && minuteValue < 10) ? `0${minuteValue}` : minuteValue,
        });
      },
    },
    inputClasses() {
      return [
        'nhsuk-input',
        'nhsuk-date-input__input',
        'nhsuk-input--width-2',
        this.error ? 'nhsuk-input--error' : undefined,
      ];
    },
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
    ariaDescribed() {
      const ariaDescribedContent = [
        this.aDescribedBy ? this.aDescribedBy : undefined,
        this.error ? `${this.errorId}` : undefined,
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/div-inline-block";
  @import "@/style/custom/ios-accessibility";
</style>
