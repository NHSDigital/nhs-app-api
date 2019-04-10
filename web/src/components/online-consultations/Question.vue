<!-- eslint-disable vue/no-v-html -->
<template>
  <div :class="formGroupClasses">
    <component :is="questionTag"
               :id="id"
               :class="questionClass"
               :for="labelFor"
               v-html="text"/>
    <slot />
  </div>
</template>

<script>
export default {
  name: 'Question',
  props: {
    id: {
      type: String,
      default: 'question',
    },
    questionTag: {
      type: String,
      default: 'label',
    },
    labelFor: {
      type: String,
      default: undefined,
    },
    text: {
      type: String,
      required: true,
    },
    error: {
      type: Boolean,
      default: false,
    },
    isLegend: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    questionClass() {
      return this.isLegend ? 'nhsuk-fieldset__legend' : 'nhsuk-label';
    },
    formGroupClasses() {
      return this.error ? ['nhsuk-form-group', 'nhsuk-form-group--error'] : 'nhsuk-form-group';
    },
  },
};
</script>
