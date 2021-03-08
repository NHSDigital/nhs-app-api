<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="error && errorText" :id="errorId" >
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-text-input :id="id"
                        v-model="numberValue"
                        :name="name"
                        :min="min"
                        :max="max"
                        :required="required"
                        :error="error"
                        step="any"
                        type="tel"
                        :a-described-by="ariaDescribed"/>
  </div>
</template>

<script>
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { INTEGER, DECIMAL } from '@/lib/online-consultations/constants/question-types';
import { questionNumberAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionNumber',
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
    max: {
      type: Number,
      default: undefined,
    },
    min: {
      type: Number,
      default: undefined,
    },
    value: {
      type: [Number, String],
      default: undefined,
    },
    type: {
      type: String,
      default: INTEGER,
      validator: value => ([INTEGER, DECIMAL].includes(value)),
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: Array,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      numberValue: this.value,
    };
  },
  computed: {
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
    ariaDescribed() {
      const ariaDescribedContent = [
        this.error && this.errorText ? `${this.id}error` : undefined,
        this.required ? undefined : 'optional-label',
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
    },
  },
  watch: {
    async numberValue(to) {
      this.checkAndEmitIsValueValid(to);
      await this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionNumberAnswerValid(value, this.required, this.type, this.min, this.max));
    },
  },
};
</script>
