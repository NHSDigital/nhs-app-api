<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-text-area :id="id"
                       v-model="textValue"
                       :required="required"
                       :name="name"
                       :error="error"
                       :maxlength="maxLength"
                       :a-described-by="ariaDescribed"/>
  </div>
</template>

<script>
import GenericTextArea from '@/components/widgets/GenericTextArea';
import { questionTextAnswerValid } from '@/lib/online-consultations/answer-validators';

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
    maxLength: {
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
      type: Array,
      default: undefined,
    },
  },
  data() {
    return {
      textValue: this.value,
    };
  },
  computed: {
    ariaDescribed() {
      const ariaDescribedContent = [
        this.error && this.errorText ? `${this.id}error` : undefined,
        this.required ? undefined : 'optional-label',
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
    },
  },
  watch: {
    textValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionTextAnswerValid(value, this.required, this.maxLength));
    },
  },
};
</script>
