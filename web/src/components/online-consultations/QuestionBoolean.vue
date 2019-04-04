<template>
  <div :class="error && $style['nhsuk-form-group--error']">
    <fieldset :class="[$style['nhsuk-fieldset'], $style['nhsuk-form-group--error']]">
      <!-- eslint-disable vue/no-v-html -->
      <legend :id="questionId" :class="$style['nhsuk-fieldset__legend']" v-html="text"/>
      <span v-if="error" :id="errorId" :class="$style['nhsuk-error-message']">
        <span :class="$style['nhsuk-u-visually-hidden']">
          {{ errorText }}</span>
      </span>

      <generic-radio-button :key="optionOneKey"
                            :label="optionOneLabel"
                            :name="name"
                            value="Yes"
                            :selected-value="value"
                            @select="selected"/>
      <generic-radio-button :key="optionTwoKey"
                            :label="optionTwoLabel"
                            :name="name"
                            value="No"
                            :selected-value="value"
                            @select="selected"/>
    </fieldset>
  </div>
</template>

<script>
import GenericRadioButton from '@/components/widgets/GenericRadioButton';

export default {
  name: 'QuestionBoolean',
  components: { GenericRadioButton },
  props: {
    text: {
      type: String,
      default: 'checkbox',
      required: true,
    },
    questionId: {
      type: String,
      default: undefined,
      required: true,
    },
    optionOneKey: {
      type: String,
      default: undefined,
    },
    optionTwoKey: {
      type: String,
      default: undefined,
    },
    optionOneLabel: {
      type: String,
      default: undefined,
      required: true,
    },
    optionTwoLabel: {
      type: String,
      default: undefined,
      required: true,
    },
    name: {
      type: String,
      default: undefined,
      required: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorId: {
      type: String,
      default: '',
    },
    errorText: {
      type: String,
      default: 'Please make a choice',
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
    },
  },
  methods: {
    selected(value) {
      this.$emit('input', value);
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/globals";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_ifff";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_functions";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/colours";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/sass-mq";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/_typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_mixins";
 @import "../../../node_modules/nhsuk-frontend/packages/core/objects/_form-group";
 @import "../../../node_modules/nhsuk-frontend/packages/components/error-message/error-message";
 @import "../../../node_modules/nhsuk-frontend/packages/components/fieldset/fieldset";

 .nhsuk-fieldset{
  margin-bottom: 16px;
 }
</style>

