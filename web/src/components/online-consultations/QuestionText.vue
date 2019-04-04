<!-- eslint-disable vue/no-v-html -->
<template>
  <div :class="formGroupClasses">
    <label :id="questionId"
           :for="answerId"
           class="question nhsuk-label"
           v-html="text"/>
    <generic-text-area :id="answerId"
                       v-model="textValue"
                       :name="answerName"
                       :maxlength="maxlength"/>
  </div>
</template>

<script>
import GenericTextArea from '@/components/widgets/GenericTextArea';

export default {
  name: 'QuestionText',
  components: {
    GenericTextArea,
  },
  props: {
    questionId: {
      type: String,
      default: 'text-question',
    },
    value: {
      type: String,
      default: '',
    },
    answerId: {
      type: String,
      default: 'text-answer',
    },
    answerName: {
      type: String,
      default: 'text-answer',
    },
    text: {
      type: String,
      required: true,
    },
    maxlength: {
      type: String,
      default: '255',
    },
  },
  data() {
    return {
      model: this.value,
    };
  },
  computed: {
    textValue: {
      get() {
        return this.model;
      },
      set(value) {
        this.model = value;
        this.$emit('input', value);
      },
    },
    formGroupClasses() {
      return [
        'nhsuk-form-group',
        this.error ? 'nhsuk-form-group--error' : undefined,
      ];
    },
  },
};
</script>
<style lang="scss">
.question {
  display: inline-block;
  margin-bottom: 1em;
}
</style>
