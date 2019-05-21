<template>
  <generic-attachment :id="id"
                      :name="name"
                      :error="error"
                      :error-text="errorText"
                      :required="required"
                      @change="onSelectedFileChanged($event)"/>
</template>

<script>

import GenericAttachment from '@/components/widgets/GenericAttachment';

export default {
  name: 'QuestionAttachment',
  components: {
    GenericAttachment,
  },
  props: {
    id: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: undefined,
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: String,
      default: '',
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      fileValue: undefined,
    };
  },
  methods: {
    checkAndEmitValueIsValid(fileValue) {
      this.isValid = this.isValidInput(fileValue);
      this.$emit('validate', this.isValid);
    },
    isValidInput(fileValue) {
      return !this.required ? true : !!fileValue;
    },
    onSelectedFileChanged(event) {
      [this.fileValue] = event.target.files;
      this.checkAndEmitValueIsValid(this.fileValue);
      this.$emit('input', this.fileValue);
    },
  },
};
</script>

