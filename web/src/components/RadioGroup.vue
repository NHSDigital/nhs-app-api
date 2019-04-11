<template>
  <error-group :show-error="showError">
    <component :is="container"
               :class="[$style['nhsuk-fieldset'], $style['nhsuk-form-group--error']]">
      <legend v-if="header" :class="$style['nhsuk-fieldset__legend']">
        {{ header }}
      </legend>
      <error-message v-if="errorMessage && showError">
        {{ errorMessage }}
      </error-message>
      <div :class="[$style['nhsuk-radios'], inline && $style['nhsuk-radios--inline']]">
        <generic-radio-button v-for="radio in radios"
                              :key="radio.value"
                              :selected-value="selectedValue"
                              :hint="radio.hint"
                              :label="radio.label"
                              :name="name"
                              :value="radio.value"
                              :class="$style['nhsuk-radios__item']"
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
@import "../../node_modules/nhsuk-frontend/packages/core/settings/globals";
@import "../../node_modules/nhsuk-frontend/packages/core/tools/_ifff";
@import "../../node_modules/nhsuk-frontend/packages/core/tools/_functions";
@import "../../node_modules/nhsuk-frontend/packages/core/settings/spacing";
@import "../../node_modules/nhsuk-frontend/packages/core/tools/spacing";
@import "../../node_modules/nhsuk-frontend/packages/core/settings/colours";
@import "../../node_modules/nhsuk-frontend/packages/core/tools/sass-mq";
@import "../../node_modules/nhsuk-frontend/packages/core/settings/_typography";
@import "../../node_modules/nhsuk-frontend/packages/core/tools/_typography";
@import "../../node_modules/nhsuk-frontend/packages/core/tools/_mixins";
@import "../../node_modules/nhsuk-frontend/packages/core/objects/_form-group";
@import "../../node_modules/nhsuk-frontend/packages/components/error-message/error-message";
@import "../../node_modules/nhsuk-frontend/packages/components/fieldset/fieldset";
@import "../../node_modules/nhsuk-frontend/packages/components/radios/radios";

.nhsuk-fieldset{
 margin-bottom: 16px;
}

 .nhsuk-radios--inline{
  display: inline;
 }

</style>

