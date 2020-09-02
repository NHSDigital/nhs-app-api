<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-text-input :id="id"
                        v-model="stringValue"
                        :name="name"
                        :error="error"
                        type="text"
                        :required="required"
                        :maxlength="maxLength"/>
  </div>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { questionStringAnswerValid } from '@/lib/online-consultations/answer-validators';

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
      type: Array,
      default: undefined,
    },
    maxLength: {
      type: String,
      default: undefined,
    },
  },
  data() {
    return {
      stringValue: this.value,
    };
  },
  watch: {
    stringValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionStringAnswerValid(value, this.required, this.maxLength));
    },
  },
};
</script>
