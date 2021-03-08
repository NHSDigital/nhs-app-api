<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-time-input :id="id"
                        v-model="timeValue"
                        :required="required"
                        :name="name"
                        :error="error"
                        :a-described-by="ariaDescribed"/>
  </div>
</template>

<script>
import GenericTimeInput from '@/components/widgets/GenericTimeInput';
import { questionTimeAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionTime',
  components: {
    GenericTimeInput,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    name: {
      type: String,
      required: true,
    },
    value: {
      type: Object,
      default() {
        return {
          hour: '',
          minute: '',
        };
      },
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
      timeValue: this.value,
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
    timeValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionTimeAnswerValid(value, this.required));
    },
  },
};
</script>
