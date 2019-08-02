<!-- eslint-disable vue/no-v-html -->
<template>
  <div :class="formGroupClasses">
    <component :is="questionTag"
               :id="id"
               :class="[questionClass, 'question', required && 'required']"
               :for="isLabel && labelFor"
               v-html="text"/>
    <p v-if="showOptional"
       class="nhsuk-hint optionalLabel marginBottom">
      ({{ this.$t('appointments.admin_help.question.optional_label') }})
    </p>
    <slot />
  </div>
</template>

<script>
import { get } from 'lodash/fp';
import { SUCCESS } from '@/lib/online-consultations/constants/status-types';

export default {
  name: 'Question',
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
    required: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    isLabel() {
      return this.questionTag === 'label';
    },
    questionClass() {
      return this.isLegend ? 'nhsuk-fieldset__legend' : 'nhsuk-label';
    },
    formGroupClasses() {
      return this.error ? ['nhsuk-form-group', 'nhsuk-form-group--error'] : 'nhsuk-form-group';
    },
    showOptional() {
      return !(this.required || get('$store.state.onlineConsultations.status', this) === SUCCESS);
    },
  },
};
</script>

<style lang="scss">
  .question.required,
  .optionalLabel.marginBottom {
    margin-bottom: 1em !important;
  }

  .question {
    .nhsuk-hint:last-of-type {
      margin-bottom: 0 !important;
    }

    a {
      display: inline;
      vertical-align: baseline;
    }
  }
</style>
