<template>
  <error-group :show-error="showError" class="nhsuk-form-group">
    <component :is="container"
               class="nhsuk-fieldset nhsuk-form-group--error">
      <legend v-if="header"
              :class="['nhsuk-fieldset__legend', `nhsuk-fieldset__legend--${headerSize}`]">
        {{ header }}
      </legend>
      <error-message v-if="errorMessage && showError" id="error_txt">
        {{ errorMessage }}
      </error-message>
      <div class="nhsuk-radios">
        <generic-radio-button v-for="radio in radios"
                              :key="getValue(radio)"
                              :selected-value="selectedValue"
                              :hint="radio.hint"
                              :label="getLabel(radio)"
                              :name="name"
                              :value="getValue(radio)"
                              :required="required"
                              :render-as-html="renderAsHtml"
                              :a-described-by="ariaDescribed"
                              class="nhsuk-radios__item"
                              @select="selected"/>
      </div>
    </component>
  </error-group>
</template>

<script>
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';

export default {
  name: 'RadioGroup',
  components: {
    ErrorGroup,
    ErrorMessage,
    GenericRadioButton,
  },
  props: {
    // eslint-disable-next-line vue/require-prop-types
    currentValue: {
      default: '',
    },
    errorMessage: {
      type: String,
      default: '',
    },
    header: {
      type: String,
      default: '',
    },
    headerSize: {
      type: String,
      default: 'xs',
      validator: value => ['xs', 's', 'm', 'l', 'xl'].includes(value),
    },
    inline: {
      type: Boolean,
      default: false,
    },
    name: {
      type: String,
      default: 'radioButton',
    },
    radios: {
      type: Array,
      required: true,
    },
    showError: {
      type: Boolean,
      default: false,
    },
    required: {
      type: Boolean,
      default: true,
    },
    renderAsHtml: {
      type: Boolean,
      default: false,
    },
    aDescribedBy: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      container: this.header ? 'fieldset' : 'div',
      selectedValue: this.currentValue,
    };
  },
  computed: {
    ariaDescribed() {
      const ariaDescribedContent = [
        this.aDescribedBy ? this.aDescribedBy : undefined,
        this.errorMessage && this.showError ? 'error_txt' : undefined,
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
    },
  },
  methods: {
    selected(value) {
      this.selectedValue = value;
      this.$emit('select', this.selectedValue);
    },
    getValue(radio) {
      if (radio.value !== undefined && radio.value !== '') {
        return radio.value;
      } if (radio.code !== undefined && radio.code !== '') {
        return radio.code;
      }
      return '';
    },
    getLabel(radio) {
      if (radio.label !== undefined && radio.label !== '') {
        return radio.label;
      } if (radio.description !== undefined && radio.description !== '') {
        return radio.description;
      }
      return '';
    },
  },
};
</script>
