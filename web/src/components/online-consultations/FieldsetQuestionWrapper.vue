<template>
  <fieldset class="nhsuk-fieldset">
    <div :class="formGroupClasses">
      <legend>
        <question :id="id"
                  :question-tag="questionTag"
                  :text="text"
                  :label-for="labelFor"
                  :error="error"
                  :is-legend="isLegend"
                  :required="required">
          <template slot="questionSlot">
            <slot name="questionSlot"/>
          </template>
        </question>
      </legend>
      <slot />
    </div>
  </fieldset>
</template>

<script>
import Question from '@/components/online-consultations/Question';

export default {
  name: 'FieldsetQuestionWrapper',
  components: {
    Question,
  },
  props: {
    id: {
      type: String,
      default: 'question',
    },
    questionTag: {
      type: String,
      default: 'div',
    },
    labelFor: {
      type: String,
      default: undefined,
    },
    text: {
      type: String,
      default: undefined,
    },
    error: {
      type: Boolean,
      default: false,
    },
    isLegend: {
      type: Boolean,
      default: false,
    },
    required: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    formGroupClasses() {
      return this.error ? ['nhsuk-form-group', 'nhsuk-form-group--error'] : 'nhsuk-form-group';
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/question";
</style>
