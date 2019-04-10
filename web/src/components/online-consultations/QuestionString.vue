<template>
  <generic-text-input :id="id"
                      v-model="stringValue"
                      :name="name"
                      :error="error"
                      :error-text="errorText"
                      type="text"/>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';

export default {
  name: 'QuestionString',
  components: {
    GenericTextInput,
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
    required: {
      type: Boolean,
      default: true,
    },
    value: {
      type: String,
      default: '',
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
    stringValue: {
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
      return !isEmpty;
    },
  },
};
</script>
