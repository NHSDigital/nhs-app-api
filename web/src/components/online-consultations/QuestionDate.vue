<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <generic-date-input :id="id"
                        v-model="dateValue"
                        :required="required"
                        :name="name"
                        :error="error"
                        :a-described-by="ariaDescribedBy"
    />
  </div>
</template>

<script>
import GenericDateInput from '@/components/widgets/GenericDateInput';
import { questionDateAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionDate',
  components: {
    GenericDateInput,
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
          day: undefined,
          month: undefined,
          year: undefined,
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
      dateValue: this.value,
    };
  },
  computed: {
    ariaDescribedBy() {
      const ariaDescribedContent = [
        this.error && this.errorText ? `${this.id}error` : undefined,
        this.required ? undefined : 'optional-label',
      ].join(' ').trim();
      return ariaDescribedContent || undefined;
    },
  },
  watch: {
    dateValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.dateValue);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionDateAnswerValid(value, this.required));
    },
  },
};
</script>
