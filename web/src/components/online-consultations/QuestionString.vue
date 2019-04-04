<template>
  <div :class="formGroupClasses">
    <!-- eslint-disable vue/no-v-html -->
    <label :id="questionId" :class="$style['nhsuk-label']" :for="answerId" v-html="text"/>
    <generic-text-input :id="answerId"
                        v-model="stringValue"
                        :name="name"
                        :error="error"
                        :error-text="errorText"
                        type="text"/>
  </div>
</template>

<script>

import GenericTextInput from '@/components/widgets/GenericTextInput';

export default {
  name: 'QuestionString',
  components: {
    GenericTextInput,
  },
  props: {
    text: {
      type: String,
      default: '',
    },
    questionId: {
      type: String,
      default: undefined,
    },
    answerId: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
    value: {
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
    formGroupClasses() {
      return [
        'nhsuk-form-group',
        this.error ? 'nhsuk-form-group--error' : undefined,
      ];
    },
    stringValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.$emit('input', value);
      },
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/globals";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/all";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/mixins";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/sass-mq";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/components/label/label";

</style>
