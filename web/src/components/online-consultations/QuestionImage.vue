<template>
  <generic-image-input :id="id"
                       :name="name"
                       :source="source"
                       :required="required"
                       :error="error"
                       :error-text="errorText"
                       @input="onImageClicked($event)"/>
</template>

<script>

import GenericImageInput from '@/components/widgets/GenericImageInput';

export default {
  name: 'QuestionImage',
  components: {
    GenericImageInput,
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
    source: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
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
  data() {
    return {
      clickPositions: [],
    };
  },
  created() {
    this.checkAndEmitIsValueValid(this.clickPositions);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.isValid = this.isValidInput(value);
      this.$emit('validate', this.isValid);
    },
    isValidInput(value) {
      return !this.required ? true : value.length > 0;
    },
    onImageClicked(event) {
      this.clickedPositionValue = {
        x: event.clientX - event.target.offsetLeft,
        y: event.clientY - event.target.offsetTop,
      };
      this.clickPositions.push(this.clickedPositionValue);
      this.checkAndEmitIsValueValid(this.clickPositions);
      this.$emit('input', this.clickPositions);
    },
  },
};
</script>
