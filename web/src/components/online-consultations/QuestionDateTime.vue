<!-- eslint-disable vue/no-v-html -->
<template>
  <div>
    <div v-if="error && errorText">
      <span v-for="singleError in errorText"
            :id="`${id}error`" :key="singleError" class="nhsuk-error-message">
        <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
        {{ singleError }}
      </span>
    </div>
    <div>
      <form>
        <generic-date-input :id="id"
                            v-model="dateTimeValue"
                            :name="name"
                            :error="error"
                            :required="required"/>
      </form>
      <br>
      <form>
        <generic-time-input :id="id"
                            v-model="dateTimeValue"
                            :name="name"
                            :error="error"
                            :required="required"/>
      </form>
    </div>
  </div>
</template>
<script>
import GenericDateInput from '@/components/widgets/GenericDateInput';
import GenericTimeInput from '@/components/widgets/GenericTimeInput';
import { questionDateTimeAnswerValid } from '@/lib/online-consultations/answer-validators';

export default {
  name: 'QuestionDatetime',
  components: {
    GenericDateInput,
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
          day: undefined,
          month: undefined,
          year: undefined,
          hour: undefined,
          minute: undefined,
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
      dateTimeValue: this.value,
    };
  },
  computed: {
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
  },
  watch: {
    dateTimeValue(to) {
      this.checkAndEmitIsValueValid(to);
      this.$emit('input', to);
    },
  },
  created() {
    this.checkAndEmitIsValueValid(this.value);
  },
  methods: {
    checkAndEmitIsValueValid(value) {
      this.$emit('validate', questionDateTimeAnswerValid(value, this.required));
    },
  },
};
</script>
<style module lang="scss" scoped>
</style>
