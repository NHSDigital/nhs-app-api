<template>
  <div id="form-error-summary" ref="formErrorSummaryContainer" :class="['nhsuk-error-summary', $style.formErrorSummary]"
       aria-labelledby="error-summary-title"
       role="alert" data-purpose="error" aria-atomic="true">
    <h2 id="error-summary-title" class="nhsuk-error-summary__title" data-purpose="error-heading">
      {{ $t(headerLocaleRef) }}
    </h2>
    <div class="nhsuk-error-summary__body">
      <ul class="nhsuk-list nhsuk-error-summary__list" data-purpose="error-reasons">
        <li v-for="(error, index) in validationErrors" :key="error" data-purpose="error-reason">
          <a :href="'#' + validationErrorsIds[index]" @click.prevent="$event.target.blur(); scrollToError(getErrorId(index));"> {{ error }} </a>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import isArray from 'lodash/fp/isArray';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

export default {
  name: 'FormErrorSummary',
  props: {
    headerLocaleRef: {
      type: String,
      default: 'generic.thereIsAProblem',
    },
    errors: {
      type: [String, Array],
      required: true,
    },
    errorsIds: {
      type: [String, Array],
      required: true,
    },
  },
  computed: {
    validationErrors() {
      return isArray(this.errors) ? this.errors : [this.errors];
    },
    validationErrorsIds() {
      return isArray(this.errorsIds) ? this.errorsIds : [this.errorsIds];
    },
  },
  beforeMount() {
    EventBus.$on(FOCUS_ERROR_ELEMENT, this.scrollToTopAndFocusFormErrorSummary);
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_ERROR_ELEMENT, this.scrollToTopAndFocusFormErrorSummary);
  },
  mounted() {
    this.focusDialog();
  },
  methods: {
    scrollToTopAndFocusFormErrorSummary() {
      window.scrollTo(0, 0);
      this.focusDialog();
    },
    focusDialog() {
      this.$refs.formErrorSummaryContainer.setAttribute('tabindex', '-1');
      this.$refs.formErrorSummaryContainer.focus();
    },
    getErrorId(index) {
      if (this.validationErrorsIds[index] !== undefined) {
        return this.validationErrorsIds[index];
      }
      return this.validationErrorsIds[0];
    },
    scrollToError(id) {
      document.getElementById(id).scrollIntoView();
      document.getElementById(id).focus();
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/form-error-summary";
</style>
