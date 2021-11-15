<template>
  <fieldset class="nhsuk-fieldset" :aria-describedby="error ? `${name}-error` : undefined">
    <legend :class="['nhsuk-fieldset__legend', legendSize, 'nhsuk-u-margin-bottom-3']">
      <h1 ref="nhsukRadiosHeader"
          class="nhsuk-fieldset__heading nhsuk-u-margin-top-3"
          tabindex="-1">{{ heading }}</h1>
    </legend>
    <error-dialog v-if="enableErrorDialog && error"
                  :header-locale-ref="errorHeadingReference"
                  :errors="errorText"/>


    <div :class="['nhsuk-form-group', error ? 'nhsuk-form-group--error' : undefined]">
      <span v-if="error" :id="`${name}-error`" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ errorText }}
      </span>

      <div class="nhsuk-radios">
        <div v-for="item in items" :key="`${name}-${item.value}`"
             class="nhsuk-radios__item">
          <input :id="`${name}-${item.value}`" v-model="choice" class="nhsuk-radios__input"
                 :name="name" type="radio" :value="item.value" :required="required"
                 :aria-describedby="error ? `${name}-error` : undefined" aria-invalid="false">
          <label class="nhsuk-label nhsuk-radios__label" :for="`${name}-${item.value}`">
            {{ item.label || item.value }}
          </label>
          <div v-if="item.hint" :id="`${name}-${item.value}-hint`"
               class="nhsuk-hint nhsuk-radios__hint">
            {{ item.hint.text }}
          </div>
        </div>
      </div>
    </div>
  </fieldset>
</template>

<script>
import ErrorDialog from '@/components/ErrorDialog';
import LegendSize from '@/lib/legend-size';

export default {
  name: 'NhsUkRadioGroup',
  components: {
    ErrorDialog,
  },
  props: {
    name: {
      type: String,
      required: true,
    },
    heading: {
      type: String,
      required: true,
    },
    legendSize: {
      type: String,
      default: LegendSize.ExtraLarge,
      validator: value => Object.values(LegendSize).includes(value),
    },
    // TODO: to be reverted to false by default
    enableErrorDialog: {
      type: Boolean,
      default: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorHeadingReference: {
      type: String,
      default: 'generic.thereIsAProblem',
    },
    errorText: {
      type: String,
      default: undefined,
    },
    items: {
      type: Array,
      required: true,
    },
    required: {
      type: Boolean,
      default: true,
    },
    value: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      itemValues: this.items.map(v => v.value),
      choice: this.value,
    };
  },
  watch: {
    choice(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.choice);
  },
  mounted() {
    if (document.activeElement !== null) {
      document.activeElement.blur();
    }
    this.$refs.nhsukRadiosHeader.focus();
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      const isValid = (!this.required && value === undefined) || this.itemValues.includes(value);
      this.$emit('validate', { isValid });
    },
  },
};
</script>
