<!-- v-html is used to render html in radio button lables as e-consult results can contain html -->
<!-- eslint-disable vue/no-v-html -->
<template>
  <component :is="componentType" class="nhsuk-fieldset"
             :aria-describedby="error ? `${name}-error` : undefined">
    <legend
      v-if="!noHeadingRequired && heading"
      :id="`${name}-legend`"
      ref="legendRef"
      tabindex="-1"
      :class="['nhsuk-fieldset__legend',
               `nhsuk-fieldset__legend--${legendSize}`,
               'nhsuk-u-margin-bottom-3']"
    >
      <template v-if="headingAsHtml">
        <span v-html="heading"/>
      </template>
      <template v-else>{{ heading }}</template>
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
        <div v-for="item in items" :key="`${name}-${getValue(item)}`"
             class="nhsuk-radios__item">
          <input :id="`${name}-${getValue(item)}`" v-model="choice" class="nhsuk-radios__input"
                 :name="name" type="radio" :value="getValue(item)" :required="required"
                 :aria-describedby="error ? `${name}-error` : undefined" aria-invalid="false">
          <label class="nhsuk-label nhsuk-radios__label" :for="`${name}-${getValue(item)}`">
            <template v-if="renderAsHtml"><span v-html="getLabel(item) || item.value"/></template>
            <template v-else>{{ getLabel(item) || item.value }}</template>
          </label>
          <div v-if="item.hint" :id="`${name}-${getValue(item)}-hint`"
               class="nhsuk-hint nhsuk-radios__hint">
            {{ item.hint.text }}
          </div>
        </div>
      </div>
    </div>
  </component>
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
    noHeadingRequired: {
      type: Boolean,
      default: false,
    },
    headingAsHtml: {
      type: Boolean,
      default: false,
    },
    heading: {
      type: String,
      default: undefined,

      // This makes headings required, unless noHeadingRequired is true.
      validator: (value, noHeadingRequired) => noHeadingRequired || (value !== undefined && value !== ''),
    },
    legendSize: {
      type: String,
      default: LegendSize.xs,
      validator: value => Object.values(LegendSize).includes(value),
    },
    enableErrorDialog: {
      type: Boolean,
      default: false,
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
    // only used if radio group already in fieldset
    disableFieldset: {
      type: Boolean,
      default: false,
    },
    // eslint-disable-next-line vue/require-prop-types
    currentValue: {
      default: undefined,
    },
    renderAsHtml: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      itemValues: this.items.map(v => v.value),
      choice: this.currentValue,
      selectedValue: this.currentValue,
      componentType: this.disableFieldset ? 'div' : 'fieldset',
    };
  },
  watch: {
    choice(to) {
      this.checkAndEmitIsValueValid(to);
      this.onSelected(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.choice);
    if (this.choice !== undefined) {
      this.onSelected(this.choice);
    }
  },
  mounted() {
    if (!this.noHeadingRequired && this.heading) {
      if (document.activeElement !== null) {
        document.activeElement.blur();
      }
      this.$refs.legendRef.focus();
    }
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      const isValid = (!this.required && value === undefined) || this.itemValues.includes(value);
      this.$emit('validate', { isValid });
    },
    onSelected(value) {
      this.choice = value;
      this.$emit('onselect', this.choice);
    },
    getValue(item) {
      if (item.value !== undefined && item.value !== '') {
        return item.value;
      } if (item.code !== undefined && item.code !== '') {
        return item.code;
      }
      return '';
    },
    getLabel(item) {
      if (item === undefined || item === null) {
        /* eslint-disable consistent-return */
        return;
      }
      if (item.label !== undefined && item.label !== '') {
        return item.label;
      } if (item.description !== undefined && item.description !== '') {
        return item.description;
      }
      return '';
    },
  },
};
</script>
