<template>
  <error-group :show-error="showInlineError">
    <error-message v-if="showInlineError && errorMessage" :id="`${checkboxId}-error`">
      {{ errorMessage }}
    </error-message>
    <generic-checkbox v-model="selected"
                      :checkbox-id="checkboxId">
      <slot/>
    </generic-checkbox>
  </error-group>
</template>

<script>
import ErrorGroup from '@/components/ErrorGroup';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';

export default {
  name: 'Checkbox',
  components: {
    ErrorGroup,
    ErrorMessage,
    GenericCheckbox,
  },
  props: {
    checkboxId: {
      type: String,
      default: undefined,
    },
    errorMessage: {
      type: String,
      default: '',
    },
    showError: Boolean,
    value: Boolean,
  },
  computed: {
    selected: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit('input', val);
      },
    },
    showInlineError() {
      return this.showError && !this.selected;
    },
  },
};
</script>
