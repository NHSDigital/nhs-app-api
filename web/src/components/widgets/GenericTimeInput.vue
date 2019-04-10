<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <span v-if="error && errorText" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <div class="inline">
      <div class="nhsuk-date-input__item">
        <label id="hourInputLabel"
               class="nhsuk-label nhsuk-date-input__label"
               :for="hourId">Hour</label>
        <input :id="hourId"
               ref="hourInput"
               v-model.number="hourValue"
               pattern="[0-9]+"
               :min="0"
               :max="24"
               :required="required"
               type="number"
               step="any"
               :class="inputClasses">
      </div>

      <div class="nhsuk-date-input__item">
        <label id="minuteInputLabel"
               class="nhsuk-label nhsuk-date-input__label"
               :for="minuteId">Minute</label>
        <input :id="minuteId"
               ref="minuteInput"
               v-model.number="minuteValue"
               pattern="[0-9]+"
               :min="0"
               :max="59"
               :required="required"
               type="number"
               step="any"
               :class="inputClasses">
      </div>
    </div>
  </div>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';

export default {
  name: 'GenericTimeInput',
  components: {
    GenericTextInput,
  },
  props: {
    hourId: {
      type: String,
      default: 'hour-input',
    },
    minuteId: {
      type: String,
      default: 'minute-input',
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
  computed: {
    hourValue: {
      get() {
        return this.value.hour;
      },
      set(hourValue) {
        this.value.hour = hourValue;
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
        this.value.minute = minuteValue;
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
  },
};
</script>
<style module lang="scss" scoped>
  div {
    display: inline-block;
  }
</style>
