<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <span v-if="error" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <input :id="id"
           ref="textInput"
           v-model="textValue"
           :aria-labelledby="ariaLabels"
           :maxlength="maxlength"
           :min="min"
           :max="max"
           :required="required"
           :name="name"
           :type="type"
           :pattern="pattern"
           :step="step"
           :class="inputClasses"
           autocomplete="off"
           autocorrect="off"
           autocapitalize="off"
           spellcheck="false">
  </div>
</template>

<script>
export default {
  name: 'GenericTextInput',
  props: {
    name: {
      type: String,
      default: undefined,
    },
    maxlength: {
      type: String,
      default: '255',
    },
    min: {
      type: Number,
      default: undefined,
    },
    max: {
      type: Number,
      default: undefined,
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
    id: {
      type: String,
      default: undefined,
    },
    type: {
      type: String,
      default: 'text',
    },
    required: {
      type: Boolean,
      default: false,
    },
    value: {
      type: [String, Number],
      default() { return undefined; },
    },
    pattern: {
      type: String,
      default: undefined,
    },
    step: {
      type: String,
      default: undefined,
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
    textValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.$emit('input', value);
      },
    },
    inputClasses() {
      return [
        'nhsuk-input',
        this.error ? 'nhsuk-input--error' : undefined,
      ];
    },
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
    ariaLabels() {
      const ariaLabels = [
        this.aLabelledBy ? this.aLabelledBy : undefined,
        this.error ? this.errorId : undefined,
      ].join(' ').trim();
      return ariaLabels || undefined;
    },
  },
  methods: {
    focus() {
      this.$refs.textInput.focus();
    },
  },
};

</script>
<style module lang="scss" scoped>
div {
  &.desktopWeb {
    input {
      max-width: 540px;
    }
  }
}
</style>
