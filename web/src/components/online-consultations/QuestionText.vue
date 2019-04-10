<template>
  <generic-text-area :id="id"
                     v-model="textValue"
                     :required="required"
                     :name="name"
                     :error="error"
                     :error-text="errorText"
                     :maxlength="maxlength"/>
</template>

<script>
import GenericTextArea from '@/components/widgets/GenericTextArea';

export default {
  name: 'QuestionText',
  components: {
    GenericTextArea,
  },
  props: {
    value: {
      type: String,
      default: '',
    },
    id: {
      type: String,
      default: 'text-answer',
    },
    name: {
      type: String,
      default: 'text-answer',
    },
    maxlength: {
      type: String,
      default: '255',
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
      isValid: true,
    };
  },
  computed: {
    textValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.checkAndEmitIsValueValid(value);
        this.$emit('input', value);
      },
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.isValid = this.isValidInput(value);
      this.$emit('validate', this.isValid);
    },
    isValidInput(value) {
      const isEmpty = (this.required && value === '');
      return !isEmpty && (value.length <= this.maxlength);
    },
  },
};
</script>
