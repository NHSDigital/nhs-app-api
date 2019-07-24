<template>
  <error-group :show-error="showError">
    <component :is="container"
               class="nhsuk-fieldset nhsuk-form-group--error">
      <legend v-if="header" class="nhsuk-fieldset__legend">
        {{ header }}
      </legend>
      <error-message v-if="errorMessage && showError">
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
  },
  data() {
    return {
      container: this.header ? 'fieldset' : 'div',
      selectedValue: this.currentValue,
    };
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

<style lang="scss">
.nhsuk-fieldset{
 margin-bottom: 16px;
}

</style>

