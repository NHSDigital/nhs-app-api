<!-- eslint-disable vue/no-v-html -->
<template>
  <div :class="formGroupClasses">
    <component :is="questionTag"
               v-if="text"
               :id="id"
               :class="[questionClass, 'question', required && 'required']"
               :for="isLabel && labelFor"
               v-html="text"/>
    <div v-else
         :id="id"
         :class="[questionClass, 'question', required && 'required']"
         :for="isLabel && labelFor">
      <slot name="question-slot" />
    </div>
    <p v-if="showOptional"
       class="nhsuk-hint optionalLabel marginBottom">
      ({{ this.$t('onlineConsultations.question.optionalLabel') }})
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
  .nhsuk-form-group a {
    vertical-align: baseline;
    display: inline !important;
  }

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
  .nhsuk-care-card {
    margin-top: 0;
    margin-bottom: 0;
  }

  .nhsuk-hint {
    b {
      display: inline-block;
    }

  }
</style>
