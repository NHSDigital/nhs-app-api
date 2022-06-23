<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
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
      <slot name="questionSlot" />
    </div>
    <p v-if="showOptional"
       id="optional-label"
       class="nhsuk-hint optionalLabel marginBottom">
      ({{ $t('onlineConsultations.question.optionalLabel') }})
    </p>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
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
    showOptional() {
      return !(this.required || get('$store.state.onlineConsultations.status', this) === SUCCESS);
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/question";
</style>
