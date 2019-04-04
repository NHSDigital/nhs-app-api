<template>
  <error-group :show-error="showError">
    <component :is="container" class="radio-group">
      <legend v-if="header">
        <h3>{{ header }}</h3>
      </legend>
      <error-message v-if="errorMessage && showError">
        {{ errorMessage }}
      </error-message>
      <generic-radio-button v-for="radio in radios"
                            :key="radio.value"
                            :selected-value="selectedValue"
                            :class="{ inline: inline }"
                            :hint="radio.hint"
                            :label="radio.label"
                            :name="name"
                            :value="radio.value"
                            @select="selected"/>
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
  },
};
</script>

<style module lang="scss" scoped>
@import '../style/radiobutton';

.nhsuk-fieldset{
 margin-bottom: 16px;
}
</style>

